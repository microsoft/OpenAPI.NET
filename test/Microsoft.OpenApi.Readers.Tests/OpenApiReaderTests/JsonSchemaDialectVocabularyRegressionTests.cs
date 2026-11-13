// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Text.Json.Nodes;
using Microsoft.OpenApi.Reader;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.OpenApiReaderTests;

public class JsonSchemaDialectVocabularyRegressionTests
{
    private const string UnsupportedDialect = "http://json-schema.org/draft-07/schema#";
    private const string KnownDialect = "https://json-schema.org/draft/2020-12/schema";
    private const string Vocabulary = "urn:example:vocab";

    [Theory]
    [InlineData("3.1.1", OpenApiConstants.Json)]
    [InlineData("3.1.1", OpenApiConstants.Yaml)]
    [InlineData("3.2.0", OpenApiConstants.Json)]
    [InlineData("3.2.0", OpenApiConstants.Yaml)]
    public void UnsupportedDialectPreservesVocabularyThatTheFallbackCannotParse(string version, string format)
    {
        var result = ParseSchema(version, format, UnsupportedDialect,
            """{ "$vocabulary": { "urn:example:vocab": "custom" } }""",
            """
            $vocabulary:
              urn:example:vocab: custom
            """);

        AssertUnsupportedDialectDiagnostic(result);
        AssertRawVocabulary(result.Document.Components.Schemas["Parent"]);
        Assert.Contains(result.Diagnostic.Errors,
            error => error.Pointer == "#/components/schemas/Parent/$vocabulary/urn:example:vocab");
    }

    [Theory]
    [InlineData("3.1.1", OpenApiConstants.Json)]
    [InlineData("3.1.1", OpenApiConstants.Yaml)]
    [InlineData("3.2.0", OpenApiConstants.Json)]
    [InlineData("3.2.0", OpenApiConstants.Yaml)]
    public void UnsupportedDialectPreservesTheWholeVocabularyWhenOneEntryCannotBeParsed(string version, string format)
    {
        var result = ParseSchema(version, format, UnsupportedDialect,
            """
            {
              "$vocabulary": {
                "urn:example:before": true,
                "urn:example:vocab": "custom",
                "urn:example:after": false
              }
            }
            """,
            """
            $vocabulary:
              urn:example:before: true
              urn:example:vocab: custom
              urn:example:after: false
            """);

        AssertUnsupportedDialectDiagnostic(result);
        var schema = result.Document.Components.Schemas["Parent"];
        Assert.Null(schema.Vocabulary);
        AssertRawVocabulary(schema);
        var rawVocabulary = Assert.IsType<JsonObject>(schema.UnrecognizedKeywords["$vocabulary"]);
        Assert.Equal(3, rawVocabulary.Count);
        Assert.True(rawVocabulary["urn:example:before"].GetValue<bool>());
        Assert.False(rawVocabulary["urn:example:after"].GetValue<bool>());
        Assert.Contains(result.Diagnostic.Errors,
            error => error.Pointer == "#/components/schemas/Parent/$vocabulary/urn:example:vocab");
    }

    [Theory]
    [InlineData("3.1.1", OpenApiConstants.Json)]
    [InlineData("3.1.1", OpenApiConstants.Yaml)]
    [InlineData("3.2.0", OpenApiConstants.Json)]
    [InlineData("3.2.0", OpenApiConstants.Yaml)]
    public void UnsupportedDialectPreservesVocabularyOnItsOwnNestedSchema(string version, string format)
    {
        var result = ParseSchema(version, format, UnsupportedDialect,
            """
            {
              "$vocabulary": { "urn:example:vocab": true },
              "properties": {
                "child": { "$vocabulary": { "urn:example:vocab": "custom" } }
              }
            }
            """,
            """
            $vocabulary:
              urn:example:vocab: true
            properties:
              child:
                $vocabulary:
                  urn:example:vocab: custom
            """);

        AssertUnsupportedDialectDiagnostic(result);
        var parent = result.Document.Components.Schemas["Parent"];
        Assert.True(parent.Vocabulary[Vocabulary]);
        AssertNoRawVocabulary(parent);
        Assert.False(parent.UnrecognizedKeywords?.ContainsKey("properties") ?? false);
        AssertRawVocabulary(parent.Properties["child"]);
        Assert.Contains(result.Diagnostic.Errors,
            error => error.Pointer == "#/components/schemas/Parent/properties/child/$vocabulary/urn:example:vocab");
    }

    [Theory]
    [InlineData("3.1.1", OpenApiConstants.Json)]
    [InlineData("3.1.1", OpenApiConstants.Yaml)]
    [InlineData("3.2.0", OpenApiConstants.Json)]
    [InlineData("3.2.0", OpenApiConstants.Yaml)]
    public void KnownDialectStillPropagatesVocabularyConversionFailure(string version, string format)
    {
        Assert.Throws<FormatException>(() => ParseSchema(version, format, KnownDialect,
            """{ "$vocabulary": { "urn:example:vocab": "custom" } }""",
            """
            $vocabulary:
              urn:example:vocab: custom
            """));
    }

    [Theory]
    [InlineData("3.1.1", OpenApiConstants.Json)]
    [InlineData("3.1.1", OpenApiConstants.Yaml)]
    [InlineData("3.2.0", OpenApiConstants.Json)]
    [InlineData("3.2.0", OpenApiConstants.Yaml)]
    public void UnknownParentDoesNotRecoverVocabularyConversionFailureFromKnownChild(string version, string format)
    {
        Assert.Throws<FormatException>(() => ParseSchema(version, format, UnsupportedDialect,
            $$"""
            {
              "properties": {
                "child": {
                  "$schema": "{{KnownDialect}}",
                  "$vocabulary": { "urn:example:vocab": "custom" }
                }
              }
            }
            """,
            $$"""
            properties:
              child:
                $schema: {{KnownDialect}}
                $vocabulary:
                  urn:example:vocab: custom
            """));
    }

    [Theory]
    [InlineData("3.1.1", OpenApiConstants.Json)]
    [InlineData("3.1.1", OpenApiConstants.Yaml)]
    [InlineData("3.2.0", OpenApiConstants.Json)]
    [InlineData("3.2.0", OpenApiConstants.Yaml)]
    public void UnsupportedDialectKeepsValidVocabularyTyped(string version, string format)
    {
        var result = ParseSchema(version, format, UnsupportedDialect,
            """{ "$vocabulary": { "urn:example:vocab": false } }""",
            """
            $vocabulary:
              urn:example:vocab: false
            """);

        AssertUnsupportedDialectDiagnostic(result);
        var schema = result.Document.Components.Schemas["Parent"];
        Assert.False(schema.Vocabulary[Vocabulary]);
        AssertNoRawVocabulary(schema);
        Assert.Single(result.Diagnostic.Errors);
    }

    [Theory]
    [InlineData("3.1.1", OpenApiConstants.Json, "$ref")]
    [InlineData("3.1.1", OpenApiConstants.Yaml, "$ref")]
    [InlineData("3.2.0", OpenApiConstants.Json, "$ref")]
    [InlineData("3.2.0", OpenApiConstants.Yaml, "$ref")]
    [InlineData("3.1.1", OpenApiConstants.Json, "$dynamicRef")]
    [InlineData("3.1.1", OpenApiConstants.Yaml, "$dynamicRef")]
    [InlineData("3.2.0", OpenApiConstants.Json, "$dynamicRef")]
    [InlineData("3.2.0", OpenApiConstants.Yaml, "$dynamicRef")]
    public void UnsupportedDialectPreservesVocabularyOnReferenceMetadata(
        string version,
        string format,
        string referenceKeyword)
    {
        var referenceValue = referenceKeyword == "$ref" ? "#/components/schemas/Target" : "#target";
        var result = ParseSchema(version, format, UnsupportedDialect,
            $$"""
            {
              "{{referenceKeyword}}": "{{referenceValue}}",
              "$vocabulary": { "urn:example:vocab": "custom" }
            }
            """,
            $$"""
            {{referenceKeyword}}: '{{referenceValue}}'
            $vocabulary:
              urn:example:vocab: custom
            """,
            includeReferenceTarget: true);

        AssertUnsupportedDialectDiagnostic(result);
        var schemas = result.Document.Components.Schemas;
        var reference = Assert.IsType<OpenApiSchemaReference>(schemas["Parent"]);
        Assert.NotNull(reference.Reference.UnrecognizedKeywords);
        Assert.True(reference.Reference.UnrecognizedKeywords.TryGetValue("$vocabulary", out var rawKeyword));
        var rawVocabulary = Assert.IsType<JsonObject>(rawKeyword);
        Assert.Equal("custom", rawVocabulary[Vocabulary].GetValue<string>());
        Assert.Same(reference.Reference.UnrecognizedKeywords, reference.UnrecognizedKeywords);
        var target = schemas["Target"];
        Assert.Same(target, reference.Target);
        Assert.True(target.Vocabulary[Vocabulary]);
        AssertNoRawVocabulary(target);
    }

    private static void AssertUnsupportedDialectDiagnostic(ReadResult result)
    {
        Assert.NotNull(result.Document);
        Assert.Contains(result.Diagnostic.Errors,
            error => error.Pointer == "#/jsonSchemaDialect" &&
                     error.Message == $"The JSON Schema dialect '{new Uri(UnsupportedDialect)}' is not supported.");
    }

    private static void AssertRawVocabulary(IOpenApiSchema schema)
    {
        Assert.NotNull(schema.UnrecognizedKeywords);
        Assert.True(schema.UnrecognizedKeywords.TryGetValue("$vocabulary", out var rawKeyword));
        var rawVocabulary = Assert.IsType<JsonObject>(rawKeyword);
        Assert.Equal("custom", rawVocabulary[Vocabulary].GetValue<string>());
    }

    private static void AssertNoRawVocabulary(IOpenApiSchema schema)
    {
        Assert.False(schema.UnrecognizedKeywords?.ContainsKey("$vocabulary") ?? false);
    }

    private static ReadResult ParseSchema(
        string version,
        string format,
        string dialect,
        string jsonSchema,
        string yamlSchema,
        bool includeReferenceTarget = false)
    {
        var settings = new OpenApiReaderSettings();
        string description;
        if (format == OpenApiConstants.Json)
        {
            var target = includeReferenceTarget
                ? """, "Target": { "$dynamicAnchor": "target", "$vocabulary": { "urn:example:vocab": true } }"""
                : string.Empty;
            description = $$"""
                {
                  "openapi": "{{version}}",
                  "info": { "title": "test", "version": "1.0.0" },
                  "jsonSchemaDialect": "{{dialect}}",
                  "components": {
                    "schemas": {
                      "Parent": {{jsonSchema}}{{target}}
                    }
                  }
                }
                """;
        }
        else
        {
            settings.AddYamlReader();
            var indentedSchema = "      " + yamlSchema.Replace("\n", "\n      ");
            var target = includeReferenceTarget
                ? "    Target:\n      $dynamicAnchor: target\n      $vocabulary:\n        urn:example:vocab: true"
                : string.Empty;
            description = $$"""
                openapi: {{version}}
                info:
                  title: test
                  version: 1.0.0
                jsonSchemaDialect: {{dialect}}
                components:
                  schemas:
                    Parent:
                {{indentedSchema}}
                {{target}}
                """;
        }

        return OpenApiDocument.Parse(description, format, settings);
    }
}
