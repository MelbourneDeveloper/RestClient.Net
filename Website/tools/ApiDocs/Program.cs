using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

var options = Arguments.Parse(args);
var projects =
    options.Projects.Length > 0
        ? options.Projects
        :
        [
            "RestClient.Net",
            "Outcome",
            "RestClient.Net.OpenApiGenerator",
            "RestClient.Net.McpGenerator",
            "Exhaustion",
        ];
var files = projects
    .SelectMany(project =>
        Directory
            .EnumerateFiles(
                Path.Combine(options.Root, project),
                "*.cs",
                SearchOption.AllDirectories
            )
            .Where(file =>
                !Path.GetRelativePath(options.Root, file)
                    .Split(Path.DirectorySeparatorChar)
                    .Any(part => part is "bin" or "obj")
            )
            .Select(file => (Project: project, Path: file))
    )
    .OrderBy(item => item.Path, StringComparer.Ordinal)
    .ToArray();
var trees = files
    .Select(file =>
        CSharpSyntaxTree.ParseText(
            File.ReadAllText(file.Path),
            new CSharpParseOptions(LanguageVersion.CSharp12, DocumentationMode.Diagnose),
            file.Path
        )
    )
    .ToArray();
foreach (
    var error in trees
        .SelectMany(tree => tree.GetDiagnostics())
        .Where(item => item.Severity == DiagnosticSeverity.Error)
)
    throw new InvalidOperationException($"Cannot export invalid C# source: {error}");
var references = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!)
    .Split(Path.PathSeparator)
    .Select(file => MetadataReference.CreateFromFile(file));

// Match the SDK implicit imports enabled by the repository Directory.Build.props.
// Semantic symbols identify public declarations and overloads; signatures retain their exact
// source types/aliases, constraints and default expressions without building product packages.
var compilation = CSharpCompilation.Create(
    "ApiDocumentation",
    trees.Append(
        CSharpSyntaxTree.ParseText(
            """
            global using global::System;
            global using global::System.Collections.Generic;
            global using global::System.IO;
            global using global::System.Linq;
            global using global::System.Net.Http;
            global using global::System.Threading;
            global using global::System.Threading.Tasks;
            """,
            new CSharpParseOptions(LanguageVersion.CSharp12)
        )
    ),
    references,
    new CSharpCompilationOptions(
        OutputKind.DynamicallyLinkedLibrary,
        nullableContextOptions: NullableContextOptions.Enable
    )
);
var types = new Dictionary<string, ApiType>(StringComparer.Ordinal);
for (var index = 0; index < trees.Length; index++)
{
    var tree = trees[index];
    var model = compilation.GetSemanticModel(tree);
    foreach (
        var declaration in tree.GetRoot()
            .DescendantNodes()
            .Where(node => node is BaseTypeDeclarationSyntax or DelegateDeclarationSyntax)
    )
    {
        if (
            model.GetDeclaredSymbol(declaration) is not INamedTypeSymbol symbol
            || !Exporter.IsPublic(symbol)
        )
            continue;
        var id = symbol.GetDocumentationCommentId()!;
        if (types.ContainsKey(id))
            continue;
        var declarations = symbol
            .DeclaringSyntaxReferences.Select(reference => reference.GetSyntax())
            .OrderBy(node => node.SyntaxTree.FilePath, StringComparer.Ordinal)
            .ThenBy(node => node.SpanStart)
            .ToArray();
        var primary =
            declarations.FirstOrDefault(node => Exporter.Documentation(node).Summary.Length > 0)
            ?? declarations[0];
        var members = new List<ApiMember>();
        foreach (
            var member in symbol
                .GetMembers()
                .Where(member =>
                    !member.IsImplicitlyDeclared
                    && member is not INamedTypeSymbol
                    && Exporter.IsPublic(member)
                )
        )
        {
            var node = member.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax();
            if (
                node is null
                || node
                    is not (MemberDeclarationSyntax or VariableDeclaratorSyntax or ParameterSyntax)
            )
                continue;
            members.Add(Exporter.Member(member, node, options));
        }
        types[id] = new ApiType(
            id,
            symbol.Name,
            symbol.ToDisplayString(Exporter.TypeNames),
            symbol.ContainingNamespace.ToDisplayString(),
            files[index].Project,
            symbol.TypeKind.ToString().ToLowerInvariant(),
            Exporter.Signature(primary),
            Exporter.Documentation(primary),
            Exporter.Parameters(primary),
            Exporter.Source(primary, options),
            "/api/reference/" + Exporter.Slug(id[2..]) + "/",
            members.OrderBy(member => member.Id, StringComparer.Ordinal).ToArray()
        );
    }
}
var api = new ApiIndex(
    1,
    options.SourceRef,
    projects.Order(StringComparer.Ordinal).ToArray(),
    types.Values.OrderBy(type => type.Id, StringComparer.Ordinal).ToArray()
);
Directory.CreateDirectory(options.Output);
foreach (var old in Directory.EnumerateFiles(options.Output, "*.md"))
    File.Delete(old);
File.WriteAllText(
    Path.Combine(options.Output, "api.json"),
    JsonSerializer.Serialize(api, Exporter.JsonOptions) + "\n"
);
foreach (var type in api.Types)
    File.WriteAllText(
        Path.Combine(options.Output, Exporter.Slug(type.Id[2..]) + ".md"),
        Exporter.Markdown(type)
    );
File.WriteAllText(Path.Combine(options.Output, "index.md"), Exporter.IndexMarkdown(api));
Console.WriteLine(
    $"Exported {api.Types.Length} public types and {api.Types.Sum(type => type.Members.Length)} members from {files.Length} C# source files."
);

internal sealed record Arguments(string Root, string Output, string SourceRef, string[] Projects)
{
    public static Arguments Parse(string[] args)
    {
        var values = new Dictionary<string, string>();
        for (var index = 0; index < args.Length; index += 2)
        {
            if (
                index + 1 >= args.Length
                || !new[] { "--root", "--output", "--source-ref", "--projects" }.Contains(
                    args[index]
                )
            )
                throw new ArgumentException(
                    "Use --root PATH --output PATH [--source-ref REF] [--projects dir,dir]"
                );
            values.Add(args[index], args[index + 1]);
        }
        return new Arguments(
            Path.GetFullPath(values["--root"]),
            Path.GetFullPath(values["--output"]),
            values.GetValueOrDefault("--source-ref", "main"),
            values
                .GetValueOrDefault("--projects", "")
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
        );
    }
}

internal sealed record SourceLocation(string File, int Line, string Url);

internal sealed record Parameter(string Name, string Type, string Modifiers, string? Default);

internal sealed record NamedDocumentation(string Name, string Description);

internal sealed record Documentation(
    string Summary,
    string Remarks,
    string Returns,
    NamedDocumentation[] Parameters,
    NamedDocumentation[] TypeParameters
);

internal sealed record ApiMember(
    string Id,
    string Anchor,
    string Name,
    string Kind,
    string Signature,
    Documentation Docs,
    Parameter[] Parameters,
    SourceLocation Source
);

internal sealed record ApiType(
    string Id,
    string Name,
    string DisplayName,
    string Namespace,
    string Project,
    string Kind,
    string Signature,
    Documentation Docs,
    Parameter[] Parameters,
    SourceLocation Source,
    string Url,
    ApiMember[] Members
);

internal sealed record ApiIndex(
    int SchemaVersion,
    string SourceRef,
    string[] Projects,
    ApiType[] Types
);

internal static partial class Exporter
{
    [GeneratedRegex("[^a-z0-9]+")]
    private static partial Regex SlugPattern();

    [GeneratedRegex(@"\s+")]
    private static partial Regex Whitespace();

    [GeneratedRegex(@"(?m)^\s*/// ?")]
    private static partial Regex DocumentationPrefix();

    [GeneratedRegex(@"^\s*/\*\*|\*/\s*$")]
    private static partial Regex BlockCommentBoundary();

    [GeneratedRegex(@"(?m)^\s*\* ?")]
    private static partial Regex BlockCommentLine();

    public static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
    };
    public static readonly SymbolDisplayFormat TypeNames = new(
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
        genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
        miscellaneousOptions: SymbolDisplayMiscellaneousOptions.UseSpecialTypes
            | SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier
    );

    public static bool IsPublic(ISymbol symbol) =>
        symbol.DeclaredAccessibility == Accessibility.Public
        && (symbol.ContainingType is null || IsPublic(symbol.ContainingType));

    public static string Slug(string value) =>
        SlugPattern().Replace(value.ToLowerInvariant(), "-").Trim('-');

    private static string Anchor(ISymbol symbol) =>
        Slug(symbol.Name)
        + "-"
        + Convert
            .ToHexString(
                SHA256.HashData(Encoding.UTF8.GetBytes(symbol.GetDocumentationCommentId()!))
            )[..12]
            .ToLowerInvariant();

    public static SourceLocation Source(SyntaxNode node, Arguments options)
    {
        var file = Path.GetRelativePath(options.Root, node.SyntaxTree.FilePath).Replace('\\', '/');
        var line = node.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
        return new SourceLocation(
            file,
            line,
            "https://github.com/MelbourneDeveloper/RestClient.Net/blob/"
                + Uri.EscapeDataString(options.SourceRef)
                + "/"
                + string.Join('/', file.Split('/').Select(Uri.EscapeDataString))
                + "#L"
                + line
        );
    }

    public static ApiMember Member(ISymbol symbol, SyntaxNode node, Arguments options)
    {
        var documentationNode = node is VariableDeclaratorSyntax ? node.Parent!.Parent! : node;
        return new ApiMember(
            symbol.GetDocumentationCommentId()!,
            Anchor(symbol),
            symbol.Name,
            symbol.Kind.ToString().ToLowerInvariant(),
            symbol is IMethodSymbol { MethodKind: MethodKind.Constructor }
            && node is TypeDeclarationSyntax
                ? $"public {symbol.ContainingType.Name}({string.Join(", ", Parameters(node).Select(parameter => parameter.Type + " " + parameter.Name + (parameter.Default is null ? "" : " = " + parameter.Default)))});"
                : Signature(node),
            Documentation(documentationNode),
            Parameters(node),
            Source(node, options)
        );
    }

    public static Parameter[] Parameters(SyntaxNode node)
    {
        var parameters = node switch
        {
            BaseMethodDeclarationSyntax method => method.ParameterList.Parameters,
            DelegateDeclarationSyntax declaration => declaration.ParameterList.Parameters,
            RecordDeclarationSyntax record => record.ParameterList?.Parameters ?? default,
            ClassDeclarationSyntax type => type.ParameterList?.Parameters ?? default,
            StructDeclarationSyntax type => type.ParameterList?.Parameters ?? default,
            IndexerDeclarationSyntax indexer => indexer.ParameterList.Parameters,
            _ => default,
        };
        return parameters
            .Select(parameter => new Parameter(
                parameter.Identifier.ValueText,
                parameter.Type?.ToString() ?? "",
                string.Join(' ', parameter.Modifiers.Select(token => token.Text)),
                parameter.Default?.Value.ToString()
            ))
            .ToArray();
    }

    public static string Signature(SyntaxNode node)
    {
        SyntaxNode signature = node switch
        {
            MethodDeclarationSyntax value => value
                .WithBody(null)
                .WithExpressionBody(null)
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
            ConstructorDeclarationSyntax value => value
                .WithBody(null)
                .WithExpressionBody(null)
                .WithInitializer(null)
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
            OperatorDeclarationSyntax value => value
                .WithBody(null)
                .WithExpressionBody(null)
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
            ConversionOperatorDeclarationSyntax value => value
                .WithBody(null)
                .WithExpressionBody(null)
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
            PropertyDeclarationSyntax value => value
                .WithExpressionBody(null)
                .WithInitializer(null)
                .WithAccessorList(Accessors(value.AccessorList))
                .WithSemicolonToken(default),
            IndexerDeclarationSyntax value => value
                .WithExpressionBody(null)
                .WithAccessorList(Accessors(value.AccessorList))
                .WithSemicolonToken(default),
            EventDeclarationSyntax value => value.WithAccessorList(Accessors(value.AccessorList)),
            ParameterSyntax value => SyntaxFactory.ParseMemberDeclaration(
                $"public {value.Type} {value.Identifier} {{ get; init; }}"
            )!,
            VariableDeclaratorSyntax value
                when value.Parent?.Parent is FieldDeclarationSyntax field => field.WithDeclaration(
                field.Declaration.WithVariables(
                    SyntaxFactory.SingletonSeparatedList(
                        field.Modifiers.Any(SyntaxKind.ConstKeyword)
                            ? value
                            : value.WithInitializer(null)
                    )
                )
            ),
            VariableDeclaratorSyntax value
                when value.Parent?.Parent is EventFieldDeclarationSyntax field =>
                field.WithDeclaration(
                    field.Declaration.WithVariables(
                        SyntaxFactory.SingletonSeparatedList(value.WithInitializer(null))
                    )
                ),
            _ => node,
        };
        signature = signature switch
        {
            TypeDeclarationSyntax type => type.WithMembers(default)
                .WithOpenBraceToken(default)
                .WithCloseBraceToken(default)
                .WithSemicolonToken(default),
            EnumDeclarationSyntax type => type.WithMembers(default)
                .WithOpenBraceToken(default)
                .WithCloseBraceToken(default)
                .WithSemicolonToken(default),
            _ => signature,
        };
        return Clean(signature);
    }

    private static string Clean(SyntaxNode node) =>
        node.ReplaceTokens(node.DescendantTokens(), (_, token) => token.WithoutTrivia())
            .WithoutTrivia()
            .NormalizeWhitespace()
            .ToFullString();

    private static AccessorListSyntax Accessors(AccessorListSyntax? list) =>
        list is null
            ? SyntaxFactory.AccessorList(
                SyntaxFactory.SingletonList(
                    SyntaxFactory
                        .AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                        .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                )
            )
            : list.WithAccessors(
                SyntaxFactory.List(
                    list.Accessors.Select(accessor =>
                        accessor
                            .WithBody(null)
                            .WithExpressionBody(null)
                            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                    )
                )
            );

    public static Documentation Documentation(SyntaxNode node)
    {
        var comments = node.GetLeadingTrivia()
            .Where(trivia =>
                trivia.HasStructure && trivia.GetStructure() is DocumentationCommentTriviaSyntax
            )
            .Select(trivia => trivia.ToFullString());
        var xml = string.Join("\n", comments);
        xml = DocumentationPrefix().Replace(xml, "");
        xml = BlockCommentBoundary().Replace(xml, "");
        xml = BlockCommentLine().Replace(xml, "");
        var root = XElement.Parse("<doc>" + xml + "</doc>");
        string Read(string name) => string.Join("\n\n", root.Elements(name).Select(Render)).Trim();
        NamedDocumentation[] Named(string name) =>
            root.Elements(name)
                .Select(element => new NamedDocumentation(
                    element.Attribute("name")?.Value ?? "",
                    Render(element).Trim()
                ))
                .ToArray();
        return new Documentation(
            Read("summary"),
            Read("remarks"),
            Read("returns"),
            Named("param"),
            Named("typeparam")
        );
    }

    private static string Render(XNode node) =>
        node switch
        {
            XText text => Whitespace()
                .Replace(text.Value, " ")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;"),
            XElement element when element.Name.LocalName is "paramref" or "typeparamref" => "`"
                + element.Attribute("name")?.Value
                + "`",
            XElement element when element.Name.LocalName == "see" => "`"
                + (
                    element.Attribute("cref")?.Value
                    ?? element.Attribute("langword")?.Value
                    ?? element.Value
                )
                + "`",
            XElement element when element.Name.LocalName == "c" => "`" + element.Value + "`",
            XElement element when element.Name.LocalName == "code" => "\n\n```csharp\n"
                + element.Value.Trim()
                + "\n```\n\n",
            XElement element when element.Name.LocalName == "para" => "\n\n"
                + string.Concat(element.Nodes().Select(Render)).Trim()
                + "\n\n",
            XElement element when element.Name.LocalName == "item" => "\n- "
                + string.Concat(element.Nodes().Select(Render)).Trim(),
            XElement element => string.Concat(element.Nodes().Select(Render)),
            _ => "",
        };

    private static string Frontmatter(string title, string permalink) =>
        "---\nlayout: layouts/api.njk\ntitle: "
        + JsonSerializer.Serialize(title)
        + "\nlang: en\npermalink: "
        + permalink
        + "\ntemplateEngineOverride: md\n---\n\n";

    private static string Cell(string value) => value.Replace("|", "\\|").Replace("\n", " ");

    private static void AppendDocumentation(
        StringBuilder builder,
        Documentation docs,
        Parameter[] parameters
    )
    {
        if (docs.Summary.Length > 0)
            builder.AppendLine(docs.Summary).AppendLine();
        if (parameters.Length > 0)
        {
            builder
                .AppendLine("| Parameter | Type | Default | Description |")
                .AppendLine("| --- | --- | --- | --- |");
            foreach (var parameter in parameters)
                builder.AppendLine(
                    $"| `{parameter.Name}` | `{Cell(parameter.Type)}` | {(parameter.Default is null ? "Required" : "`" + Cell(parameter.Default) + "`")} | {Cell(docs.Parameters.FirstOrDefault(item => item.Name == parameter.Name)?.Description ?? "")} |"
                );
            builder.AppendLine();
        }
        foreach (var parameter in docs.TypeParameters)
            builder.AppendLine($"**Type parameter `{parameter.Name}`:** {parameter.Description}\n");
        if (docs.Returns.Length > 0)
            builder.AppendLine("**Returns:** " + docs.Returns).AppendLine();
        if (docs.Remarks.Length > 0)
            builder.AppendLine("**Remarks:** " + docs.Remarks).AppendLine();
    }

    public static string Markdown(ApiType type)
    {
        var builder = new StringBuilder(Frontmatter(type.DisplayName, type.Url));
        builder.AppendLine(
            $"[All API types](/api/reference/) · `{type.Project}` · [View source]({type.Source.Url})\n"
        );
        builder.AppendLine("```csharp").AppendLine(type.Signature).AppendLine("```\n");
        AppendDocumentation(builder, type.Docs, type.Parameters);
        if (type.Members.Length > 0)
        {
            builder.AppendLine("## Members\n");
            foreach (var member in type.Members)
                builder.AppendLine($"- [{member.Name}](#{member.Anchor})");
            builder.AppendLine();
        }
        foreach (var member in type.Members)
        {
            builder.AppendLine(
                $"<h2 id=\"{member.Anchor}\">{System.Net.WebUtility.HtmlEncode(member.Name)}</h2>\n"
            );
            builder.AppendLine("```csharp").AppendLine(member.Signature).AppendLine("```\n");
            AppendDocumentation(builder, member.Docs, member.Parameters);
            builder.AppendLine(
                $"[View source: {member.Source.File}:{member.Source.Line}]({member.Source.Url})\n"
            );
        }
        return builder.ToString();
    }

    public static string IndexMarkdown(ApiIndex api)
    {
        var builder = new StringBuilder(Frontmatter("Source API reference", "/api/reference/"));
        builder.AppendLine(
            $"Explore {api.Types.Length} public types and {api.Types.Sum(type => type.Members.Length)} members, exported directly from the C# source and XML documentation.\n"
        );
        builder.AppendLine(
            "[Download the API index](/api/reference/api.json) · [JSON schema](/api/reference/schema.json)\n"
        );
        foreach (var project in api.Projects)
        {
            builder.AppendLine("## " + project + "\n");
            foreach (var type in api.Types.Where(type => type.Project == project))
                builder.AppendLine(
                    $"- [`{type.DisplayName}`]({type.Url}){(type.Docs.Summary.Length == 0 ? "" : " — " + type.Docs.Summary.Split('\n')[0])}"
                );
            builder.AppendLine();
        }
        return builder.ToString();
    }
}
