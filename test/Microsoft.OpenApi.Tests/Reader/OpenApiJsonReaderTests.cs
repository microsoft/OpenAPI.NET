using System;
using System.IO;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Validations;
using Xunit;

namespace Microsoft.OpenApi.Tests.Reader;

public class OpenApiJsonReaderTests
{
    private static readonly Uri DocumentLocation = new("https://contoso.test/openapi.json");

    [Fact]
    public void ReadReturnsDiagnosticWhenJsonIsInvalid()
    {
        var reader = new OpenApiJsonReader();
        using var stream = CreateStream("{");

        var result = reader.Read(stream, DocumentLocation, new OpenApiReaderSettings());

        Assert.Null(result.Document);
        var error = Assert.Single(result.Diagnostic.Errors);
        Assert.Equal(OpenApiConstants.Json, result.Diagnostic.Format);
        Assert.Contains("Expected depth to be zero at the end of the JSON payload.", error.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ReadAsyncReturnsDiagnosticWhenJsonIsInvalid()
    {
        var reader = new OpenApiJsonReader();
        await using var stream = CreateStream("{");

        var result = await reader.ReadAsync(stream, DocumentLocation, new OpenApiReaderSettings(), CancellationToken.None);

        Assert.Null(result.Document);
        Assert.Single(result.Diagnostic.Errors);
        Assert.Equal(OpenApiConstants.Json, result.Diagnostic.Format);
    }

    [Fact]
    public void ReadValidatesParsedDocumentAgainstConfiguredRules()
    {
        var ruleSet = new ValidationRuleSet();
        ruleSet.Add(typeof(OpenApiDocument), new ValidationRule<OpenApiDocument>("AlwaysFail", static (context, _) => context.CreateError("rule", "Document failed validation.")));
        var reader = new OpenApiJsonReader();
        using var stream = CreateStream("""{"openapi":"3.0.1","info":{"title":"Sample","version":"1.0.0"},"paths":{}}""");

        var result = reader.Read(stream, DocumentLocation, new OpenApiReaderSettings { RuleSet = ruleSet });

        Assert.NotNull(result.Document);
        var error = Assert.Single(result.Diagnostic.Errors);
        Assert.Equal("#/", error.Pointer);
        Assert.Equal("Document failed validation.", error.Message);
    }

    [Fact]
    public void ReadReturnsDiagnosticWhenRootNodeCannotBeParsedAsDocument()
    {
        var reader = new OpenApiJsonReader();

        var result = reader.Read(JsonNode.Parse("[]")!, DocumentLocation, new OpenApiReaderSettings());

        Assert.Null(result.Document);
        var error = Assert.Single(result.Diagnostic.Errors);
        Assert.Contains("Expected scalar value.", error.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void ReadFragmentReturnsDiagnosticWhenJsonNodeCannotBeParsedAsSchema()
    {
        var reader = new OpenApiJsonReader();
        var input = JsonNode.Parse("[]")!;

        var schema = reader.ReadFragment<OpenApiSchema>(
            input,
            OpenApiSpecVersion.OpenApi3_0,
            new OpenApiDocument(),
            out var diagnostic);

        Assert.Null(schema);
        var error = Assert.Single(diagnostic.Errors);
        Assert.Contains("schema must be a map/object", error.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void ReadFragmentReturnsDiagnosticWhenJsonIsInvalid()
    {
        var reader = new OpenApiJsonReader();
        using var stream = CreateStream("{");

        var schema = reader.ReadFragment<OpenApiSchema>(stream, OpenApiSpecVersion.OpenApi3_0, new OpenApiDocument(), out var diagnostic);

        Assert.Null(schema);
        Assert.Single(diagnostic.Errors);
    }

    private static MemoryStream CreateStream(string json)
    {
        return new MemoryStream(Encoding.UTF8.GetBytes(json));
    }
}
