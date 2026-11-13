// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Reader;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.OpenApiReaderTests;

public class JsonSchemaDialectArrayLocationRegressionTests
{
    private const string UnsupportedDialect = "https://example.com/custom-dialect";
    private const string KnownDialect = "https://json-schema.org/draft/2020-12/schema";
    private const string ItemPointer = "#/components/schemas/Item";

    public static IEnumerable<object[]> CompositionCases()
    {
        foreach (var version in new[] { "3.1.1", "3.2.0" })
            foreach (var format in new[] { OpenApiConstants.Json, OpenApiConstants.Yaml })
                foreach (var keyword in new[] { "allOf", "anyOf", "oneOf" })
                {
                    yield return [version, format, keyword];
                }
    }

    [Theory]
    [MemberData(nameof(CompositionCases))]
    public void CompositionDeclarationsHaveDistinctArrayElementLocations(string version, string format, string keyword)
    {
        var result = Parse(version, format,
            $$"""
            { "{{keyword}}": [
              { "$schema": "{{UnsupportedDialect}}/first", "type": "string" },
              { "$schema": "{{UnsupportedDialect}}/second", "type": "integer" }
            ] }
            """,
            $$"""
            {{keyword}}:
              - $schema: {{UnsupportedDialect}}/first
                type: string
              - $schema: {{UnsupportedDialect}}/second
                type: integer
            """);

        AssertErrorPointers(result, $"{ItemPointer}/{keyword}/0", $"{ItemPointer}/{keyword}/1");
        Assert.All(result.Diagnostic.Errors, error => Assert.StartsWith("The JSON Schema dialect ", error.Message));
        var members = GetComposition(result.Document.Components.Schemas["Item"], keyword);
        Assert.Equal(2, members.Count);
        Assert.Equal(JsonSchemaType.String, members[0].Type);
        Assert.Equal(JsonSchemaType.Integer, members[1].Type);
        Assert.Equal(JsonSchemaType.Boolean, result.Document.Components.Schemas["Unaffected"].Type);
    }

    [Theory]
    [MemberData(nameof(CompositionCases))]
    public void SkippedCompositionValuesDoNotCompactDiagnosticIndices(string version, string format, string keyword)
    {
        var result = Parse(version, format,
            $$"""
            { "{{keyword}}": [null, true,
              { "$schema": "{{UnsupportedDialect}}/first", "type": "string" }, 42,
              { "$schema": "{{UnsupportedDialect}}/second", "type": "integer" }
            ] }
            """,
            $$"""
            {{keyword}}:
              - null
              - true
              - $schema: {{UnsupportedDialect}}/first
                type: string
              - 42
              - $schema: {{UnsupportedDialect}}/second
                type: integer
            """);

        AssertErrorPointers(result, $"{ItemPointer}/{keyword}/2", $"{ItemPointer}/{keyword}/4");
        var members = GetComposition(result.Document.Components.Schemas["Item"], keyword);
        Assert.Equal(2, members.Count);
        Assert.Equal(JsonSchemaType.String, members[0].Type);
        Assert.Equal(JsonSchemaType.Integer, members[1].Type);
    }

    [Theory]
    [InlineData("3.1.1", OpenApiConstants.Json)]
    [InlineData("3.1.1", OpenApiConstants.Yaml)]
    [InlineData("3.2.0", OpenApiConstants.Json)]
    [InlineData("3.2.0", OpenApiConstants.Yaml)]
    public void NestedCompositionsRestoreTheLocationBeforeFollowingSiblingSchemas(string version, string format)
    {
        var result = Parse(version, format,
            $$"""
            {
              "allOf": [
                { "anyOf": [
                  { "$schema": "{{UnsupportedDialect}}/first", "type": "string" },
                  { "$schema": "{{UnsupportedDialect}}/second", "type": "integer" }
                ] },
                { "oneOf": [{}, { "$schema": "{{UnsupportedDialect}}/third", "type": "number" }] }
              ],
              "properties": { "after/~": { "$schema": "{{UnsupportedDialect}}/fourth", "type": "boolean" } }
            }
            """,
            $$"""
            allOf:
              - anyOf:
                  - $schema: {{UnsupportedDialect}}/first
                    type: string
                  - $schema: {{UnsupportedDialect}}/second
                    type: integer
              - oneOf:
                  - {}
                  - $schema: {{UnsupportedDialect}}/third
                    type: number
            properties:
              'after/~':
                $schema: {{UnsupportedDialect}}/fourth
                type: boolean
            """);

        AssertErrorPointers(result,
            $"{ItemPointer}/allOf/0/anyOf/0",
            $"{ItemPointer}/allOf/0/anyOf/1",
            $"{ItemPointer}/allOf/1/oneOf/1",
            $"{ItemPointer}/properties/after~1~0");
        var schema = result.Document.Components.Schemas["Item"];
        Assert.Equal(JsonSchemaType.String, schema.AllOf[0].AnyOf[0].Type);
        Assert.Equal(JsonSchemaType.Integer, schema.AllOf[0].AnyOf[1].Type);
        Assert.Equal(JsonSchemaType.Number, schema.AllOf[1].OneOf[1].Type);
        Assert.Equal(JsonSchemaType.Boolean, schema.Properties["after/~"].Type);
        Assert.Equal(JsonSchemaType.Boolean, result.Document.Components.Schemas["Unaffected"].Type);
    }

    [Theory]
    [MemberData(nameof(CompositionCases))]
    public void FallbackKeywordDiagnosticsIncludeTheirCompositionElementIndex(string version, string format, string keyword)
    {
        var result = Parse(version, format,
            $$"""
            {
              "$schema": "{{UnsupportedDialect}}",
              "{{keyword}}": [
                { "type": "string", "minLength": "first" },
                { "type": "string", "minLength": "second" }
              ],
              "properties": { "after/~": { "type": "string", "minLength": "after" } }
            }
            """,
            $$"""
            $schema: {{UnsupportedDialect}}
            {{keyword}}:
              - type: string
                minLength: first
              - type: string
                minLength: second
            properties:
              'after/~':
                type: string
                minLength: after
            """);

        AssertErrorPointers(result, ItemPointer,
            $"{ItemPointer}/{keyword}/0/minLength",
            $"{ItemPointer}/{keyword}/1/minLength",
            $"{ItemPointer}/properties/after~1~0/minLength");
        Assert.Single(result.Diagnostic.Errors,
            error => error.Message.StartsWith("The JSON Schema dialect ", StringComparison.Ordinal));
        var schema = result.Document.Components.Schemas["Item"];
        var members = GetComposition(schema, keyword);
        Assert.Equal(2, members.Count);
        Assert.Equal("first", members[0].UnrecognizedKeywords["minLength"].GetValue<string>());
        Assert.Equal("second", members[1].UnrecognizedKeywords["minLength"].GetValue<string>());
        Assert.Equal("after", schema.Properties["after/~"].UnrecognizedKeywords["minLength"].GetValue<string>());
        Assert.Equal(JsonSchemaType.Boolean, result.Document.Components.Schemas["Unaffected"].Type);
    }

    [Theory]
    [MemberData(nameof(CompositionCases))]
    public void ReferenceMetadataDiagnosticsIncludeTheirCompositionElementIndex(string version, string format, string keyword)
    {
        var result = Parse(version, format,
            $$"""
            { "{{keyword}}": [
              { "type": "integer" },
              { "$schema": "{{UnsupportedDialect}}", "$ref": "#/components/schemas/Target", "description": "metadata", "minLength": "custom" }
            ] }
            """,
            $$"""
            {{keyword}}:
              - type: integer
              - $schema: {{UnsupportedDialect}}
                $ref: '#/components/schemas/Target'
                description: metadata
                minLength: custom
            """);

        AssertErrorPointers(result, $"{ItemPointer}/{keyword}/1", $"{ItemPointer}/{keyword}/1/minLength");
        var members = GetComposition(result.Document.Components.Schemas["Item"], keyword);
        var reference = Assert.IsType<OpenApiSchemaReference>(members[1]);
        Assert.Same(result.Document.Components.Schemas["Target"], reference.Target);
        Assert.Equal("metadata", reference.Reference.Description);
        Assert.Equal("custom", reference.UnrecognizedKeywords["minLength"].GetValue<string>());
        Assert.Equal("target", reference.Target.Description);
        Assert.False(reference.Target.UnrecognizedKeywords?.ContainsKey("minLength") ?? false);
        Assert.Equal(JsonSchemaType.Integer, members[0].Type);
    }

    [Theory]
    [MemberData(nameof(CompositionCases))]
    public void KnownDialectCompositionMembersKeepTheirTypedValues(string version, string format, string keyword)
    {
        var result = Parse(version, format,
            $$"""
            { "{{keyword}}": [
              { "$schema": "{{KnownDialect}}", "type": "string", "minLength": 2 },
              { "type": "integer", "minimum": 3 }
            ] }
            """,
            $$"""
            {{keyword}}:
              - $schema: {{KnownDialect}}
                type: string
                minLength: 2
              - type: integer
                minimum: 3
            """);

        Assert.Empty(result.Diagnostic.Errors);
        var members = GetComposition(result.Document.Components.Schemas["Item"], keyword);
        Assert.Equal(2, members.Count);
        Assert.Equal(JsonSchemaType.String, members[0].Type);
        Assert.Equal(2, members[0].MinLength);
        Assert.Equal(JsonSchemaType.Integer, members[1].Type);
        Assert.Equal("3", members[1].Minimum);
    }

    private static IList<IOpenApiSchema> GetComposition(IOpenApiSchema schema, string keyword) => keyword switch
    {
        "allOf" => schema.AllOf,
        "anyOf" => schema.AnyOf,
        "oneOf" => schema.OneOf,
        _ => throw new ArgumentOutOfRangeException(nameof(keyword)),
    };

    private static void AssertErrorPointers(ReadResult result, params string[] pointers)
    {
        Assert.NotNull(result.Document);
        Assert.Equal(pointers, result.Diagnostic.Errors.Select(error => error.Pointer));
    }

    private static ReadResult Parse(string version, string format, string jsonSchema, string yamlSchema)
    {
        var settings = new OpenApiReaderSettings();
        string document;
        if (format == OpenApiConstants.Json)
        {
            document = $$"""
                {
                  "openapi": "{{version}}", "info": { "title": "test", "version": "1.0.0" },
                  "components": { "schemas": {
                    "Item": {{jsonSchema}},
                    "Unaffected": { "type": "boolean" },
                    "Target": { "type": "string", "description": "target" }
                  } }
                }
                """;
        }
        else
        {
            settings.AddYamlReader();
            document = $$"""
                openapi: {{version}}
                info:
                  title: test
                  version: 1.0.0
                components:
                  schemas:
                    Item:
                {{Indent(yamlSchema, 6)}}
                    Unaffected:
                      type: boolean
                    Target:
                      type: string
                      description: target
                """;
        }

        return OpenApiDocument.Parse(document, format, settings);
    }

    private static string Indent(string value, int spaces)
    {
        var indentation = new string(' ', spaces);
        return indentation + value.Replace("\n", "\n" + indentation);
    }
}
