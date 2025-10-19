global using GeneratorError = Outcome.Result<
    RestClient.Net.OpenApiGenerator.GeneratorResult,
    string
>.Error<RestClient.Net.OpenApiGenerator.GeneratorResult, string>;
global using GeneratorOk = Outcome.Result<
    RestClient.Net.OpenApiGenerator.GeneratorResult,
    string
>.Ok<RestClient.Net.OpenApiGenerator.GeneratorResult, string>;
