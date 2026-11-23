import { defineConfig } from '@playwright/test';

const baseURL = process.env.WEBSITE_TEST_BASE_URL || 'http://127.0.0.1:4173';
const port = new URL(baseURL).port || '4173';

export default defineConfig({
  testDir: './tests',
  fullyParallel: true,
  forbidOnly: !!process.env.CI,
  retries: process.env.CI ? 2 : 0,
  workers: process.env.CI ? 2 : 4,
  reporter: 'list',
  use: {
    baseURL,
    trace: 'on-first-retry',
  },
  webServer: {
    command: `python3 -m http.server ${port} --bind 127.0.0.1 --directory _site`,
    url: baseURL,
    reuseExistingServer: false,
    timeout: 120000,
  },
});
