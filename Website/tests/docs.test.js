/**
 * Documentation Tests
 */
import { test, expect } from '@playwright/test';

test.describe('Documentation', () => {
  test('docs index loads', async ({ page }) => {
    const response = await page.goto('/docs/');
    expect(response?.status()).toBe(200);
  });

  test('docs has title', async ({ page }) => {
    await page.goto('/docs/');
    const h1 = page.locator('h1');
    await expect(h1).toContainText(/Getting Started|RestClient/);
  });

  test('docs has code examples', async ({ page }) => {
    await page.goto('/docs/');
    const code = page.locator('pre code');
    await expect(code.first()).toBeVisible();
  });

  test('openapi docs loads', async ({ page }) => {
    const response = await page.goto('/docs/openapi/');
    expect(response?.status()).toBe(200);
  });

  test('mcp docs loads', async ({ page }) => {
    const response = await page.goto('/docs/mcp/');
    expect(response?.status()).toBe(200);
  });

  test('exhaustion docs loads', async ({ page }) => {
    const response = await page.goto('/docs/exhaustion/');
    expect(response?.status()).toBe(200);
  });
});
