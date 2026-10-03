/**
 * Blog Tests
 */
import { test, expect } from './fixtures.js';

test.describe('Blog', () => {
  test('blog index loads', async ({ page }) => {
    const response = await page.goto('/blog/');
    expect(response?.status()).toBe(200);
  });

  test('blog has title', async ({ page }) => {
    await page.goto('/blog/');
    const h1 = page.locator('h1');
    await expect(h1).toHaveText('Ideas worth building on.');
    await expect(page).toHaveTitle(/Journal/);
  });

  test('blog has post list', async ({ page }) => {
    await page.goto('/blog/');
    const posts = page.locator('article.feature-card');
    const count = await posts.count();
    expect(count).toBeGreaterThan(0);
  });

  test('blog post page loads', async ({ page }) => {
    const response = await page.goto('/blog/introducing-restclient/');
    expect(response?.status()).toBe(200);
  });

  test('blog post has back link', async ({ page }) => {
    await page.goto('/blog/introducing-restclient/');
    const backLink = page.locator('a[href="/blog/"]');
    await expect(backLink.first()).toBeVisible();
  });
});
