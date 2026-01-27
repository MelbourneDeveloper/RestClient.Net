/**
 * Chinese i18n Tests
 */
import { test, expect } from '@playwright/test';

test.describe('Chinese Homepage', () => {
  test('/zh/ loads', async ({ page }) => {
    const response = await page.goto('/zh/');
    expect(response?.status()).toBe(200);
  });

  test('/zh/ has lang="zh" attribute', async ({ page }) => {
    await page.goto('/zh/');
    await expect(page.locator('html')).toHaveAttribute('lang', 'zh');
  });

  test('/zh/ has Chinese content', async ({ page }) => {
    await page.goto('/zh/');
    const content = await page.content();
    expect(content).toMatch(/[\u4e00-\u9fff]/);
  });
});

test.describe('Chinese Docs', () => {
  test('/zh/docs/ loads', async ({ page }) => {
    const response = await page.goto('/zh/docs/');
    expect(response?.status()).toBe(200);
  });

  test('/zh/docs/openapi/ loads', async ({ page }) => {
    const response = await page.goto('/zh/docs/openapi/');
    expect(response?.status()).toBe(200);
  });

  test('/zh/docs/mcp/ loads', async ({ page }) => {
    const response = await page.goto('/zh/docs/mcp/');
    expect(response?.status()).toBe(200);
  });

  test('/zh/docs/exhaustion/ loads', async ({ page }) => {
    const response = await page.goto('/zh/docs/exhaustion/');
    expect(response?.status()).toBe(200);
  });
});

test.describe('Chinese Blog', () => {
  test('/zh/blog/ loads', async ({ page }) => {
    const response = await page.goto('/zh/blog/');
    expect(response?.status()).toBe(200);
  });

  test('/zh/blog/ has Chinese title', async ({ page }) => {
    await page.goto('/zh/blog/');
    const h1 = page.locator('h1');
    const text = await h1.textContent();
    expect(text).toContain('博客');
  });
});

test.describe('Chinese API', () => {
  test('/zh/api/ loads', async ({ page }) => {
    const response = await page.goto('/zh/api/');
    expect(response?.status()).toBe(200);
  });
});

test.describe('Language Selector', () => {
  test('language selector exists', async ({ page }) => {
    await page.goto('/');
    const langBtn = page.locator('.language-btn, .language-switcher button, [aria-label*="language"]');
    await expect(langBtn.first()).toBeVisible();
  });

  test('language dropdown has Chinese option', async ({ page }) => {
    await page.goto('/');
    const langBtn = page.locator('.language-btn, .language-switcher button').first();
    await langBtn.click();
    const zhOption = page.locator('.language-dropdown a[lang="zh"], .language-dropdown a:has-text("中文")');
    await expect(zhOption.first()).toBeVisible();
  });
});
