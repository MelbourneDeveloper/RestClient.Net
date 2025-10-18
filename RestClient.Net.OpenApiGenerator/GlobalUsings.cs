global using Microsoft.OpenApi;
global using Microsoft.OpenApi.Reader;
global using ErrorUrl = Outcome.Result<(string, string), string>.Error<(string, string), string>;
global using GeneratorError = Outcome.Result<
    RestClient.Net.OpenApiGenerator.GeneratorResult,
    string
>.Error<RestClient.Net.OpenApiGenerator.GeneratorResult, string>;
global using GeneratorOk = Outcome.Result<
    RestClient.Net.OpenApiGenerator.GeneratorResult,
    string
>.Ok<RestClient.Net.OpenApiGenerator.GeneratorResult, string>;
global using OkUrl = Outcome.Result<(string, string), string>.Ok<(string, string), string>;
