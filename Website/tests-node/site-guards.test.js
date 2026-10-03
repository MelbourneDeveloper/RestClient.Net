import test from 'node:test';
import assert from 'node:assert/strict';
import { mkdtemp, mkdir, writeFile, rm } from 'node:fs/promises';
import { tmpdir } from 'node:os';
import path from 'node:path';
import { auditSite, cssInjectionErrors, CSS_BUDGET } from '../scripts/check-site.js';

async function fixture(t, { prefix = '/', stylesheet = 'body{color:#123}' } = {}) {
  const directory = await mkdtemp(path.join(tmpdir(), 'restclient-site-guard-'));
  t.after(() => rm(directory, { recursive: true, force: true }));
  const siteUrl = 'https://example.test';
  const absolute = route => `${siteUrl}${prefix}${route.replace(/^\//, '')}`;
  const write = async (filename, value) => {
    await mkdir(path.dirname(path.join(directory, filename)), { recursive: true });
    await writeFile(path.join(directory, filename), value);
  };
  const html = (body = '<h1>RestClient.Net</h1>', extra = '') => `<!doctype html><html lang="en"><head>
<title>RestClient.Net documentation</title><meta name="description" content="Documented HTTP results">
<meta name="viewport" content="width=device-width,initial-scale=1">
<link rel="canonical" href="${absolute('/')}"><link rel="stylesheet" href="${prefix}assets/site.css">
${['og:title', 'og:description', 'og:image', 'og:type', 'twitter:card', 'twitter:title', 'twitter:description', 'twitter:image'].map(name => `<meta ${name.startsWith('og:') ? 'property' : 'name'}="${name}" content="RestClient.Net">`).join('')}
<meta property="og:url" content="${absolute('/')}">
<script type="application/ld+json">{"@context":"https://schema.org","@type":"WebSite","name":"RestClient.Net"}</script>${extra}</head><body>${body}</body></html>`;
  await write('index.html', html());
  await write('assets/site.css', stylesheet);
  await write('sitemap.xml', `<urlset><url><loc>${absolute('/')}</loc></url></urlset>`);
  await write('robots.txt', `User-agent: *\nAllow: /\nSitemap: ${absolute('/sitemap.xml')}\n`);
  await write('feed.xml', `<feed><link href="${absolute('/')}" /></feed>`);
  await write('llms.txt', `# RestClient.Net\n\n[Documentation](${absolute('/')})`);
  return { directory, write, html, run: () => auditSite(directory, { siteUrl, pathPrefix: prefix }) };
}

test('raw CSS budget accepts exactly 2500 bytes and rejects one extra byte', async t => {
  const site = await fixture(t, { stylesheet: ' '.repeat(CSS_BUDGET) });
  const exact = await site.run();
  assert.equal(exact.cssBytes, 2500);
  assert.deepEqual(exact.errors, []);
  await site.write('assets/site.css', ' '.repeat(CSS_BUDGET + 1));
  const overflow = await site.run();
  assert.equal(overflow.cssBytes, 2501);
  assert.match(overflow.errors.join('\n'), /CSS budget exceeded/);
});

test('CSS budget totals all files and measures UTF-8 bytes rather than characters', async t => {
  const site = await fixture(t, { stylesheet: ' '.repeat(2499) });
  await site.write('assets/hidden/extra.css', 'é');
  const report = await site.run();
  assert.equal(report.cssBytes, 2501);
  assert.match(report.errors.join('\n'), /CSS budget exceeded/);
  await site.write('assets/hidden/extra.css', 'a');
  assert.deepEqual((await site.run()).errors, []);
});

test('real inline styles fail while escaped code examples remain valid', async t => {
  const site = await fixture(t);
  await site.write('index.html', site.html('<h1>Docs</h1><code>&lt;p style="color:red"&gt;</code>'));
  assert.deepEqual((await site.run()).errors, []);
  for (const body of ['<h1 style="color:red">Docs</h1>', '<h1>Docs</h1><style>p{color:red}</style>', '<h1>Docs</h1><template><div style="color:red"></div></template>']) {
    await site.write('index.html', site.html(body));
    assert.match((await site.run()).errors.join('\n'), /inline CSS bypasses/);
  }
});

test('external styles, data CSS, imports, and stylesheet preloads cannot bypass budget', async t => {
  const site = await fixture(t);
  for (const link of ['<link rel="stylesheet" href="https://cdn.test/theme.css">', '<link rel="stylesheet" href="data:text/css,p{color:red}">', '<link rel="preload" as="style" href="https://cdn.test/theme.css">']) {
    await site.write('index.html', site.html('<h1>Docs</h1>', link));
    assert.match((await site.run()).errors.join('\n'), /external or embedded stylesheet/);
  }
  await site.write('index.html', site.html());
  await site.write('assets/site.css', '@import "https://cdn.test/theme.css";');
  assert.match((await site.run()).errors.join('\n'), /unbudgeted CSS/);
});

test('JavaScript CSS injection is rejected across DOM, CSSOM, and generated markup', () => {
  for (const source of [
    'document.body.style.color="red";',
    'document.body["style"].cssText="color:red";',
    'document.body["st"+"yle"].color="red";',
    'document.createElement("style");',
    'document["createElement"]("style");',
    'document.body.setAttribute("style","color:red");',
    'new CSSStyleSheet().replaceSync("body{}");',
    'document.adoptedStyleSheets=[];',
    'sheet.insertRule("body{}");',
    'document.body.animate([{opacity:0},{opacity:1}],100);',
    'document.body.innerHTML=`<span style="color:red">hi</span>`;',
    'link.rel="stylesheet";',
    'link.setAttribute("rel","stylesheet");',
  ]) assert.ok(cssInjectionErrors(source).length > 0, source);
});

test('native canvas rendering and comment text do not count as CSS injection', () => {
  const source = '// document.body.style.color="red";\nconst canvas=document.querySelector("canvas");const ctx=canvas.getContext("2d");ctx.fillStyle="#123";ctx.fillRect(0,0,10,10);requestAnimationFrame(()=>{});';
  assert.deepEqual(cssInjectionErrors(source), []);
  assert.match(cssInjectionErrors('const = ;').join('\n'), /invalid JavaScript/);
});

test('external scripts cannot outsource hidden CSS', async t => {
  const site = await fixture(t);
  await site.write('index.html', site.html('<h1>Docs</h1>', '<script src="https://cdn.test/theme.js"></script>'));
  assert.match((await site.run()).errors.join('\n'), /external JavaScript/);
});

test('the exact user-authorized Google tag works on root and prefixed deployments', async t => {
  for (const prefix of ['/', '/RestClient.Net/']) {
    const site = await fixture(t, { prefix });
    await site.write('index.html', site.html('<h1>Docs</h1>', `
<script async src="https://www.googletagmanager.com/gtag/js?id=G-PGDS00DBRX"></script>
<script>
window.dataLayer = window.dataLayer || [];
function gtag(){dataLayer.push(arguments);}
gtag('js', new Date());
gtag('config', 'G-PGDS00DBRX');
</script>`));
    const report = await site.run();
    assert.deepEqual(report.errors, []);
    assert.equal(report.cssBytes, Buffer.byteLength('body{color:#123}', 'utf8'));
    assert.equal(report.pageCount, 1);
  }
});

test('Google tag exception rejects every altered origin, path, measurement ID, and query', async t => {
  const site = await fixture(t);
  for (const src of [
    'http://www.googletagmanager.com/gtag/js?id=G-PGDS00DBRX',
    '//www.googletagmanager.com/gtag/js?id=G-PGDS00DBRX',
    'https://googletagmanager.com/gtag/js?id=G-PGDS00DBRX',
    'https://www.googletagmanager.com.evil.test/gtag/js?id=G-PGDS00DBRX',
    'https://www.googletagmanager.com@evil.test/gtag/js?id=G-PGDS00DBRX',
    'https://www.googletagmanager.com:444/gtag/js?id=G-PGDS00DBRX',
    'https://www.googletagmanager.com/gtm.js?id=G-PGDS00DBRX',
    'https://www.googletagmanager.com/gtag/js/extra?id=G-PGDS00DBRX',
    'https://www.googletagmanager.com/gtag/js?id=G-OTHER',
    'https://www.googletagmanager.com/gtag/js',
    'https://www.googletagmanager.com/gtag/js?other=G-PGDS00DBRX',
    'https://www.googletagmanager.com/gtag/js?id=G-PGDS00DBRX&extra=1',
    'https://www.googletagmanager.com/gtag/js?id=G-PGDS00DBRX&id=G-OTHER',
    'https://www.googletagmanager.com/gtag/js?id=G-PGDS00DBRX#extra',
    'https://www.googletagmanager.com/gtag/js?id=%47-PGDS00DBRX',
  ]) {
    await site.write('index.html', site.html('<h1>Docs</h1>', `<script async src="${src}"></script>`));
    const report = await site.run();
    assert.ok(report.errors.some(error => error.includes('external JavaScript')), src);
    assert.equal(report.cssBytes, Buffer.byteLength('body{color:#123}', 'utf8'));
  }
});

test('authorized Google tag does not exempt any CSS or other external scripts', async t => {
  const site = await fixture(t, { stylesheet: ' '.repeat(CSS_BUDGET + 1) });
  await site.write('index.html', site.html('<h1 style="color:red">Docs</h1>', `
<script async src="https://www.googletagmanager.com/gtag/js?id=G-PGDS00DBRX"></script>
<script src="https://cdn.test/unapproved.js"></script>
<link rel="stylesheet" href="https://www.googletagmanager.com/gtag/js?id=G-PGDS00DBRX">`));
  const errors = (await site.run()).errors.join('\n');
  assert.match(errors, /CSS budget exceeded/);
  assert.match(errors, /inline CSS bypasses/);
  assert.match(errors, /external or embedded stylesheet/);
  assert.match(errors, /external JavaScript/);
});

test('SVG assets cannot conceal extra CSS outside the shared budget', async t => {
  const site = await fixture(t);
  for (const svg of ['<svg><style>text{font-size:99px}</style></svg>', '<svg><text style="font-size:99px">Hello</text></svg>']) {
    await site.write('assets/graphic.svg', svg);
    assert.match((await site.run()).errors.join('\n'), /SVG styles bypass/);
  }
  await site.write('assets/graphic.svg', '<svg><path fill="#123" d="M0 0h10v10z" /></svg>');
  assert.deepEqual((await site.run()).errors, []);
});

test('local script injection is checked even when the initial HTML is clean', async t => {
  const site = await fixture(t);
  await site.write('assets/motion.js', 'document.body.style.background="red";');
  assert.match((await site.run()).errors.join('\n'), /CSS injection/);
});

test('root and project-prefix deployments use real canonical and internal routes', async t => {
  for (const prefix of ['/', '/RestClient.Net/']) {
    const site = await fixture(t, { prefix });
    await site.write('index.html', site.html(`<h1 id="top">Docs</h1><a href="${prefix}#top">Top</a>`));
    assert.deepEqual((await site.run()).errors, []);
    if (prefix !== '/') {
      await site.write('index.html', site.html('<h1>Docs</h1><a href="/">Wrong deployment root</a>'));
      assert.match((await site.run()).errors.join('\n'), /escapes deployment prefix/);
    }
  }
});

test('missing local routes and anchor fragments fail the build', async t => {
  const site = await fixture(t);
  await site.write('index.html', site.html('<h1>Docs</h1><a href="/missing/">Missing</a><a href="#missing">Missing heading</a>'));
  const errors = (await site.run()).errors.join('\n');
  assert.match(errors, /broken internal link/);
  assert.match(errors, /missing anchor/);
});

test('hreflang cannot advertise a missing translation', async t => {
  const site = await fixture(t);
  await site.write('index.html', site.html('<h1>Docs</h1>', '<link rel="alternate" hreflang="zh" href="https://example.test/zh/">'));
  const errors = (await site.run()).errors.join('\n');
  assert.match(errors, /hreflang does not point to a real local page/);
  assert.match(errors, /broken internal link/);
});

test('canonical, structured data, and crawl output regressions fail visibly', async t => {
  const site = await fixture(t);
  await site.write('index.html', site.html().replace('rel="canonical"', 'rel="unrelated"').replace('"@context":', 'broken:'));
  await site.write('sitemap.xml', '<urlset><url><loc>https://example.test/missing/</loc></url></urlset>');
  await site.write('robots.txt', 'User-agent: *');
  const errors = (await site.run()).errors.join('\n');
  assert.match(errors, /canonical must be/);
  assert.match(errors, /invalid JSON-LD/);
  assert.match(errors, /Sitemap has a nonexistent page/);
  assert.match(errors, /Sitemap omits/);
  assert.match(errors, /wrong sitemap/);
});

test('sitemap URL validation decodes exactly one XML entity layer', async t => {
  const site = await fixture(t);
  await site.write('sitemap.xml', '<urlset><url><loc>https://example.test/&amp;lt;missing&amp;gt;</loc></url></urlset>');
  const report = await site.run();
  assert.ok(report.errors.includes('Sitemap has a nonexistent page: https://example.test/&lt;missing&gt;'));
  assert.ok(!report.errors.some(error => error.includes('https://example.test/<missing>')));
});
