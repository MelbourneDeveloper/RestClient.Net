import { test as base, expect } from '@playwright/test';

export const analyticsId = 'G-PGDS00DBRX';
export const analyticsScriptUrl = `https://www.googletagmanager.com/gtag/js?id=${analyticsId}`;

export async function expectAnalytics(page) {
  const loader = page.locator(`head script[src="${analyticsScriptUrl}"]`);
  await expect(loader, 'Each HTML page loads the requested Google tag once in its head').toHaveCount(1);
  await expect(loader).toHaveAttribute('async', '');
  await expect(page.locator('script[src*="googletagmanager.com"]')).toHaveCount(1);
  const queue = await page.evaluate(() => {
    const calls = Array.isArray(window.dataLayer) ? window.dataLayer.map(entry => Array.from(entry)) : [];
    return {
      commands: calls.map(call => call[0]),
      initializationHasDate: calls[0]?.[1] instanceof Date,
      initializedAt: calls[0]?.[1] instanceof Date ? calls[0][1].getTime() : null,
      config: calls[1],
      gtagType: typeof window.gtag,
    };
  });
  expect(queue.commands, 'Queue initializes before configuring, with no duplicate tag setup').toEqual(['js', 'config']);
  expect(queue.initializationHasDate).toBe(true);
  expect(Number.isFinite(queue.initializedAt)).toBe(true);
  expect(queue.initializedAt).toBeGreaterThan(0);
  expect(queue.config).toEqual(['config', analyticsId]);
  expect(queue.gtagType).toBe('function');
}

export const test = base.extend({
  browserHealth: [async ({ page, context }, use) => {
    // Exercise the real inline initialization without loading Google code or sending visits.
    // Only this exact, authorized URL is intercepted; browser failures stay strict.
    await context.route(analyticsScriptUrl, route => route.fulfill({
      status: 200,
      contentType: 'application/javascript',
      body: '/* Google tag intentionally stubbed in browser tests. */',
    }));
    const errors = [];
    page.on('pageerror', error => errors.push(error.message));
    page.on('console', message => { if (message.type() === 'error') errors.push(message.text()); });
    await use();
    expect(errors, 'Every interaction must finish without browser errors').toEqual([]);
    if (await page.locator('meta[name="viewport"]').count()) {
      await expectAnalytics(page);
      await expect(page.locator('main')).toHaveCount(1);
      await expect(page.locator('h1')).toHaveCount(1);
      await expect(page.locator('html')).toHaveAttribute('lang', /^(en|zh)$/);
      await expect(page).toHaveTitle(/^(?:.+ · )?RestClient\.Net$/);
      await expect(page.locator('meta[name="description"]')).toHaveAttribute('content', /\S/);
      await expect(page.locator('link[rel="canonical"]')).toHaveCount(1);
      await expect(page.locator('link[rel="stylesheet"]')).toHaveCount(1);
      await expect(page.locator('style,[style]'), 'Styles must stay inside the 2500-byte stylesheet budget').toHaveCount(0);
      const remoteStyles = await page.evaluate(() => [...document.styleSheets].filter(sheet => !sheet.href || new URL(sheet.href).origin !== location.origin).length);
      expect(remoteStyles).toBe(0);
    }
  }, { auto: true }],
});

export { expect };
