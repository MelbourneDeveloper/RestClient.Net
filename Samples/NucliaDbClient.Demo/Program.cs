using Microsoft.Extensions.DependencyInjection;
using NucliaDB.Generated;
using Outcome;

// Setup HTTP client factory
var services = new ServiceCollection();
services.AddHttpClient("default", client =>
{
    client.BaseAddress = new Uri("http://localhost:8080/api/v1");
    client.Timeout = TimeSpan.FromSeconds(30);
});

var serviceProvider = services.BuildServiceProvider();
var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
var httpClient = httpClientFactory.CreateClient("default");

Console.WriteLine("NucliaDB Demo - Creating and retrieving a Knowledge Box\n");

// Create a knowledge box
var kbSlug = $"test-kb-{DateTime.UtcNow:yyyyMMddHHmmss}";
var createPayload = new
{
    slug = kbSlug,
    title = "Test Knowledge Box",
    description = "A test KB created via RestClient.Net",
};

Console.WriteLine($"Creating knowledge box with slug: {kbSlug}");
var createResult = await httpClient.CreateKnowledgeBoxKbsAsync(createPayload).ConfigureAwait(false);

var kbId = createResult switch
{
    OkKnowledgeBoxObj ok =>
        $"Created successfully! UUID: {ok.Value.Uuid}",
    ErrorKnowledgeBoxObj error => error.Value switch
    {
        HttpError<string>.ErrorResponseError err =>
            $"Error {err.StatusCode}: {err.Body}",
        HttpError<string>.ExceptionError err =>
            $"Exception: {err.Exception.Message}",
        _ => "Unknown error",
    },
};

Console.WriteLine(kbId);

// Retrieve the knowledge box by slug
Console.WriteLine($"\nRetrieving knowledge box by slug: {kbSlug}");
var getResult = await httpClient.KbBySlugKbSSlugGetAsync(kbSlug).ConfigureAwait(false);

var kbDetails = getResult switch
{
    OkKnowledgeBoxObjHTTPValidationError ok =>
        $"Retrieved KB:\n  Slug: {ok.Value.Slug}\n  UUID: {ok.Value.Uuid}",
    ErrorKnowledgeBoxObjHTTPValidationError error => error.Value switch
    {
        HttpError<HTTPValidationError>.ErrorResponseError err =>
            $"Error {err.StatusCode}: {err.Body}",
        HttpError<HTTPValidationError>.ExceptionError err =>
            $"Exception: {err.Exception.Message}",
        _ => "Unknown error",
    },
};

Console.WriteLine(kbDetails);
