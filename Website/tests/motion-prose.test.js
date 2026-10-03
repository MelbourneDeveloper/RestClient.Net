import { test, expect } from './fixtures.js';
import { mkdir } from 'node:fs/promises';
import path from 'node:path';

const pixels = canvas => canvas.evaluate(element => {
  const data = element.toDataURL();
  let hash = 2166136261;
  for (let index = 0; index < data.length; index++) hash = Math.imul(hash ^ data.charCodeAt(index), 16777619);
  return hash >>> 0;
});

for (const viewport of [{ width: 1280, height: 800 }, { width: 375, height: 667 }]) {
  test(`all outcome edit cycles change real canvas pixels at ${viewport.width}px`, async ({ page }) => {
    await page.setViewportSize(viewport);
    await page.emulateMedia({ reducedMotion: 'reduce' });
    const outgoing = [];
    page.on('request', request => { if (['fetch', 'xhr'].includes(request.resourceType())) outgoing.push(request.url()); });
    await page.goto('/');
    const canvas = page.locator('#flow-canvas');
    await canvas.scrollIntoViewIfNeeded();
    await expect(canvas).toHaveAttribute('data-motion', 'paused');
    await expect(page.locator('#motion-toggle')).toHaveAttribute('aria-pressed', 'true');
    let previous = await pixels(canvas);
    const signatures = new Map();
    for (const [outcome, status, branch] of [
      ['response', '404 Not Found', 'ResponseErrorPost'],
      ['exception', 'Connection failed', 'ExceptionErrorPost'],
      ['success', '200 OK', 'OkPost'],
      ['response', '404 Not Found', 'ResponseErrorPost'],
      ['success', '200 OK', 'OkPost'],
    ]) {
      const button = page.locator(`button[data-outcome="${outcome}"]`);
      await expect(button).toBeEnabled();
      await button.click();
      await expect(button).toHaveAttribute('aria-pressed', 'true');
      await expect(page.locator('button[data-outcome][aria-pressed="true"]')).toHaveCount(1);
      await expect(page.locator('button[data-outcome][aria-pressed="false"]')).toHaveCount(2);
      await expect(page.locator('#outcome-status')).toContainText(status);
      await expect(page.locator('#outcome-status')).toHaveAttribute('aria-atomic', 'true');
      await expect(page.locator('#outcome-code strong')).toHaveCount(1);
      await expect(page.locator('#outcome-code strong')).toContainText(branch);
      await expect(canvas).toHaveAttribute('data-outcome', outcome);
      await expect(canvas).toHaveAttribute('data-motion', 'paused');
      const current = await pixels(canvas);
      expect(current, 'Changing outcome must redraw real graphics').not.toBe(previous);
      if (signatures.has(outcome)) expect(current, 'Returning to a paused outcome must restore its exact rendering').toBe(signatures.get(outcome));
      signatures.set(outcome, current);
      previous = current;
      expect(await page.evaluate(() => document.documentElement.scrollWidth <= innerWidth)).toBe(true);
      await expect(page.locator('style,[style]')).toHaveCount(0);
    }
    expect(new Set(signatures.values()).size).toBe(3);
    expect(outgoing).toEqual([]);
  });
}

test('animation changes pixels while playing and freezes across pause/resume cycles', async ({ page }) => {
  await page.goto('/');
  const canvas = page.locator('#flow-canvas');
  await canvas.scrollIntoViewIfNeeded();
  const toggle = page.locator('#motion-toggle');
  await expect(canvas).toHaveAttribute('data-motion', 'running');
  let initial = await pixels(canvas);
  await expect.poll(() => pixels(canvas)).not.toBe(initial);
  for (let cycle = 0; cycle < 2; cycle++) {
    await toggle.click();
    await expect(toggle).toHaveText('Play motion');
    await expect(toggle).toHaveAttribute('aria-pressed', 'true');
    await expect(canvas).toHaveAttribute('data-motion', 'paused');
    const paused = await pixels(canvas);
    await page.waitForTimeout(180);
    expect(await pixels(canvas), 'Paused motion must not keep rendering changing pixels').toBe(paused);
    await toggle.click();
    await expect(toggle).toHaveText('Pause motion');
    await expect(toggle).toHaveAttribute('aria-pressed', 'false');
    await expect(canvas).toHaveAttribute('data-motion', 'running');
    initial = await pixels(canvas);
    await expect.poll(() => pixels(canvas)).not.toBe(initial);
  }
});

test('reduced-motion preference freezes graphics and changing the preference updates behavior', async ({ page }) => {
  await page.emulateMedia({ reducedMotion: 'reduce' });
  await page.goto('/');
  const canvas = page.locator('#flow-canvas');
  await canvas.scrollIntoViewIfNeeded();
  await expect(canvas).toHaveAttribute('data-motion', 'paused');
  const initial = await pixels(canvas);
  await page.waitForTimeout(180);
  expect(await pixels(canvas)).toBe(initial);
  expect(await page.locator('html').evaluate(element => getComputedStyle(element).scrollBehavior)).toBe('auto');
  await page.emulateMedia({ reducedMotion: 'no-preference' });
  await expect(canvas).toHaveAttribute('data-motion', 'running');
  await expect.poll(() => pixels(canvas)).not.toBe(initial);
  await page.emulateMedia({ reducedMotion: 'reduce' });
  await expect(canvas).toHaveAttribute('data-motion', 'paused');
  const stopped = await pixels(canvas);
  await page.waitForTimeout(180);
  expect(await pixels(canvas)).toBe(stopped);
});

test('keyboard outcome selection keeps code, live status, and selected graphics synchronized', async ({ page }) => {
  await page.goto('/');
  for (const [outcome, text] of [['exception', 'ExceptionError'], ['response', 'ResponseError'], ['success', '200 OK']]) {
    const button = page.locator(`button[data-outcome="${outcome}"]`);
    await button.focus();
    await page.keyboard.press('Space');
    await expect(button).toBeFocused();
    await expect(button).toHaveAttribute('aria-pressed', 'true');
    await expect(page.locator('#outcome-status')).toContainText(text);
    await expect(page.locator('#flow-canvas')).toHaveAttribute('data-outcome', outcome);
    await expect(page.locator('button[data-outcome][aria-pressed="true"]')).toHaveCount(1);
  }
});

test('docs, blog, and source API reference share the exact same prose CSS', async ({ page }) => {
  const samples = [];
  const screenshots = path.resolve('../.artifacts/website-review');
  await mkdir(screenshots, { recursive: true });
  for (const [route, name] of [['/docs/', 'docs'], ['/blog/introducing-restclient/', 'blog'], ['/api/reference/restclient-net-httpclientextensions/', 'api']]) {
    await page.goto(route);
    await expect(page.locator('.prose')).toHaveCount(1);
    await expect(page.locator('.prose h1')).toHaveCount(1);
    const styles = {};
    for (const selector of ['.prose > p:not(.eyebrow)', '.prose h2', '.prose pre']) {
      const element = page.locator(selector).first();
      await expect(element).toBeVisible();
      styles[selector] = await element.evaluate(node => {
        const style = getComputedStyle(node);
        return Object.fromEntries(['fontFamily', 'fontSize', 'fontWeight', 'lineHeight', 'color', 'letterSpacing'].map(key => [key, style[key]]));
      });
    }
    samples.push(styles);
    await page.screenshot({ path: path.join(screenshots, `${name}-desktop.png`), fullPage: true });
    await page.setViewportSize({ width: 375, height: 667 });
    expect(await page.evaluate(() => document.documentElement.scrollWidth <= innerWidth)).toBe(true);
    await page.screenshot({ path: path.join(screenshots, `${name}-mobile.png`), fullPage: true });
    await page.setViewportSize({ width: 1280, height: 720 });
  }
  expect(samples[1]).toEqual(samples[0]);
  expect(samples[2]).toEqual(samples[0]);
});

test('core content and navigation remain usable with JavaScript disabled', async ({ browser, baseURL }) => {
  const context = await browser.newContext({ javaScriptEnabled: false });
  const page = await context.newPage();
  await page.goto(baseURL);
  await expect(page.locator('.hero h1')).toBeVisible();
  const fallback = page.locator('noscript p');
  await expect(fallback).toBeVisible();
  expect(await fallback.textContent()).toContain('A request produces a success');
  await expect(page.locator('#outcome-code')).toContainText('OkPost');
  await expect(page.locator('button[data-outcome]')).toHaveCount(3);
  for (const button of await page.locator('button[data-outcome]').all()) await expect(button).toBeDisabled();
  await page.locator('.hero-actions a').first().click();
  await expect(page.locator('.prose')).toBeVisible();
  await expect(page.locator('.prose pre code').first()).toBeVisible();
  await context.close();
});

test('every generated API type and its long qualified heading fit mobile width', async ({ page, request }) => {
  test.setTimeout(60000);
  await page.setViewportSize({ width: 375, height: 667 });
  const response = await request.get('/api/reference/api.json');
  expect(response.status()).toBe(200);
  const api = await response.json();
  expect(api.types.length).toBeGreaterThan(20);
  expect(api.types.some(type => type.displayName.includes('OpenApiCodeGenerator'))).toBe(true);
  for (const type of api.types) {
    const loaded = await page.goto(type.url);
    expect(loaded.status(), type.url).toBe(200);
    await expect(page.locator('.prose h1')).toHaveText(type.displayName);
    const bounds = await page.locator('.prose h1').boundingBox();
    expect(bounds.width, type.url).toBeLessThanOrEqual(375);
    expect(await page.evaluate(() => document.documentElement.scrollWidth <= innerWidth), type.url).toBe(true);
    await expect(page.locator('.prose pre code').first()).toBeVisible();
    await expect(page.locator('link[rel="canonical"]')).toHaveAttribute('href', new RegExp(`${type.url}$`));
  }
});

test('page lifecycle suspension freezes graphics and restoration resumes real motion', async ({ page }) => {
  await page.goto('/');
  const canvas = page.locator('#flow-canvas');
  await canvas.scrollIntoViewIfNeeded();
  const running = await pixels(canvas);
  await expect.poll(() => pixels(canvas)).not.toBe(running);
  await page.evaluate(() => dispatchEvent(new PageTransitionEvent('pagehide', { persisted: true })));
  const suspended = await pixels(canvas);
  await page.waitForTimeout(180);
  expect(await pixels(canvas)).toBe(suspended);
  await page.evaluate(() => dispatchEvent(new PageTransitionEvent('pageshow', { persisted: true })));
  await expect.poll(() => pixels(canvas)).not.toBe(suspended);
  await expect(page.locator('#motion-toggle')).toHaveAttribute('aria-pressed', 'false');
  await expect(canvas).toHaveAttribute('data-motion', 'running');
});
