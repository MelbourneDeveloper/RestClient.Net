/**
 * SEO Tests
 */
import { test, expect } from '@playwright/test';

test.describe('SEO Files', () => {
  test('robots.txt exists', async ({ page }) => {
    const response = await page.goto('/robots.txt');
    expect(response?.status()).toBe(200);
    const content = await page.content();
    expect(content).toContain('User-agent');
  });

  test('sitemap.xml exists', async ({ page }) => {
    const response = await page.goto('/sitemap.xml');
    expect(response?.status()).toBe(200);
  });

  test('feed.xml exists', async ({ page }) => {
    const response = await page.goto('/feed.xml');
    expect(response?.status()).toBe(200);
  });

  test('llms.txt exists', async ({ page }) => {
    const response = await page.goto('/llms.txt');
    expect(response?.status()).toBe(200);
  });
});

test.describe('Meta Tags', () => {
  test('homepage has meta description', async ({ page }) => {
    await page.goto('/');
    const meta = page.locator('meta[name="description"]');
    await expect(meta).toHaveAttribute('content', /.+/);
  });

  test('homepage has og:title', async ({ page }) => {
    await page.goto('/');
    const meta = page.locator('meta[property="og:title"]');
    await expect(meta).toHaveAttribute('content', /.+/);
  });

  test('homepage has twitter:card', async ({ page }) => {
    await page.goto('/');
    const meta = page.locator('meta[name="twitter:card"]');
    await expect(meta).toHaveAttribute('content', /.+/);
  });

  test('homepage has canonical URL', async ({ page }) => {
    await page.goto('/');
    const link = page.locator('link[rel="canonical"]');
    await expect(link).toHaveAttribute('href', /.+/);
  });

  test('homepage has hreflang tags', async ({ page }) => {
    await page.goto('/');
    const zhHreflang = page.locator('link[hreflang="zh"]');
    await expect(zhHreflang).toHaveAttribute('href', /\/zh\//);
  });
});

test.describe('JSON-LD', () => {
  test('homepage has JSON-LD structured data', async ({ page }) => {
    await page.goto('/');
    const jsonLd = page.locator('script[type="application/ld+json"]');
    // Scripts are hidden elements, check count instead of visibility
    const count = await jsonLd.count();
    expect(count).toBeGreaterThan(0);
  });
});
