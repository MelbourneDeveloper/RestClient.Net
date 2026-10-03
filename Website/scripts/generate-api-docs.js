#!/usr/bin/env node
/** Export actual public C# declarations and XML docs; curated guides stay hand-written. */
import { execFileSync } from 'node:child_process';
import { copyFileSync, mkdirSync } from 'node:fs';
import path from 'node:path';
import { fileURLToPath } from 'node:url';

const filename = fileURLToPath(import.meta.url);
const website = path.resolve(path.dirname(filename), '..');
export const toolProject = path.join(website, 'tools/ApiDocs/ApiDocs.csproj');

export function generateApi({ root = path.dirname(website), output = path.join(website, 'src/api/reference'), sourceRef = 'main', projects = [] } = {}) {
  const argumentsList = ['run', '--project', toolProject, '--configuration', 'Release', '--no-launch-profile', '--',
    '--root', path.resolve(root), '--output', path.resolve(output), '--source-ref', sourceRef];
  if (projects.length) argumentsList.push('--projects', projects.join(','));
  const result = execFileSync('dotnet', argumentsList, {
    cwd: website,
    encoding: 'utf8',
    timeout: 180_000,
    maxBuffer: 4 * 1024 * 1024,
    env: { ...process.env, DOTNET_PROCESSOR_COUNT: '2', DOTNET_GCHeapHardLimit: '0x40000000',
      DOTNET_CLI_TELEMETRY_OPTOUT: '1', MSBUILDDISABLENODEREUSE: '1', UseSharedCompilation: 'false' },
  });
  mkdirSync(output, { recursive: true });
  copyFileSync(path.join(website, 'tools/ApiDocs/schema.json'), path.join(output, 'schema.json'));
  return result;
}

if (process.argv[1] && path.resolve(process.argv[1]) === filename) {
  const options = {};
  for (let index = 2; index < process.argv.length; index += 2) {
    const flag = process.argv[index];
    const value = process.argv[index + 1];
    if (!value || !['--root', '--output', '--source-ref', '--projects'].includes(flag)) {
      throw new Error('Use [--root PATH] [--output PATH] [--source-ref REF] [--projects dir,dir]');
    }
    options[{ '--root': 'root', '--output': 'output', '--source-ref': 'sourceRef', '--projects': 'projects' }[flag]] = flag === '--projects' ? value.split(',') : value;
  }
  process.stdout.write(generateApi(options));
}
