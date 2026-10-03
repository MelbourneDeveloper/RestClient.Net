import assert from 'node:assert/strict';
import { mkdtempSync, mkdirSync, readFileSync, readdirSync, rmSync, writeFileSync } from 'node:fs';
import os from 'node:os';
import path from 'node:path';
import test from 'node:test';
import { generateApi } from '../scripts/generate-api-docs.js';

const fixture = `namespace Fixture;
/// <summary>A callable converter.</summary>
/// <param name="input">The value to convert.</param>
/// <returns>The converted value.</returns>
public delegate TResult Converter<in T, out TResult>(T input) where T : class;

/// <summary>A documented client.</summary>
/// <typeparam name="T">The item type.</typeparam>
public partial class Client<T> where T : class, new()
{
    /// <summary>Fetch the original item. See <see cref="System.String"/>.</summary>
    /// <param name="id">The numeric identifier.</param>
    /// <param name="label">An optional label.</param>
    /// <returns>The item text.</returns>
    /// <remarks>Templates such as <c>{{ value }}</c> remain literal.</remarks>
    public string Fetch(int id = 42, string? label = "two words") => id.ToString();
    /// <summary>Fetch an item by its string identifier.</summary>
    public string Fetch(string id) => id;
    /// <summary>A generic factory.</summary>
    public TResult Create<TResult>() where TResult : T, new() => new TResult();
    /// <summary>Readable count.</summary>
    public int Count { get; private set; }
    /// <summary>Constants retain their literal whitespace.</summary>
    public const string First = "one  two", Second = "second";
    private void Secret() { }
    internal void InternalOperation() { }
    public sealed class Nested { public int Value { get; init; } }
}
/// <summary>A payload with positional properties.</summary>
/// <param name="Name">The payload name.</param>
/// <param name="Count">The count.</param>
public record Payload(string Name, int Count = 2);
internal class Hidden { public class Leaked { public void NeverPublish() { } } }
`;

function snapshot(directory) {
  return Object.fromEntries(readdirSync(directory).sort().map(name => [name, readFileSync(path.join(directory, name), 'utf8')]));
}

test('source API export follows declarations and documentation through edits', { timeout: 180_000 }, async t => {
  const root = mkdtempSync(path.join(os.tmpdir(), 'restclient-api-export-'));
  t.after(() => rmSync(root, { recursive: true, force: true }));
  const sources = path.join(root, 'Fixture');
  const output = path.join(root, 'reference');
  mkdirSync(sources);
  writeFileSync(path.join(sources, 'Api.cs'), fixture);
  writeFileSync(path.join(sources, 'More.cs'), 'namespace Fixture; public partial class Client<T> { public bool Delete(int id) => true; }');
  const options = { root, output, projects: ['Fixture'], sourceRef: 'test-revision' };
  generateApi(options);
  const initial = snapshot(output);
  const index = JSON.parse(initial['api.json']);
  const client = index.types.find(type => type.name === 'Client');
  const fetches = client.members.filter(member => member.name === 'Fetch');

  await t.test('public surface, overload identities, signatures and source links are accurate', () => {
    assert.equal(index.schemaVersion, 1);
    assert.equal(index.sourceRef, 'test-revision');
    assert.equal(index.types.filter(type => type.name === 'Client').length, 1, 'partial declarations merge');
    assert.deepEqual(index.types.map(type => type.name).sort(), ['Client', 'Converter', 'Nested', 'Payload']);
    assert.equal(fetches.length, 2);
    assert.equal(new Set(fetches.map(member => member.id)).size, 2);
    assert.equal(new Set(fetches.map(member => member.anchor)).size, 2);
    assert.ok(client.members.some(member => member.name === 'Delete'), 'the second partial contributes members');
    assert.ok(!client.members.some(member => ['Secret', 'InternalOperation'].includes(member.name)));
    assert.match(client.signature, /where T : class, new\(\)/);
    assert.match(client.members.find(member => member.name === 'Create').signature, /where TResult : T, new\(\)/);
    assert.match(client.members.find(member => member.name === 'Count').signature, /private set;/);
    assert.match(client.members.find(member => member.name === 'First').signature, /First = "one  two"/);
    assert.match(client.members.find(member => member.name === 'Second').signature, /Second = "second"/);
    assert.doesNotMatch(client.members.find(member => member.name === 'Second').signature, /First =/);
    const numeric = fetches.find(member => member.parameters[0].type === 'int');
    assert.equal(numeric.parameters[0].default, '42');
    assert.equal(numeric.parameters[1].type, 'string?');
    assert.equal(numeric.parameters[1].default, '"two words"');
    assert.equal(numeric.source.file, 'Fixture/Api.cs');
    assert.equal(numeric.source.line, fixture.split('\n').findIndex(line => line.includes('public string Fetch(int')) + 1);
    assert.match(numeric.source.url, /\/blob\/test-revision\/Fixture\/Api.cs#L\d+$/);
    const delegate = index.types.find(type => type.kind === 'delegate');
    assert.match(delegate.signature, /delegate TResult Converter<in T, out TResult>\(T input\)/);
    assert.match(delegate.signature, /where T : class/);
    assert.equal(delegate.parameters[0].type, 'T');
    const record = index.types.find(type => type.name === 'Payload');
    assert.deepEqual(record.parameters.map(parameter => parameter.name), ['Name', 'Count']);
    assert.equal(record.parameters[1].default, '2');
    assert.deepEqual(record.members.filter(member => ['Name', 'Count'].includes(member.name)).map(member => member.name).sort(), ['Count', 'Name']);
  });

  await t.test('XML prose and Markdown use the shared layout without template evaluation', () => {
    const numeric = fetches.find(member => member.parameters[0].type === 'int');
    assert.match(numeric.docs.summary, /Fetch the original item/);
    assert.match(numeric.docs.summary, /`System.String`/);
    assert.equal(numeric.docs.parameters[0].description, 'The numeric identifier.');
    assert.equal(numeric.docs.returns, 'The item text.');
    assert.match(numeric.docs.remarks, /`{{ value }}`/);
    const markdown = initial['fixture-client-1.md'];
    assert.match(markdown, /layout: layouts\/api.njk/);
    assert.match(markdown, /templateEngineOverride: md/);
    assert.doesNotMatch(markdown, /^# /m);
    for (const overload of fetches) assert.ok(markdown.includes(`id="${overload.anchor}"`));
    assert.match(markdown, /\*\*Returns:\*\* The item text/);
    assert.match(markdown, /\*\*Remarks:\*\* Templates/);
    assert.match(markdown, /\| `id` \| `int` \| `42` \| The numeric identifier/);
    assert.ok(initial['schema.json']);
    assert.doesNotMatch(JSON.stringify(index), new RegExp(root.replace(/[.*+?^${}()|[\]\\]/g, '\\$&')));
  });

  await t.test('identical input exports byte-identical JSON and Markdown', () => {
    generateApi(options);
    assert.deepEqual(snapshot(output), initial);
  });

  await t.test('editing real source changes API signatures and prose while stable overloads retain anchors', () => {
    writeFileSync(path.join(sources, 'Api.cs'), fixture.replace('Fetch the original item.', 'Fetch the revised item.')
      .replace('public string Fetch(int id = 42', 'public string Fetch(long id = 42'));
    generateApi(options);
    const edited = snapshot(output);
    const changed = JSON.parse(edited['api.json']).types.find(type => type.name === 'Client');
    const numeric = changed.members.find(member => member.name === 'Fetch' && member.parameters[0].type === 'long');
    assert.ok(numeric);
    assert.match(numeric.signature, /Fetch\(long id = 42/);
    assert.match(numeric.docs.summary, /Fetch the revised item/);
    assert.notEqual(numeric.anchor, fetches.find(member => member.parameters[0].type === 'int').anchor);
    assert.equal(changed.members.find(member => member.name === 'Fetch' && member.parameters[0].type === 'string').anchor,
      fetches.find(member => member.parameters[0].type === 'string').anchor);
    assert.match(edited['fixture-client-1.md'], /Fetch the revised item/);
    assert.doesNotMatch(edited['fixture-client-1.md'], /Fetch the original item/);
    assert.notEqual(edited['api.json'], initial['api.json']);
  });
});

test('symbol IDs resolve SDK implicit usings and distinguish nullable references from values', { timeout: 180_000 }, t => {
  const root = mkdtempSync(path.join(os.tmpdir(), 'restclient-api-symbols-'));
  t.after(() => rmSync(root, { recursive: true, force: true }));
  mkdirSync(path.join(root, 'Fixture'));
  writeFileSync(path.join(root, 'Fixture', 'Transport.cs'), `namespace Fixture;
public static class Transport
{
    public static void Send(HttpClient client, HttpContent? content,
        Func<HttpResponseMessage, string?> callback, Action<long, long>? progress,
        IReadOnlyDictionary<string, string>? headers, CancellationToken token, int? retries) { }
}`);
  const output = path.join(root, 'reference');
  generateApi({ root, output, projects: ['Fixture'] });
  const index = JSON.parse(readFileSync(path.join(output, 'api.json'), 'utf8'));
  const send = index.types.find(type => type.name === 'Transport').members.find(member => member.name === 'Send');
  assert.equal(send.id, 'M:Fixture.Transport.Send(System.Net.Http.HttpClient,System.Net.Http.HttpContent,System.Func{System.Net.Http.HttpResponseMessage,System.String},System.Action{System.Int64,System.Int64},System.Collections.Generic.IReadOnlyDictionary{System.String,System.String},System.Threading.CancellationToken,System.Nullable{System.Int32})');
  assert.deepEqual(send.parameters.map(parameter => parameter.type), [
    'HttpClient', 'HttpContent?', 'Func<HttpResponseMessage, string?>', 'Action<long, long>?',
    'IReadOnlyDictionary<string, string>?', 'CancellationToken', 'int?',
  ], 'source spelling and nullable annotations remain intact');
  assert.doesNotMatch(send.id, /Nullable\{(?:HttpContent|Action|IReadOnlyDictionary)/);
});

test('symbol IDs resolve third-party types used in repository public signatures', { timeout: 180_000 }, t => {
  const root = mkdtempSync(path.join(os.tmpdir(), 'restclient-api-references-'));
  t.after(() => rmSync(root, { recursive: true, force: true }));
  mkdirSync(path.join(root, 'Fixture'));
  writeFileSync(path.join(root, 'Fixture', 'External.cs'), `using Microsoft.OpenApi;
using Urls;
namespace Fixture;
public static class External
{
    public static void Inspect(AbsoluteUrl url, IHttpClientFactory clients, IOpenApiSchema? schema) { }
}`);
  const output = path.join(root, 'reference');
  generateApi({ root, output, projects: ['Fixture'] });
  const index = JSON.parse(readFileSync(path.join(output, 'api.json'), 'utf8'));
  const inspect = index.types.find(type => type.name === 'External').members.find(member => member.name === 'Inspect');
  assert.equal(inspect.id, 'M:Fixture.External.Inspect(Urls.AbsoluteUrl,System.Net.Http.IHttpClientFactory,Microsoft.OpenApi.IOpenApiSchema)');
  assert.equal(inspect.parameters[2].type, 'IOpenApiSchema?');
  assert.doesNotMatch(inspect.id, /Nullable\{IOpenApiSchema/);
});
