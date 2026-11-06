// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.OpenApi.Reader;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.OpenApiReaderTests;

public class JsonSchemaDialectDiagnosticTests
{
    private const string CustomDialect = "https://example.com/custom-dialect";
    private const string Draft202012 = "https://json-schema.org/draft/2020-12/schema";

    [Theory]
    [InlineData("3.1.1")]
    [InlineData("3.2.0")]
    public async Task ResolvingDialectPreservesConfiguredReaderDiagnosticsWithoutDuplicates(string version)
    {
        const string format = "json-with-diagnostics";
        var loader = new KnownDialectLoader();
        var settings = new OpenApiReaderSettings
        {
            LoadExternalRefs = true,
            CustomExternalLoader = loader,
        };
        Assert.True(settings.TryAddReader(format, new DiagnosticReader()));
        using var input = CreateStream($$"""
            {
              "openapi": "{{version}}",
              "info": { "title": "test", "version": "1.0.0" },
              "$schema": "{{Draft202012}}",
              "unexpected": true,
              "jsonSchemaDialect": "{{CustomDialect}}",
              "components": {
                "schemas": {
                  "Item": { "type": "string" }
                }
              }
            }
            """);

        var result = await OpenApiDocument.LoadAsync(
            input,
            format,
            settings,
            TestContext.Current.CancellationToken);

        Assert.NotNull(result.Document);
        Assert.Equal(1, loader.CallCount);
        Assert.Equal(2, result.Diagnostic.Errors.Count);
        Assert.Equal(2, result.Diagnostic.Warnings.Count);
        var readerError = Assert.Single(result.Diagnostic.Errors,
            error => error.Message == DiagnosticReader.ErrorMessage);
        Assert.Equal("#/configured-reader-error", readerError.Pointer);
        var readerWarning = Assert.Single(result.Diagnostic.Warnings,
            warning => warning.Message == DiagnosticReader.WarningMessage);
        Assert.Equal("#/configured-reader-warning", readerWarning.Pointer);
        Assert.Single(result.Diagnostic.Errors,
            error => error.Message.StartsWith("unexpected is not a valid property at ", StringComparison.Ordinal));
        Assert.Single(result.Diagnostic.Warnings,
            warning => warning.Message.StartsWith("$schema is not a valid property at ", StringComparison.Ordinal));
        Assert.DoesNotContain(result.Diagnostic.Errors,
            error => error.Message.StartsWith("The JSON Schema dialect ", StringComparison.Ordinal));
    }

    [Theory]
    [InlineData("3.1.1")]
    [InlineData("3.2.0")]
    public async Task ResolvingDialectPreservesConfiguredReaderDocumentChanges(string version)
    {
        const string format = "json-with-document-changes";
        const string expectedTitle = "test - configured reader";
        var loader = new KnownDialectLoader();
        var settings = new OpenApiReaderSettings
        {
            LoadExternalRefs = true,
            CustomExternalLoader = loader,
        };
        Assert.True(settings.TryAddReader(format,
            new DiagnosticReader(result => result.Document.Info.Title = expectedTitle)));
        using var input = CreateDialectDocument(version);

        var result = await OpenApiDocument.LoadAsync(
            input,
            format,
            settings,
            TestContext.Current.CancellationToken);

        Assert.NotNull(result.Document);
        Assert.Equal(expectedTitle, result.Document.Info.Title);
        Assert.Equal(1, loader.CallCount);
    }

    [Theory]
    [InlineData("3.1.1")]
    [InlineData("3.2.0")]
    public async Task ResolvingDialectDoesNotRestoreDocumentRejectedByConfiguredReader(string version)
    {
        const string format = "json-with-document-rejection";
        var loader = new KnownDialectLoader();
        var settings = new OpenApiReaderSettings
        {
            LoadExternalRefs = true,
            CustomExternalLoader = loader,
        };
        Assert.True(settings.TryAddReader(format,
            new DiagnosticReader(result => result.Document = null)));
        using var input = CreateDialectDocument(version);

        var result = await OpenApiDocument.LoadAsync(
            input,
            format,
            settings,
            TestContext.Current.CancellationToken);

        Assert.Null(result.Document);
        Assert.Equal(0, loader.CallCount);
        var readerError = Assert.Single(result.Diagnostic.Errors,
            error => error.Message == DiagnosticReader.ErrorMessage);
        Assert.Equal("#/configured-reader-error", readerError.Pointer);
    }

    [Theory]
    [InlineData("3.1.1")]
    [InlineData("3.2.0")]
    public async Task ResolvingDialectInvokesExtensionParserOnce(string version)
    {
        var extensionCalls = 0;
        var loader = new KnownDialectLoader();
        var settings = new OpenApiReaderSettings
        {
            LoadExternalRefs = true,
            CustomExternalLoader = loader,
            ExtensionParsers =
            {
                {
                    "x-test", (node, _) =>
                    {
                        extensionCalls++;
                        return new JsonNodeExtension(node);
                    }
                }
            },
        };
        using var input = CreateStream($$"""
            {
              "openapi": "{{version}}",
              "info": { "title": "test", "version": "1.0.0" },
              "x-test": true,
              "jsonSchemaDialect": "{{CustomDialect}}",
              "components": {
                "schemas": {
                  "Item": { "type": "string" }
                }
              }
            }
            """);

        var result = await OpenApiDocument.LoadAsync(
            input,
            OpenApiConstants.Json,
            settings,
            TestContext.Current.CancellationToken);

        Assert.Empty(result.Diagnostic.Errors);
        Assert.Equal(1, loader.CallCount);
        Assert.Equal(1, extensionCalls);
        var extension = Assert.IsType<JsonNodeExtension>(result.Document.Extensions["x-test"]);
        Assert.True(extension.Node.GetValue<bool>());
    }

    [Theory]
    [InlineData("3.1.1")]
    [InlineData("3.2.0")]
    public async Task ResolvingDialectInvokesValidationRuleOnce(string version)
    {
        var validationCalls = 0;
        var rules = new ValidationRuleSet();
        rules.Add(typeof(OpenApiDocument),
            new ValidationRule<OpenApiDocument>("CountDocumentVisits", (_, _) => validationCalls++));
        var loader = new KnownDialectLoader();
        var settings = new OpenApiReaderSettings
        {
            LoadExternalRefs = true,
            CustomExternalLoader = loader,
            RuleSet = rules,
        };
        using var input = CreateDialectDocument(version);

        var result = await OpenApiDocument.LoadAsync(
            input,
            OpenApiConstants.Json,
            settings,
            TestContext.Current.CancellationToken);

        Assert.Empty(result.Diagnostic.Errors);
        Assert.Equal(1, loader.CallCount);
        Assert.Equal(1, validationCalls);
    }

    [Theory]
    [InlineData("3.1.1")]
    [InlineData("3.2.0")]
    public async Task ResolvingDialectRemovesOnlyGeneratedErrorFromReadOnlyDiagnostics(string version)
    {
        const string format = "json-with-read-only-diagnostics";
        OpenApiError customError = null;
        var loader = new KnownDialectLoader();
        var settings = new OpenApiReaderSettings
        {
            LoadExternalRefs = true,
            CustomExternalLoader = loader,
        };
        Assert.True(settings.TryAddReader(format, new DiagnosticReader(result =>
        {
            var generatedError = Assert.Single(result.Diagnostic.Errors,
                error => error.Message == $"The JSON Schema dialect '{CustomDialect}' is not supported.");
            customError = new OpenApiError(generatedError.Pointer, generatedError.Message);
            result.Diagnostic.Errors = Array.AsReadOnly(new[] { generatedError, customError });
        })));
        using var input = CreateDialectDocument(version);

        var result = await OpenApiDocument.LoadAsync(
            input,
            format,
            settings,
            TestContext.Current.CancellationToken);

        Assert.NotNull(result.Document);
        Assert.NotNull(customError);
        Assert.Same(customError, Assert.Single(result.Diagnostic.Errors));
        Assert.Equal(1, loader.CallCount);
    }

    [Theory]
    [InlineData("3.1.1")]
    [InlineData("3.2.0")]
    public void NestedSchemaDialectOverridesInheritedDialect(string version)
    {
        var result = OpenApiDocument.Parse(
            $$"""
            {
              "openapi": "{{version}}",
              "info": { "title": "test", "version": "1.0.0" },
              "jsonSchemaDialect": "https://example.com/document-dialect",
              "components": {
                "schemas": {
                  "Parent": {
                    "$schema": "{{Draft202012}}",
                    "type": "object",
                    "properties": {
                      "child": {
                        "$schema": "{{CustomDialect}}",
                        "customKeyword": true,
                        "type": "string"
                      }
                    }
                  }
                }
              }
            }
            """,
            OpenApiConstants.Json);

        var error = Assert.Single(result.Diagnostic.Errors);
        Assert.Equal("#/components/schemas/Parent/properties/child", error.Pointer);
        Assert.Equal($"The JSON Schema dialect '{CustomDialect}' is not supported.", error.Message);
        var parent = result.Document.Components.Schemas["Parent"];
        Assert.Equal(new Uri(Draft202012), parent.Schema);
        var child = parent.Properties["child"];
        Assert.Equal(new Uri(CustomDialect), child.Schema);
        Assert.True(child.UnrecognizedKeywords.ContainsKey("customKeyword"));
    }

    [Theory]
    [InlineData("3.1.1")]
    [InlineData("3.2.0")]
    public void ExplicitSchemaDialectDoesNotLeakIntoFollowingSchemas(string version)
    {
        const string documentDialect = "https://example.com/document-dialect";
        var result = OpenApiDocument.Parse(
            $$"""
            {
              "openapi": "{{version}}",
              "info": { "title": "test", "version": "1.0.0" },
              "jsonSchemaDialect": "{{documentDialect}}",
              "components": {
                "schemas": {
                  "First": {
                    "$schema": "{{Draft202012}}",
                    "type": "object",
                    "properties": { "nested": { "type": "string" } }
                  },
                  "Second": { "type": "string" }
                }
              }
            }
            """,
            OpenApiConstants.Json);

        var error = Assert.Single(result.Diagnostic.Errors);
        Assert.Equal("#/jsonSchemaDialect", error.Pointer);
        Assert.Equal($"The JSON Schema dialect '{documentDialect}' is not supported.", error.Message);
        Assert.Equal(new Uri(Draft202012), result.Document.Components.Schemas["First"].Schema);
        Assert.Null(result.Document.Components.Schemas["Second"].Schema);
    }

    private static MemoryStream CreateStream(string document) => new(Encoding.UTF8.GetBytes(document));

    private static MemoryStream CreateDialectDocument(string version) => CreateStream($$"""
        {
          "openapi": "{{version}}",
          "info": { "title": "test", "version": "1.0.0" },
          "jsonSchemaDialect": "{{CustomDialect}}",
          "components": {
            "schemas": {
              "Item": { "type": "string" }
            }
          }
        }
        """);

    private sealed class KnownDialectLoader : IStreamLoader
    {
        internal int CallCount { get; private set; }

        public Task<Stream> LoadAsync(Uri baseUrl, Uri uri, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Assert.Equal(new Uri(CustomDialect), uri);
            CallCount++;
            return Task.FromResult<Stream>(CreateStream($$"""{ "$schema": "{{Draft202012}}" }"""));
        }
    }

    private sealed class DiagnosticReader : IOpenApiReader
    {
        internal const string ErrorMessage = "Configured reader error.";
        internal const string WarningMessage = "Configured reader warning.";
        private readonly OpenApiJsonReader _reader = new();
        private readonly Action<ReadResult> _onRead;

        internal DiagnosticReader(Action<ReadResult> onRead = null)
        {
            _onRead = onRead;
        }

        public async Task<ReadResult> ReadAsync(
            Stream input,
            Uri location,
            OpenApiReaderSettings settings,
            CancellationToken cancellationToken = default)
        {
            var result = await _reader.ReadAsync(input, location, settings, cancellationToken);
            return AddDiagnostics(result);
        }

        public ReadResult Read(MemoryStream input, Uri location, OpenApiReaderSettings settings)
        {
            return AddDiagnostics(_reader.Read(input, location, settings));
        }

        public T ReadFragment<T>(
            MemoryStream input,
            OpenApiSpecVersion version,
            OpenApiDocument openApiDocument,
            out OpenApiDiagnostic diagnostic,
            OpenApiReaderSettings settings = null) where T : IOpenApiElement
        {
            return _reader.ReadFragment<T>(input, version, openApiDocument, out diagnostic, settings);
        }

        private ReadResult AddDiagnostics(ReadResult result)
        {
            result.Diagnostic.Errors.Add(new OpenApiError("#/configured-reader-error", ErrorMessage));
            result.Diagnostic.Warnings.Add(new OpenApiError("#/configured-reader-warning", WarningMessage));
            _onRead?.Invoke(result);
            return result;
        }
    }
}
