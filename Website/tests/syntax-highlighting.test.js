/**
 * Syntax Highlighting Tests
 * Verifies that ALL pages with code blocks have proper Prism syntax highlighting
 * NO SKIPPING - FAIL HARD if anything is wrong!
 */
import { test, expect } from '@playwright/test';

// All pages that MUST contain code blocks with syntax highlighting
// If any page doesn't have code or highlighting, TEST FAILS!
const pagesWithCode = [
  // English pages
  { url: '/', name: 'Homepage', minTokens: 20 },
  { url: '/docs/', name: 'Docs index', minTokens: 10 },
  { url: '/docs/basic-usage/', name: 'Basic Usage', minTokens: 20 },
  { url: '/docs/error-handling/', name: 'Error Handling', minTokens: 20 },
  { url: '/docs/advanced-usage/', name: 'Advanced Usage', minTokens: 20 },
  { url: '/docs/exhaustion/', name: 'Exhaustion', minTokens: 10 },
  { url: '/docs/openapi/', name: 'OpenAPI', minTokens: 10 },
  { url: '/docs/mcp/', name: 'MCP', minTokens: 10 },
  { url: '/examples/', name: 'Examples', minTokens: 100 },
  { url: '/blog/introducing-restclient/', name: 'Blog post', minTokens: 10 },

  // Chinese pages
  { url: '/zh/', name: 'Chinese Homepage', minTokens: 20 },
  { url: '/zh/docs/', name: 'Chinese Docs index', minTokens: 10 },
  { url: '/zh/docs/basic-usage/', name: 'Chinese Basic Usage', minTokens: 20 },
  { url: '/zh/docs/error-handling/', name: 'Chinese Error Handling', minTokens: 20 },
  { url: '/zh/examples/', name: 'Chinese Examples', minTokens: 100 },
];

test.describe('Syntax Highlighting - All Pages MUST Have Tokens', () => {
  for (const { url, name, minTokens } of pagesWithCode) {
    test(`${name} (${url}) MUST have at least ${minTokens} syntax tokens`, async ({ page }) => {
      await page.goto(url);

      // Page MUST have code blocks - NO EXCEPTIONS
      const codeBlocks = page.locator('pre code');
      const codeBlockCount = await codeBlocks.count();
      expect(codeBlockCount, `${name} MUST have code blocks but has ZERO!`).toBeGreaterThan(0);

      // Code blocks MUST have syntax highlighting tokens - NO EXCEPTIONS
      const tokens = page.locator('pre code .token');
      const tokenCount = await tokens.count();
      expect(tokenCount, `${name} has ${codeBlockCount} code blocks but ZERO syntax tokens! BROKEN!`).toBeGreaterThanOrEqual(minTokens);
    });
  }
});

test.describe('Syntax Highlighting - Token Types MUST Exist', () => {
  test('Homepage MUST have keyword tokens', async ({ page }) => {
    await page.goto('/');
    const keywords = page.locator('.token.keyword');
    const count = await keywords.count();
    expect(count, 'Homepage MUST have keyword tokens for C# code - BROKEN!').toBeGreaterThan(0);
  });

  test('Homepage MUST have string tokens', async ({ page }) => {
    await page.goto('/');
    const strings = page.locator('.token.string');
    const count = await strings.count();
    expect(count, 'Homepage MUST have string tokens for C# code - BROKEN!').toBeGreaterThan(0);
  });

  test('Examples page MUST have keyword tokens', async ({ page }) => {
    await page.goto('/examples/');
    const keywords = await page.locator('.token.keyword').count();
    expect(keywords, 'Examples MUST have keyword tokens - BROKEN!').toBeGreaterThan(10);
  });

  test('Examples page MUST have string tokens', async ({ page }) => {
    await page.goto('/examples/');
    const strings = await page.locator('.token.string').count();
    expect(strings, 'Examples MUST have string tokens - BROKEN!').toBeGreaterThan(10);
  });

  test('Examples page MUST have punctuation tokens', async ({ page }) => {
    await page.goto('/examples/');
    const punctuation = await page.locator('.token.punctuation').count();
    expect(punctuation, 'Examples MUST have punctuation tokens - BROKEN!').toBeGreaterThan(50);
  });

  test('Chinese Examples MUST have keyword tokens', async ({ page }) => {
    await page.goto('/zh/examples/');
    const keywords = await page.locator('.token.keyword').count();
    expect(keywords, 'Chinese Examples MUST have keyword tokens - BROKEN!').toBeGreaterThan(10);
  });

  test('Chinese Examples MUST have string tokens', async ({ page }) => {
    await page.goto('/zh/examples/');
    const strings = await page.locator('.token.string').count();
    expect(strings, 'Chinese Examples MUST have string tokens - BROKEN!').toBeGreaterThan(10);
  });
});

// API method detail pages - these have actual code examples
test.describe('Syntax Highlighting - API Method Detail Pages', () => {
  const apiMethodPages = [
    { url: '/api/getasync/', name: 'GetAsync' },
    { url: '/api/postasync/', name: 'PostAsync' },
    { url: '/api/putasync/', name: 'PutAsync' },
    { url: '/api/deleteasync/', name: 'DeleteAsync' },
    { url: '/api/patchasync/', name: 'PatchAsync' },
  ];

  for (const { url, name } of apiMethodPages) {
    test(`${name} method page MUST have syntax highlighting`, async ({ page }) => {
      await page.goto(url);
      const codeBlocks = await page.locator('pre code').count();
      expect(codeBlocks, `${name} MUST have code blocks - BROKEN!`).toBeGreaterThan(0);
      const tokens = await page.locator('pre code .token').count();
      expect(tokens, `${name} MUST have syntax tokens - BROKEN!`).toBeGreaterThan(0);
    });
  }
});

test.describe('Syntax Highlighting - Visual MUST Be Correct', () => {
  test('tokens MUST have color applied (not plain black text)', async ({ page }) => {
    await page.goto('/examples/');

    const keyword = page.locator('.token.keyword').first();
    await expect(keyword, 'No keyword token found - BROKEN!').toBeVisible();

    const color = await keyword.evaluate(el => getComputedStyle(el).color);
    expect(color, 'Keyword token has no color - CSS BROKEN!').not.toBe('rgb(0, 0, 0)');
  });

  test('Homepage code blocks MUST have language class', async ({ page }) => {
    await page.goto('/');

    const codeWithLang = page.locator('pre code[class*="language-"]');
    const count = await codeWithLang.count();
    expect(count, 'Homepage code blocks MUST have language-* class - BROKEN!').toBeGreaterThan(0);
  });

  test('Examples code blocks MUST have language class', async ({ page }) => {
    await page.goto('/examples/');

    const codeWithLang = page.locator('pre code[class*="language-"]');
    const count = await codeWithLang.count();
    expect(count, 'Examples code blocks MUST have language-* class - BROKEN!').toBeGreaterThan(0);
  });
});
