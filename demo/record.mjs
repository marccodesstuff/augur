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
  const code = 'SOC2-CC8.1-E2E';
  await page.fill("[data-testid='input-code']", code);
  await page.fill("[data-testid='input-title']", 'E2E Demonstration Control');
  await page.fill("[data-testid='input-owner']", 'QA');
  await sleep(200);
  await shot(page, '03-create-form');
  await page.click("[data-testid='btn-create-control']");
  await page.waitForSelector(`[data-testid='control-row-${code}']`, { timeout: 10000 });
  await sleep(300);
  await shot(page, '04-control-created');

  // 4. Evidence upload
  await page.goto(`${BASE}/evidence`);
  await page.waitForSelector("[data-testid='evidence-controls']");
  await page.locator("[data-testid^='btn-upload-']").first().click();
  await page.waitForSelector("[data-testid='evidence-modal']");
  await page.fill("[data-testid='input-filename']", 'access-policy.pdf');
  await page.fill("[data-testid='input-uploader']", 'qa-bot');
  await sleep(200);
  await shot(page, '05-evidence-modal');
  await page.click("[data-testid='btn-submit-evidence']");
  await page.waitForSelector("[data-testid='evidence-modal']", { state: 'hidden', timeout: 10000 });
  await sleep(300);
  await shot(page, '06-evidence-attached');

  // 5. Agent run
  await page.goto(`${BASE}/agent`);
  await page.waitForSelector("[data-testid='btn-run-agent']");
  await page.fill("[data-testid='input-source']", 'github-dependabot');
  await sleep(200);
  await shot(page, '07-agent-form');
  await page.click("[data-testid='btn-run-agent']");
  await page.waitForSelector("[data-testid='agent-result']", { timeout: 15000 });
  await sleep(400);
  await shot(page, '08-agent-result');

  // 6. Swagger / OpenAPI 3.1.1
  await page.goto(`${API}/swagger`);
  await sleep(600);
  await shot(page, '09-swagger');

  await browser.close();
  console.log('DONE');
})().catch(e => { console.error(e); process.exit(1); });
