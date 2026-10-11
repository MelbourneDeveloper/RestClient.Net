import { test, expect } from './fixtures.js';

const routes = [['/', 'Home'], ['/docs/', 'Docs'], ['/api/', 'API'], ['/blog/', 'Blog'], ['/examples/', 'Examples']];

test('homepage hero has clear copy and working actions', async ({ page }) => {
  await page.goto('/');
  await expect(page.locator('.hero')).toBeVisible();
  await expect(page.locator('.hero h1')).toHaveText(/Every outcome.*In view/s);
  await expect(page.locator('.hero-tagline')).toContainText('HTTP');
  await expect(page.locator('.hero-actions a')).toHaveCount(2);
  await page.locator('.hero-actions a').first().click();
  await expect(page).toHaveURL(/\/docs\/$/);
  await expect(page.locator('.prose')).toBeVisible();
  await expect(page.locator('h1')).toHaveCount(1);
});

test('feature cards expose explanations and destination links', async ({ page }) => {
  await page.goto('/');
  const cards = page.locator('.feature-card');
  expect(await cards.count()).toBeGreaterThanOrEqual(6);
  for (const card of await cards.all()) {
    await expect(card).toBeVisible();
    await expect(card.locator('h3')).not.toBeEmpty();
    await expect(card).toHaveAttribute('href', /\//);
  }
});

test('request explorer explains every outcome with native graphics', async ({ page }) => {
  await page.goto('/');
  await expect(page.locator('#flow-canvas')).toBeVisible();
  await expect(page.locator('button[data-outcome]')).toHaveCount(3);
  await expect(page.locator('#outcome-status')).toHaveAttribute('aria-live', 'polite');
  await expect(page.locator('#outcome-code')).toContainText('switch');
  await expect(page.locator('video,iframe')).toHaveCount(0);
  await expect(page.locator('#motion-toggle')).toBeVisible();
});

test('navigation supports a multi-page reading journey', async ({ page }) => {
  await page.goto('/');
  for (const [route, label] of routes) {
    await page.locator('#mobile-nav nav').locator(`a[href="${route}"]`).click();
    expect(new URL(page.url()).pathname).toBe(route);
    await expect(page.locator('h1')).toBeVisible();
    const active = page.locator('#mobile-nav [aria-current="page"]');
    await expect(active).toHaveCount(1);
    await expect(active).toHaveText(label);
    await expect(active).toHaveAttribute('href', route);
  }
});

test('active navigation stays exclusive on child pages too', async ({ page }) => {
  for (const [route, label] of [...routes, ['/docs/basic-usage/', 'Docs']]) {
    await page.goto(route);
    const active = page.locator('#mobile-nav .nav-link.active');
    await expect(active).toHaveCount(1);
    await expect(active).toHaveText(label);
    await expect(active).toHaveAttribute('aria-current', 'page');
  }
});

test('docs sidebar opens a guide and returns to the overview', async ({ page }) => {
  await page.goto('/docs/');
  const sidebar = page.locator('.docs-sidebar');
  await expect(sidebar).toBeVisible();
  await sidebar.locator('a[href="/docs/basic-usage/"]').click();
  await expect(page.locator('.prose h1')).toContainText('Basic');
  await expect(sidebar.locator('[aria-current="page"]')).toHaveAttribute('href', '/docs/basic-usage/');
  await sidebar.locator('a[href="/docs/"]').click();
  await expect(page).toHaveURL(/\/docs\/$/);
  await expect(page.locator('.prose pre code').first()).toBeVisible();
});

test('code stays selectable highlighted text with distinct token colors', async ({ page }) => {
  await page.goto('/');
  const code = page.locator('pre code').first();
  await expect(code).toBeVisible();
  await expect(code).toHaveClass(/language-csharp/);
  expect(await code.locator('.token').count()).toBeGreaterThan(0);
  const colors = await code.evaluate(element => [getComputedStyle(element.querySelector('.keyword')).color, getComputedStyle(element.querySelector('.string')).color]);
  expect(colors[0]).not.toBe(colors[1]);
  expect(await code.textContent()).toContain('result');
});

test('footer links to guides, source, packages, and generated reference', async ({ page }) => {
  await page.goto('/');
  const footer = page.locator('footer');
  await expect(footer).toBeVisible();
  expect(await footer.locator('a').count()).toBeGreaterThan(5);
  await expect(footer.locator('a[href*="nuget.org"]')).toBeVisible();
  await expect(footer.locator('a[href*="github.com"]')).toBeVisible();
  await footer.locator('a[href="/api/reference/"]').click();
  await expect(page).toHaveURL(/\/api\/reference\/$/);
  await expect(page.locator('.prose')).toBeVisible();
});

test('motion control works with keyboard and creates no inline CSS', async ({ page }) => {
  await page.goto('/');
  const toggle = page.locator('#motion-toggle');
  await expect(toggle).toBeVisible();
  await expect(toggle).toHaveAccessibleName(/motion/i);
  await toggle.focus();
  await page.keyboard.press('Enter');
  await expect(toggle).toHaveAttribute('aria-pressed', 'true');
  await expect(page.locator('#flow-canvas')).toHaveAttribute('data-motion', 'paused');
  await page.keyboard.press('Enter');
  await expect(toggle).toHaveAttribute('aria-pressed', 'false');
  await expect(page.locator('#flow-canvas')).toHaveAttribute('data-motion', 'running');
  await expect(page.locator('style,[style]')).toHaveCount(0);
});

test('language selector visits translated document and its original', async ({ page }) => {
  await page.goto('/docs/basic-usage/');
  await page.locator('.language-switcher summary').click();
  await expect(page.locator('.language-switcher')).toHaveAttribute('open', '');
  await page.locator('.language-switcher a[lang="zh"]').click();
  await expect(page).toHaveURL(/\/zh\/docs\/basic-usage\/$/);
  await expect(page.locator('html')).toHaveAttribute('lang', 'zh');
  await page.locator('.language-switcher summary').click();
  await page.locator('.language-switcher a[lang="en"]').click();
  await expect(page).toHaveURL(/\/docs\/basic-usage\/$/);
  await expect(page.locator('html')).toHaveAttribute('lang', 'en');
});

test('Chinese homepage keeps localized navigation and outcome controls', async ({ page }) => {
  await page.goto('/zh/');
  await expect(page.locator('html')).toHaveAttribute('lang', 'zh');
  await expect(page.locator('h1')).toContainText('每种结果');
  await expect(page.locator('#mobile-nav [aria-current="page"]')).toHaveText('首页');
  await expect(page.locator('button[data-outcome="success"]')).toContainText('成功');
  await expect(page.locator('.hero-actions a').first()).toHaveAttribute('href', '/zh/docs/');
});

test('mobile navigation opens and closes using keyboard and pointer', async ({ page }) => {
  await page.setViewportSize({ width: 375, height: 667 });
  await page.goto('/');
  const menu = page.locator('#mobile-nav');
  const summary = menu.locator('summary');
  await expect(summary).toBeVisible();
  for (const control of [summary, page.locator('.language-switcher summary')]) {
    const bounds = await control.boundingBox();
    expect(bounds.height, 'Mobile controls must provide a usable touch target').toBeGreaterThanOrEqual(44);
    expect(bounds.width).toBeGreaterThanOrEqual(44);
  }
  if (await menu.getAttribute('open') !== null) await summary.click();
  await expect(menu).not.toHaveAttribute('open');
  await expect(menu.locator('nav')).toBeHidden();
  await summary.focus();
  await page.keyboard.press('Enter');
  await expect(menu).toHaveAttribute('open', '');
  await expect(menu.locator('nav')).toBeVisible();
  await menu.locator('a[href="/docs/"]').click();
  await expect(page).toHaveURL(/\/docs\/$/);
  await expect(page.locator('.prose')).toBeVisible();
  expect(await page.evaluate(() => document.documentElement.scrollWidth <= innerWidth)).toBe(true);
});

test('blog articles contain dates, headings, examples, and a return link', async ({ page }) => {
  await page.goto('/blog/');
  const article = page.locator('article h2 a[href="/blog/introducing-restclient/"]');
  await expect(article).toBeVisible();
  await article.click();
  await expect(page.locator('article.prose h1')).toContainText('RestClient.Net');
  await expect(page.locator('article.prose time')).toHaveAttribute('datetime', /^\d{4}-\d{2}-\d{2}/);
  await expect(page.locator('article.prose pre code').first()).toBeVisible();
  await expect(page.locator('article.prose a[href="/blog/"]')).toBeVisible();
});

test('API guide and generated reference contain useful source documentation', async ({ page }) => {
  await page.goto('/api/httpclient-extensions/');
  await expect(page.locator('h1')).toBeVisible();
  await expect(page.locator('main')).toContainText('GetAsync');
  await page.goto('/api/reference/');
  await expect(page.locator('h1')).toContainText('API');
  expect(await page.locator('.prose a').count()).toBeGreaterThan(5);
});

test('examples remain readable without horizontal page overflow on mobile', async ({ page }) => {
  await page.setViewportSize({ width: 375, height: 667 });
  await page.goto('/examples/');
  await expect(page.locator('h1')).toBeVisible();
  expect(await page.locator('pre code').count()).toBeGreaterThan(0);
  await expect(page.locator('pre code').first()).toBeVisible();
  const listing = page.locator('pre code.language-csharp');
  await expect(listing).toHaveCount(1);
  for (const symbol of ['RestClientExamples', 'GetPostAsync', 'CreatePostAsync', 'GetUsingFactoryAsync', '.Match(', 'CancellationToken']) {
    await expect(listing).toContainText(symbol);
  }
  await expect(page.locator('pre code.language-bash')).toHaveText('dotnet run --project Website/examples/Examples.csproj');
  await expect(page.locator('.prose')).not.toContainText('{{ examples.source');
  for (const anchor of ['basic-get', 'post-request', 'ihttpclientfactory', 'status-code-handling']) {
    await expect(page.locator(`#${anchor}`)).toHaveCount(1);
  }
  expect(await page.evaluate(() => document.documentElement.scrollWidth <= innerWidth)).toBe(true);
});
