import { test, expect, expectAnalytics, analyticsScriptUrl } from './fixtures.js';

for (const [language, routes] of [
  ['English', ['/', '/docs/basic-usage/', '/blog/introducing-restclient/', '/api/reference/restclient-net-httpclientextensions/', '/examples/']],
  ['Chinese', ['/zh/', '/zh/docs/basic-usage/', '/zh/blog/introducing-restclient/', '/zh/api/httpclient-extensions/', '/zh/examples/']],
]) {
  test(`${language} page families queue the provided Google Analytics configuration once`, async ({ page }) => {
    const requests = [];
    page.on('request', request => { if (request.url() === analyticsScriptUrl) requests.push(request.url()); });
    for (const route of routes) {
      const response = await page.goto(route);
      expect(response?.status()).toBe(200);
      await expect(page.locator('html')).toHaveAttribute('lang', language === 'English' ? 'en' : 'zh');
      await expectAnalytics(page);
    }
    expect(requests).toHaveLength(routes.length);
  });
}

test('link navigation and reload start one fresh analytics queue per document', async ({ page }) => {
  await page.goto('/');
  await expectAnalytics(page);
  await page.evaluate(() => { window.__analyticsTestDocument = 'initial'; });

  await page.getByRole('navigation', { name: 'Main navigation', exact: true }).getByRole('link', { name: 'Docs', exact: true }).click();
  await expect(page).toHaveURL(/\/docs\/$/);
  expect(await page.evaluate(() => window.__analyticsTestDocument)).toBeUndefined();
  await expectAnalytics(page);
  await page.evaluate(() => { window.__analyticsTestDocument = 'before-reload'; });

  const response = await page.reload();
  expect(response?.status()).toBe(200);
  expect(await page.evaluate(() => window.__analyticsTestDocument)).toBeUndefined();
  await expectAnalytics(page);

  await page.getByRole('navigation', { name: 'Main navigation', exact: true }).getByRole('link', { name: 'API', exact: true }).click();
  await expect(page).toHaveURL(/\/api\/$/);
  await expectAnalytics(page);
});
