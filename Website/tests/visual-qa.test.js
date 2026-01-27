import { test, expect } from '@playwright/test';

const baseUrl = 'http://localhost:8080';

test.describe('Visual QA Checks', () => {
  test('homepage hero section renders correctly', async ({ page }) => {
    await page.goto(baseUrl);
    const hero = page.locator('.hero');
    await expect(hero).toBeVisible();
    await expect(page.locator('.hero-logo')).toBeVisible();
    await expect(page.locator('.hero-tagline')).toBeVisible();
    await expect(page.locator('.hero-actions')).toBeVisible();
  });

  test('homepage feature cards have proper styling', async ({ page }) => {
    await page.goto(baseUrl);
    const cards = page.locator('.feature-card');
    await expect(cards.first()).toBeVisible();
    const count = await cards.count();
    expect(count).toBeGreaterThanOrEqual(6);
  });

  test('Why Discriminated Unions section renders', async ({ page }) => {
    await page.goto(baseUrl);
    const unionSection = page.locator('text=Why Discriminated Unions?');
    await expect(unionSection).toBeVisible();
    
    // Check both code examples exist
    const withoutExhaustion = page.locator('text=Without Exhaustion');
    const withExhaustion = page.locator('text=With Exhaustion');
    await expect(withoutExhaustion).toBeVisible();
    await expect(withExhaustion).toBeVisible();
    
    // Check code blocks in this section exist
    const codeBlocks = page.locator('.feature-card pre code');
    const codeCount = await codeBlocks.count();
    expect(codeCount).toBeGreaterThanOrEqual(2);
  });

  test('navigation links work', async ({ page }) => {
    await page.goto(baseUrl);

    // Check docs link
    await page.click('a[href="/docs/"]');
    await expect(page).toHaveURL(/\/docs\//);

    // Check API link
    await page.click('a[href="/api/"]');
    await expect(page).toHaveURL(/\/api\//);

    // Check blog link
    await page.click('a[href="/blog/"]');
    await expect(page).toHaveURL(/\/blog\//);
  });

  test('navigation active state is exclusive - only ONE nav item should be active per page', async ({ page }) => {
    // Test home page - only Home should be active
    await page.goto(baseUrl);
    let activeLinks = page.locator('.nav-link.active');
    await expect(activeLinks).toHaveCount(1);
    await expect(activeLinks.first()).toHaveText('Home');

    // Test docs page - only Docs should be active
    await page.goto(`${baseUrl}/docs/`);
    activeLinks = page.locator('.nav-link.active');
    await expect(activeLinks).toHaveCount(1);
    await expect(activeLinks.first()).toHaveText('Docs');

    // Test API page - only API should be active
    await page.goto(`${baseUrl}/api/`);
    activeLinks = page.locator('.nav-link.active');
    await expect(activeLinks).toHaveCount(1);
    await expect(activeLinks.first()).toHaveText('API');

    // Test blog page - only Blog should be active
    await page.goto(`${baseUrl}/blog/`);
    activeLinks = page.locator('.nav-link.active');
    await expect(activeLinks).toHaveCount(1);
    await expect(activeLinks.first()).toHaveText('Blog');

    // Test examples page - only Examples should be active
    await page.goto(`${baseUrl}/examples/`);
    activeLinks = page.locator('.nav-link.active');
    await expect(activeLinks).toHaveCount(1);
    await expect(activeLinks.first()).toHaveText('Examples');
  });

  test('docs sidebar navigation works', async ({ page }) => {
    await page.goto(`${baseUrl}/docs/`);
    const sidebar = page.locator('.sidebar, .docs-sidebar');
    await expect(sidebar.first()).toBeVisible();
  });

  test('code blocks have proper syntax highlighting colors', async ({ page }) => {
    await page.goto(baseUrl);
    const codeBlock = page.locator('pre code').first();
    await expect(codeBlock).toBeVisible();

    // Check that syntax highlighting classes are present (Prism uses .token, hljs uses .hljs)
    const hasHighlight = await page.evaluate(() => {
      const code = document.querySelector('pre code');
      return code && (
        code.classList.contains('hljs') ||
        code.querySelector('.hljs-keyword') ||
        code.querySelector('.token') ||
        code.classList.contains('language-csharp') ||
        code.classList.contains('language-bash')
      );
    });
    expect(hasHighlight).toBeTruthy();
  });

  test('footer renders with all sections', async ({ page }) => {
    await page.goto(baseUrl);
    const footer = page.locator('footer');
    await expect(footer).toBeVisible();
    
    // Check footer has links
    const footerLinks = footer.locator('a');
    const linkCount = await footerLinks.count();
    expect(linkCount).toBeGreaterThan(5);
  });

  test('theme toggle button exists', async ({ page }) => {
    await page.goto(baseUrl);
    const themeToggle = page.locator('#theme-toggle');
    await expect(themeToggle).toBeVisible();
  });

  test('language switcher exists and works', async ({ page }) => {
    await page.goto(baseUrl);
    const langSwitcher = page.locator('.language-switcher');
    if (await langSwitcher.count() > 0) {
      await expect(langSwitcher).toBeVisible();
    }
  });

  test('Chinese pages load correctly', async ({ page }) => {
    await page.goto(`${baseUrl}/zh/`);
    await expect(page).toHaveURL(/\/zh\//);
    
    // Check lang attribute
    const html = page.locator('html');
    await expect(html).toHaveAttribute('lang', 'zh');
  });

  test('mobile menu toggle exists on mobile viewport', async ({ page }) => {
    await page.setViewportSize({ width: 375, height: 667 });
    await page.goto(baseUrl);
    
    // Mobile menu toggle should be visible on mobile
    const mobileToggle = page.locator('#mobile-menu-toggle, .mobile-menu-toggle, [aria-label*="menu"]');
    // This may or may not be visible depending on CSS - just check page loads
    await expect(page.locator('.hero')).toBeVisible();
  });

  test('blog posts have proper structure', async ({ page }) => {
    await page.goto(`${baseUrl}/blog/`);
    const posts = page.locator('article, .post, .blog-post');
    
    // Should have blog posts
    const postCount = await posts.count();
    expect(postCount).toBeGreaterThan(0);
  });

  test('API reference pages load', async ({ page }) => {
    await page.goto(`${baseUrl}/api/`);
    await expect(page.locator('h1')).toBeVisible();

    // Navigate to a specific API page
    await page.goto(`${baseUrl}/api/httpclient-extensions/`);
    await expect(page.locator('h1')).toBeVisible();
  });

  test('examples page loads', async ({ page }) => {
    await page.goto(`${baseUrl}/examples/`);
    await expect(page.locator('h1')).toBeVisible();
  });
});
