import { readFileSync } from 'node:fs';
export default {
  source: readFileSync(new URL('../../examples/Program.cs', import.meta.url), 'utf8'),
};
