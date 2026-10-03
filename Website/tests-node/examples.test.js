import assert from 'node:assert/strict';
import { execFileSync } from 'node:child_process';
import { readFileSync } from 'node:fs';
import path from 'node:path';
import { fileURLToPath } from 'node:url';
import test from 'node:test';
import examples from '../src/_data/examples.js';

const website = fileURLToPath(new URL('../', import.meta.url));
test('both translated pages render the tested C# source without independent copies', () => {
  assert.equal(examples.source, readFileSync(path.join(website, 'examples/Program.cs'), 'utf8'));
  for (const page of ['src/examples/index.njk', 'src/zh/examples/index.njk']) {
    const source = readFileSync(path.join(website, page), 'utf8');
    assert.match(source, /examples\.source/);
    assert.match(source, /class="prose/);
    assert.doesNotMatch(source, /serializeRequest:|content\.ReadFromJsonAsync|=>\s*\{/);
  }
});

test('documented example compiles against the published package and handles real request interactions', { timeout: 180_000 }, () => {
  const result = execFileSync('dotnet', ['run', '--project', path.join(website, 'examples/Tests/Examples.Tests.csproj'), '--configuration', 'Release', '--no-launch-profile'], {
    cwd: website, encoding: 'utf8', timeout: 150_000, maxBuffer: 4 * 1024 * 1024,
    env: { ...process.env, DOTNET_PROCESSOR_COUNT: '2', DOTNET_GCHeapHardLimit: '0x40000000',
      MSBUILDDISABLENODEREUSE: '1', UseSharedCompilation: 'false', DOTNET_CLI_TELEMETRY_OPTOUT: '1' },
  });
  assert.match(result, /PASS: 28 example assertions; no network requests\./);
  assert.doesNotMatch(result, /warning |error /i);
});
