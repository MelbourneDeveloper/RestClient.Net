namespace RestClient.Net.OpenApiGenerator;

/// <summary>Represents the result of code generation.</summary>
/// <param name="ExtensionMethodsCode">The generated extension methods code.</param>
/// <param name="ModelsCode">The generated models code.</param>
public record GeneratorResult(string ExtensionMethodsCode, string ModelsCode);
