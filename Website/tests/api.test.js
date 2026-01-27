/**
 * API Reference Tests
 */
import { test, expect } from '@playwright/test';

test.describe('API Reference', () => {
  test('API index loads', async ({ page }) => {
    const response = await page.goto('/api/');
    expect(response?.status()).toBe(200);
  });

  test('API page has title', async ({ page }) => {
    await page.goto('/api/');
    const h1 = page.locator('h1');
    await expect(h1).toContainText('API');
  });

  test('API page has package links', async ({ page }) => {
    await page.goto('/api/');
    const links = page.locator('.feature-card, a[href*="docs"]');
    const count = await links.count();
    expect(count).toBeGreaterThan(0);
  });
});

test.describe('API Documentation Pages', () => {
  test('HttpClient Extensions page loads', async ({ page }) => {
    const response = await page.goto('/api/httpclient-extensions/');
    expect(response?.status()).toBe(200);
  });

  test('HttpClient Extensions has method docs', async ({ page }) => {
    await page.goto('/api/httpclient-extensions/');
    const content = await page.content();
    expect(content).toContain('GetAsync');
    expect(content).toContain('PostAsync');
  });

  test('Result Types page loads', async ({ page }) => {
    const response = await page.goto('/api/result-types/');
    expect(response?.status()).toBe(200);
  });

  test('Result Types has type documentation', async ({ page }) => {
    await page.goto('/api/result-types/');
    const content = await page.content();
    expect(content).toContain('HttpError');
    expect(content).toContain('pattern matching');
  });

  test('OpenAPI Generator page loads', async ({ page }) => {
    const response = await page.goto('/api/openapi-generator/');
    expect(response?.status()).toBe(200);
  });

  test('OpenAPI Generator has CLI docs', async ({ page }) => {
    await page.goto('/api/openapi-generator/');
    const content = await page.content();
    expect(content).toContain('OpenApiGenerator');
  });

  test('MCP Generator page loads', async ({ page }) => {
    const response = await page.goto('/api/mcp-generator/');
    expect(response?.status()).toBe(200);
  });

  test('MCP Generator has MCP protocol docs', async ({ page }) => {
    await page.goto('/api/mcp-generator/');
    const content = await page.content();
    expect(content).toContain('Model Context Protocol');
  });
});
