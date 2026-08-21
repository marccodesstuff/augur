import { chromium } from 'playwright';

const BASE = 'http://localhost:5000';
const API = 'http://localhost:5050';

async function shot(page, name) {
  await page.screenshot({ path: `shots/${name}.png` });
  console.log('captured', name);
}

const sleep = (ms) => new Promise(r => setTimeout(r, ms));

(async () => {
  const browser = await chromium.launch();
  const page = await browser.newPage({ viewport: { width: 1280, height: 800 } });

  // 1. Dashboard
  await page.goto(`${BASE}/`);
  await page.waitForSelector("[data-testid='dashboard-header']");
  await sleep(400);
  await shot(page, '01-dashboard');

  // 2. Controls list
  await page.goto(`${BASE}/controls`);
  await page.waitForSelector("[data-testid='btn-create-control']");
  await sleep(300);
  await shot(page, '02-controls');

  // 3. Create a control
    const code = 'SOC2-E2E-001';
    await page.fill("[data-testid='input-code']", code);
    await page.fill("[data-testid='input-title']", 'E2E Demonstration Control');
    await page.fill("[data-testid='input-owner']", 'QA');
    await page.fill("[data-testid='input-description']", 'Created during demo recording');
    await sleep(200);
    await shot(page, '03-create-form');
    await page.click("[data-testid='btn-create-control']");
    // Wait for the new control row to appear (deterministic, no fixed sleep)
    await page.waitForSelector(`[data-testid='control-row-${code}']`, { timeout: 10000 });
    await shot(page, '04-control-created');

  // 4. Evidence page (show existing evidence UI)
  await page.goto(`${BASE}/evidence`);
  await page.waitForLoadState('networkidle');
  await sleep(500);
  await shot(page, '05-evidence-page');

  // 5. Swagger / OpenAPI 3.1.1
  await page.goto(`${API}/swagger`);
  await sleep(1000);
  await shot(page, '06-swagger');

  await browser.close();
  console.log('DONE');
})().catch(e => { console.error(e); process.exit(1); });