# RestClient.Net website

The site uses Eleventy, local layouts, and one shared `.prose` class for guides,
API reference pages, and articles. Its interactive HTTP/Result diagram is drawn
in canvas; the code examples and outcome controls remain ordinary accessible HTML.

## Develop and verify

Install Node.js 22 and the .NET 9 SDK, then run these commands from `Website/`:

```sh
npm ci
npm run dev
```

The preview uses port 4173. It does not terminate other processes.

```sh
npm run test:unit
npm run build
npm test
```

The build exports API documentation, renders the site, and audits the result.
The browser tests require Chromium: `npx playwright install chromium` installs it.

English and Chinese example pages render `examples/Program.cs`. The unit checks
compile that source against RestClient.Net 7.3.1 and exercise requests and errors
through a local message handler, without making network requests.

The CSS budget is **2,500 raw UTF-8 bytes across all shipped CSS files**. The build
guard also rejects inline styles, external stylesheets, and JavaScript that injects
CSS. Canvas drawing is used for the actual motion graphic, not to replace content
layout or typography. Reduced-motion preferences and the animation pause control
are respected.

## API reference

`npm run generate-api` runs `scripts/generate-api-docs.js`, which invokes the
Roslyn exporter under `tools/ApiDocs/`. It reads public C# declarations and XML
comments from the library source and writes deterministic Markdown and JSON to
`src/api/reference/`. This generated directory is ignored by Git and recreated
on every build. Edit C# declarations and comments to update the reference; curated
API guides remain tracked under `src/api/`.

Exporter tests change fixture declarations and comments, exercise overloads, and
check that private implementation details stay out of the reference.

## Deployment URLs

Local previews use root-relative routes. Set `SITE_URL` to the production origin
and `SITE_PATH_PREFIX` to the mount path when deploying, for example:

```sh
SITE_URL=https://melbournedeveloper.github.io \
SITE_PATH_PREFIX=/RestClient.Net/ npm run build
```

The GitHub Pages workflow reads these values from `actions/configure-pages`.
It runs unit and browser tests before building and auditing the production URLs.
Pull requests run these checks; only pushes to `main` deploy the resulting site.
