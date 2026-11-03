// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Text.Json.Nodes;
using Microsoft.OpenApi.Reader;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.OpenApiReaderTests;

public class JsonSchemaDialectFallbackRegressionTests
{
    private const string UnsupportedDialect = "http://json-schema.org/draft-07/schema#";
    private const string KnownDialect = "https://json-schema.org/draft/2020-12/schema";

    [Theory]
    [InlineData("3.1.1", OpenApiConstants.Json)]
    [InlineData("3.1.1", OpenApiConstants.Yaml)]
    [InlineData("3.2.0", OpenApiConstants.Json)]
    [InlineData("3.2.0", OpenApiConstants.Yaml)]
    public void UnsupportedDialectPreservesTupleItemsThatTheFallbackCannotParse(string version, string format)
    {
        var result = ParseSchema(version, format, UnsupportedDialect,
            """{ "type": "array", "items": [{ "type": "string" }] }""",
            """
            type: array
            items:
              - type: string
            """);

        AssertUnsupportedDialectDiagnostic(result);
        AssertRawTupleItems(result.Document.Components.Schemas["Parent"], "string");
    }

    [Theory]
    [InlineData("3.1.1", OpenApiConstants.Json)]
    [InlineData("3.1.1", OpenApiConstants.Yaml)]
    [InlineData("3.2.0", OpenApiConstants.Json)]
    [InlineData("3.2.0", OpenApiConstants.Yaml)]
    public void UnsupportedDialectPreservesAnIntegerKeywordThatTheFallbackCannotParse(string version, string format)
    {
        var result = ParseSchema(version, format, UnsupportedDialect,
            """{ "type": "string", "minLength": "custom-length" }""",
            """
            type: string
            minLength: custom-length
            """);

        AssertUnsupportedDialectDiagnostic(result);
        var schema = result.Document.Components.Schemas["Parent"];
        Assert.Null(schema.MinLength);
        Assert.NotNull(schema.UnrecognizedKeywords);
        Assert.True(schema.UnrecognizedKeywords.TryGetValue("minLength", out var rawKeyword));
        Assert.Equal("custom-length", rawKeyword.GetValue<string>());
    }

    [Theory]
    [InlineData("3.1.1", OpenApiConstants.Json)]
    [InlineData("3.1.1", OpenApiConstants.Yaml)]
    [InlineData("3.2.0", OpenApiConstants.Json)]
    [InlineData("3.2.0", OpenApiConstants.Yaml)]
    public void UnsupportedDialectPreservesTheFailingKeywordOnItsOwnNestedSchema(string version, string format)
    {
        var result = ParseSchema(version, format, UnsupportedDialect,
            """
            {
              "type": "array",
              "items": { "type": "string" },
              "properties": {
                "child": {
                  "type": "array",
                  "items": [{ "type": "integer" }]
                }
              }
            }
            """,
            """
            type: array
            items:
              type: string
            properties:
              child:
                type: array
                items:
                  - type: integer
            """);

        AssertUnsupportedDialectDiagnostic(result);
        var parent = result.Document.Components.Schemas["Parent"];
        Assert.NotNull(parent.Items);
        Assert.Equal(JsonSchemaType.String, parent.Items.Type);
        AssertNoRawKeyword(parent, "items");
        AssertNoRawKeyword(parent, "properties");
        AssertRawTupleItems(parent.Properties["child"], "integer");
    }

    [Theory]
    [InlineData("3.1.1", OpenApiConstants.Json)]
    [InlineData("3.1.1", OpenApiConstants.Yaml)]
    [InlineData("3.2.0", OpenApiConstants.Json)]
    [InlineData("3.2.0", OpenApiConstants.Yaml)]
    public void KnownDialectDoesNotPreserveMalformedItemsAsAnUnrecognizedKeyword(string version, string format)
    {
        var result = ParseSchema(version, format, KnownDialect,
            """{ "type": "array", "items": [{ "type": "string" }] }""",
            """
            type: array
            items:
              - type: string
            """);

        Assert.NotNull(result.Document);
        Assert.NotEmpty(result.Diagnostic.Errors);
        Assert.DoesNotContain(result.Diagnostic.Errors,
            error => error.Message.StartsWith("The JSON Schema dialect ", StringComparison.Ordinal));
        var schema = result.Document.Components.Schemas["Parent"];
        Assert.Null(schema.Items);
        AssertNoRawKeyword(schema, "items");
    }

    [Theory]
    [InlineData("3.1.1", OpenApiConstants.Json)]
    [InlineData("3.1.1", OpenApiConstants.Yaml)]
    [InlineData("3.2.0", OpenApiConstants.Json)]
    [InlineData("3.2.0", OpenApiConstants.Yaml)]
    public void KnownNestedDialectDoesNotInheritFallbackKeywordPreservation(string version, string format)
    {
        var result = ParseSchema(version, format, UnsupportedDialect,
            $$"""
            {
              "type": "object",
              "items": { "type": "boolean" },
              "properties": {
                "child": {
                  "$schema": "{{KnownDialect}}",
                  "type": "array",
                  "items": [{ "type": "string" }]
                }
              }
            }
            """,
            $$"""
            type: object
            items:
              type: boolean
            properties:
              child:
                $schema: {{KnownDialect}}
                type: array
                items:
                  - type: string
            """);

        AssertUnsupportedDialectDiagnostic(result);
        var parent = result.Document.Components.Schemas["Parent"];
        Assert.Equal(JsonSchemaType.Boolean, parent.Items.Type);
        AssertNoRawKeyword(parent, "items");
        AssertNoRawKeyword(parent, "properties");
        var child = parent.Properties["child"];
        Assert.Equal(new Uri(KnownDialect), child.Schema);
        Assert.Null(child.Items);
        AssertNoRawKeyword(child, "items");
        Assert.Contains(result.Diagnostic.Errors,
            error => error.Pointer == "#/components/schemas/Parent/properties/child/items");
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
    public void UnsupportedDialectPreservesFailedItemsOnReferenceMetadata(
        string version,
        string format,
        string referenceKeyword)
    {
        var referenceValue = referenceKeyword == "$ref" ? "#/components/schemas/Target" : "#target";
        var result = ParseSchema(version, format, UnsupportedDialect,
            $$"""
            {
              "{{referenceKeyword}}": "{{referenceValue}}",
              "type": "array",
              "items": [{ "type": "string" }]
            }
            """,
            $$"""
            {{referenceKeyword}}: '{{referenceValue}}'
            type: array
            items:
              - type: string
            """,
            includeReferenceTarget: true);

        AssertUnsupportedDialectDiagnostic(result);
        var schemas = result.Document.Components.Schemas;
        var reference = Assert.IsType<OpenApiSchemaReference>(schemas["Parent"]);
        Assert.Null(reference.Reference.Items);
        Assert.NotNull(reference.Reference.UnrecognizedKeywords);
        Assert.True(reference.Reference.UnrecognizedKeywords.TryGetValue("items", out var rawKeyword));
        var tupleItems = Assert.IsType<JsonArray>(rawKeyword);
        Assert.Equal("string", Assert.Single(tupleItems)["type"].GetValue<string>());
        Assert.Same(reference.Reference.UnrecognizedKeywords, reference.UnrecognizedKeywords);
        var target = schemas["Target"];
        Assert.Same(target, reference.Target);
        Assert.Equal(JsonSchemaType.Boolean, target.Items.Type);
        AssertNoRawKeyword(target, "items");
    }

    [Theory]
    [InlineData("3.1.1", OpenApiConstants.Json)]
    [InlineData("3.1.1", OpenApiConstants.Yaml)]
    [InlineData("3.2.0", OpenApiConstants.Json)]
    [InlineData("3.2.0", OpenApiConstants.Yaml)]
    public void UnknownParentDoesNotRecoverNumericParseFailureFromKnownChild(string version, string format)
    {
        Assert.Throws<FormatException>(() => ParseSchema(version, format, UnsupportedDialect,
            $$"""
            {
              "type": "object",
              "properties": {
                "validSibling": { "type": "string" },
                "child": {
                  "$schema": "{{KnownDialect}}",
                  "type": "string",
                  "maxLength": "bad"
                }
              }
            }
            """,
            $$"""
            type: object
            properties:
              validSibling:
                type: string
              child:
                $schema: {{KnownDialect}}
                type: string
                maxLength: bad
            """));
    }

    private static void AssertUnsupportedDialectDiagnostic(ReadResult result)
    {
        Assert.NotNull(result.Document);
        Assert.Contains(result.Diagnostic.Errors,
            error => error.Pointer == "#/jsonSchemaDialect" &&
                     error.Message == $"The JSON Schema dialect '{new Uri(UnsupportedDialect)}' is not supported.");
    }

    private static void AssertRawTupleItems(IOpenApiSchema schema, string itemType)
    {
        Assert.Null(schema.Items);
        Assert.NotNull(schema.UnrecognizedKeywords);
        Assert.True(schema.UnrecognizedKeywords.TryGetValue("items", out var rawKeyword));
        var tupleItems = Assert.IsType<JsonArray>(rawKeyword);
        var item = Assert.Single(tupleItems);
        Assert.Equal(itemType, item["type"].GetValue<string>());
    }

    private static void AssertNoRawKeyword(IOpenApiSchema schema, string keyword)
    {
        Assert.False(schema.UnrecognizedKeywords?.ContainsKey(keyword) ?? false);
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
                ? """, "Target": { "$dynamicAnchor": "target", "type": "array", "items": { "type": "boolean" } }"""
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
                ? "    Target:\n      $dynamicAnchor: target\n      type: array\n      items:\n        type: boolean"
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
