#!/usr/bin/env node
import { readFile, readdir, stat } from 'node:fs/promises';
import path from 'node:path';
import { fileURLToPath } from 'node:url';
import { parse as parseHtml } from 'parse5';
import { parse as parseJavaScript } from 'acorn';

export const CSS_BUDGET = 2500;
const AUTHORIZED_GOOGLE_TAG = 'https://www.googletagmanager.com/gtag/js?id=G-PGDS00DBRX';

async function filesUnder(directory) {
  const entries = await readdir(directory, { withFileTypes: true });
  return (await Promise.all(entries.map(entry => entry.isDirectory()
    ? filesUnder(path.join(directory, entry.name)) : path.join(directory, entry.name)))).flat();
}

function elements(document) {
  const result = [];
  const visit = node => {
    if (node.tagName) result.push(node);
    for (const child of node.childNodes ?? []) visit(child);
    if (node.content) visit(node.content);
  };
  visit(document);
  return result;
}

const attribute = (node, name) => node.attrs?.find(item => item.name === name)?.value;
const content = node => node.nodeName === '#text' ? node.value : (node.childNodes ?? []).map(content).join('');
const routeFor = file => `/${file.replaceAll(path.sep, '/').replace(/index\.html$/, '')}`;
const decodeXml = value => value.replace(/&(amp|quot|apos|lt|gt);/g, (_, entity) => ({ amp: '&', quot: '"', apos: "'", lt: '<', gt: '>' })[entity]);

export function cssInjectionErrors(source, filename = 'script.js') {
  const errors = [];
  let tree;
  try { tree = parseJavaScript(source, { ecmaVersion: 'latest', sourceType: 'module' }); }
  catch (error) { return [`${filename}: invalid JavaScript: ${error.message}`]; }
  const literal = node => node?.type === 'Literal' ? node.value : node?.type === 'BinaryExpression' && node.operator === '+' && typeof literal(node.left) === 'string' && typeof literal(node.right) === 'string' ? literal(node.left) + literal(node.right) : undefined;
  const property = node => node?.computed ? literal(node.property) : node?.property?.name;
  const forbidden = new Set(['style', 'cssText', 'styleSheet', 'adoptedStyleSheets', 'CSSStyleSheet', 'insertRule', 'addRule', 'replaceSync', 'setProperty', 'animate']);
  const visit = node => {
    if (!node || typeof node !== 'object') return;
    if (node.type === 'MemberExpression' && forbidden.has(property(node))) errors.push(`${filename}: CSS injection through ${property(node)} is outside the stylesheet budget`);
    if (node.type === 'Identifier' && node.name === 'CSSStyleSheet') errors.push(`${filename}: constructed stylesheets bypass the CSS budget`);
    if (node.type === 'CallExpression' && property(node.callee) === 'createElement' && node.arguments[0]?.value?.toLowerCase() === 'style') errors.push(`${filename}: dynamically created style element`);
    if (node.type === 'CallExpression' && property(node.callee) === 'setAttribute' && node.arguments[0]?.value?.toLowerCase() === 'style') errors.push(`${filename}: dynamically created style attribute`);
    if (node.type === 'AssignmentExpression' && property(node.left) === 'rel' && literal(node.right) === 'stylesheet') errors.push(`${filename}: dynamically created stylesheet link`);
    if (node.type === 'CallExpression' && property(node.callee) === 'setAttribute' && literal(node.arguments[0]) === 'rel' && literal(node.arguments[1]) === 'stylesheet') errors.push(`${filename}: dynamically created stylesheet link`);
    const value = node.type === 'Literal' ? node.value : node.type === 'TemplateElement' ? node.value.raw : null;
    if (typeof value === 'string' && /<style\b|<[a-z][^>]*\sstyle\s*=/i.test(value)) errors.push(`${filename}: HTML string injects unbudgeted styles`);
    for (const [key, value] of Object.entries(node)) {
      if (key === 'start' || key === 'end') continue;
      if (Array.isArray(value)) value.forEach(visit);
      else if (value && typeof value === 'object') visit(value);
    }
  };
  visit(tree);
  return [...new Set(errors)];
}

export async function auditSite(directory, options = {}) {
  const root = path.resolve(directory);
  const configured = new URL(options.siteUrl ?? process.env.SITE_URL ?? 'https://melbournedeveloper.github.io');
  const prefix = `/${(options.pathPrefix ?? process.env.SITE_PATH_PREFIX ?? configured.pathname).replace(/^\/+|\/+$/g, '')}`.replace(/\/?$/, '/');
  const absolute = route => new URL(`${prefix}${route.replace(/^\//, '')}`, configured.origin).href;
  const errors = [];
  const files = await filesUnder(root);
  const css = files.filter(file => /\.css$/i.test(file));
  const cssBytes = (await Promise.all(css.map(file => stat(file)))).reduce((sum, file) => sum + file.size, 0);
  if (!css.length) errors.push('No stylesheet was emitted');
  if (cssBytes > (options.cssBudget ?? CSS_BUDGET)) errors.push(`CSS budget exceeded: ${cssBytes} raw UTF-8 bytes > ${options.cssBudget ?? CSS_BUDGET}`);
  for (const file of css) if (/@import\b/i.test(await readFile(file, 'utf8'))) errors.push(`${path.relative(root, file)}: @import can load unbudgeted CSS`);
  for (const file of files.filter(file => /\.svg$/i.test(file))) {
    const nodes = elements(parseHtml(await readFile(file, 'utf8')));
    if (nodes.some(node => node.tagName === 'style' || attribute(node, 'style') !== undefined)) errors.push(`${path.relative(root, file)}: SVG styles bypass the shared stylesheet budget`);
  }

  const pages = new Map();
  for (const file of files.filter(file => file.endsWith('.html'))) {
    const relative = path.relative(root, file);
    const html = await readFile(file, 'utf8');
    const nodes = elements(parseHtml(html));
    const route = routeFor(relative);
    pages.set(route, { relative, html, nodes, url: absolute(route), ids: new Set(nodes.map(node => attribute(node, 'id')).filter(Boolean)) });
  }
  if (!pages.size) errors.push('No HTML pages were emitted');

  const resolveLocal = (reference, page) => {
    if (/^(?:mailto:|tel:|data:|javascript:)/i.test(reference)) return null;
    let url;
    try { url = new URL(reference, page.url); } catch { errors.push(`${page.relative}: invalid URL ${reference}`); return null; }
    if (url.origin !== configured.origin) return null;
    if (!url.pathname.startsWith(prefix)) { errors.push(`${page.relative}: link escapes deployment prefix: ${reference}`); return null; }
    let relative;
    try { relative = decodeURIComponent(url.pathname.slice(prefix.length)); } catch { errors.push(`${page.relative}: invalid encoded URL ${reference}`); return null; }
    let file = path.resolve(root, relative.endsWith('/') || !relative ? `${relative}index.html` : relative);
    if (!files.includes(file) && files.includes(path.join(file, 'index.html'))) file = path.join(file, 'index.html');
    if (file !== root && !file.startsWith(`${root}${path.sep}`)) { errors.push(`${page.relative}: link escapes output directory`); return null; }
    if (!files.includes(file)) errors.push(`${page.relative}: broken internal link ${reference}`);
    const target = pages.get(routeFor(path.relative(root, file)));
    if (url.hash && !url.hash.startsWith('#:~:') && target) {
      let fragment;
      try { fragment = decodeURIComponent(url.hash.slice(1)); } catch { fragment = url.hash.slice(1); }
      if (!target.ids.has(fragment)) errors.push(`${page.relative}: missing anchor ${reference}`);
    }
    return { url, target };
  };

  const titles = new Map();
  for (const page of pages.values()) {
    const { nodes, relative, url } = page;
    const matches = tag => nodes.filter(node => node.tagName === tag);
    const meta = name => nodes.find(node => node.tagName === 'meta' && (attribute(node, 'name') === name || attribute(node, 'property') === name));
    const title = matches('title').map(content).join('').trim();
    if (!title) errors.push(`${relative}: missing page title`);
    if (titles.has(title)) errors.push(`${relative}: duplicate title also used by ${titles.get(title)}`);
    titles.set(title, relative);
    if (matches('h1').length !== 1) errors.push(`${relative}: expected exactly one h1`);
    if (!attribute(meta('description') ?? {}, 'content')?.trim()) errors.push(`${relative}: missing description`);
    if (!attribute(meta('viewport') ?? {}, 'content')?.includes('width=device-width')) errors.push(`${relative}: missing responsive viewport`);
    if (attribute(meta('robots') ?? {}, 'content')?.includes('noindex')) errors.push(`${relative}: unintended noindex`);
    const canonicals = matches('link').filter(node => attribute(node, 'rel') === 'canonical');
    if (canonicals.length !== 1 || attribute(canonicals[0], 'href') !== url) errors.push(`${relative}: canonical must be ${url}`);
    for (const name of ['og:title', 'og:description', 'og:image', 'og:url', 'og:type', 'twitter:card', 'twitter:title', 'twitter:description', 'twitter:image']) {
      if (!attribute(meta(name) ?? {}, 'content')) errors.push(`${relative}: missing ${name}`);
    }
    if (attribute(meta('og:url') ?? {}, 'content') !== url) errors.push(`${relative}: og:url differs from canonical`);
    const schemas = matches('script').filter(node => attribute(node, 'type') === 'application/ld+json');
    if (!schemas.length) errors.push(`${relative}: missing structured data`);
    for (const schema of schemas) {
      try { JSON.parse(content(schema)); } catch { errors.push(`${relative}: invalid JSON-LD`); }
    }
    for (const node of nodes) {
      if (node.tagName === 'style' || attribute(node, 'style') !== undefined) errors.push(`${relative}: inline CSS bypasses the shared stylesheet budget`);
      if (node.tagName === 'img' && attribute(node, 'alt') === undefined) errors.push(`${relative}: image missing alt text`);
      for (const name of ['href', 'src', 'action']) {
        const reference = attribute(node, name);
        if (reference) resolveLocal(reference, page);
      }
      if (node.tagName === 'link' && (attribute(node, 'rel')?.split(/\s+/).includes('stylesheet') || attribute(node, 'as') === 'style')) {
        const href = attribute(node, 'href') ?? '';
        const target = new URL(href, url);
        if (target.origin !== configured.origin || !target.pathname.endsWith('.css')) errors.push(`${relative}: external or embedded stylesheet ${href}`);
      }
      if (node.tagName === 'script' && attribute(node, 'type') !== 'application/ld+json') {
        const src = attribute(node, 'src');
        if (src && new URL(src, url).origin !== configured.origin && src !== AUTHORIZED_GOOGLE_TAG) errors.push(`${relative}: external JavaScript could inject unbudgeted styles`);
        if (!src && content(node).trim()) errors.push(...cssInjectionErrors(content(node), relative));
      }
      if (node.tagName === 'link' && attribute(node, 'hreflang')) {
        const reference = attribute(node, 'href');
        const target = reference ? resolveLocal(reference, page)?.target : null;
        const language = attribute(node, 'hreflang');
        if (!target) errors.push(`${relative}: hreflang does not point to a real local page`);
        else if (language !== 'x-default' && attribute(target.nodes.find(item => item.tagName === 'html'), 'lang') !== language) errors.push(`${relative}: hreflang language differs from destination`);
      }
    }
  }
  for (const file of files.filter(file => /\.(?:m?js)$/i.test(file))) errors.push(...cssInjectionErrors(await readFile(file, 'utf8'), path.relative(root, file)));

  const required = ['sitemap.xml', 'feed.xml', 'robots.txt', 'llms.txt'];
  for (const filename of required) if (!files.includes(path.join(root, filename))) errors.push(`Missing ${filename}`);
  if (files.includes(path.join(root, 'sitemap.xml'))) {
    const sitemap = await readFile(path.join(root, 'sitemap.xml'), 'utf8');
    const urls = [...sitemap.matchAll(/<loc>(.*?)<\/loc>/gs)].map(match => decodeXml(match[1].trim()));
    const expected = new Set([...pages.values()].map(page => page.url));
    if (new Set(urls).size !== urls.length) errors.push('Sitemap contains duplicate pages');
    for (const url of urls) if (!expected.has(url)) errors.push(`Sitemap has a nonexistent page: ${url}`);
    for (const url of expected) if (!urls.includes(url)) errors.push(`Sitemap omits ${url}`);
  }
  if (files.includes(path.join(root, 'robots.txt')) && !(await readFile(path.join(root, 'robots.txt'), 'utf8')).includes(`Sitemap: ${absolute('/sitemap.xml')}`)) errors.push('robots.txt points to the wrong sitemap');
  if (files.includes(path.join(root, 'feed.xml'))) {
    const feed = await readFile(path.join(root, 'feed.xml'), 'utf8');
    for (const match of feed.matchAll(/<link\s+[^>]*href="([^"]+)"/g)) resolveLocal(decodeXml(match[1]), { relative: 'feed.xml', url: absolute('/feed.xml') });
  }
  return { cssBytes, cssBudget: options.cssBudget ?? CSS_BUDGET, pageCount: pages.size, errors: [...new Set(errors)] };
}

if (process.argv[1] && path.resolve(process.argv[1]) === fileURLToPath(import.meta.url)) {
  const report = await auditSite(process.argv[2] ?? '_site');
  console.log(`Website: ${report.pageCount} pages; CSS ${report.cssBytes}/${report.cssBudget} raw bytes.`);
  if (report.errors.length) {
    console.error(report.errors.join('\n'));
    process.exitCode = 1;
  }
}
