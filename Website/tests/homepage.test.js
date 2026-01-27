/**
 * Homepage Tests
 */
import { test, expect } from '@playwright/test';

test.describe('Homepage', () => {
  test('homepage loads', async ({ page }) => {
    const response = await page.goto('/');
    expect(response?.status()).toBe(200);
  });

  test('has correct title', async ({ page }) => {
    await page.goto('/');
    await expect(page).toHaveTitle(/RestClient\.Net/);
  });

  test('has hero section', async ({ page }) => {
    await page.goto('/');
    const hero = page.locator('.hero');
    await expect(hero).toBeVisible();
  });

  test('has feature cards', async ({ page }) => {
    await page.goto('/');
    const features = page.locator('.feature-card');
    const count = await features.count();
    expect(count).toBeGreaterThan(0);
  });

  test('has navigation', async ({ page }) => {
    await page.goto('/');
    const nav = page.locator('nav, .nav');
    await expect(nav.first()).toBeVisible();
  });

  test('has docs link in nav', async ({ page }) => {
    await page.goto('/');
    const docsLink = page.locator('a[href="/docs/"], a[href*="docs"]').first();
    await expect(docsLink).toBeVisible();
  });

  test('has code example', async ({ page }) => {
    await page.goto('/');
    const code = page.locator('pre code');
    await expect(code.first()).toBeVisible();
  });
});
