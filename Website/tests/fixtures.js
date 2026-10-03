import { test as base, expect } from '@playwright/test';

export const test = base.extend({
  browserHealth: [async ({ page }, use) => {
    const errors = [];
    page.on('pageerror', error => errors.push(error.message));
    page.on('console', message => { if (message.type() === 'error') errors.push(message.text()); });
    await use();
    expect(errors, 'Every interaction must finish without browser errors').toEqual([]);
    if (await page.locator('meta[name="viewport"]').count()) {
      await expect(page.locator('main')).toHaveCount(1);
      await expect(page.locator('h1')).toHaveCount(1);
      await expect(page.locator('html')).toHaveAttribute('lang', /^(en|zh)$/);
      await expect(page).toHaveTitle(/RestClient\.Net/);
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
