// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using Microsoft.OpenApi.Reader;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.OpenApiReaderTests;

public class JsonSchemaDialectReferenceKeywordRegressionTests
{
    private const string UnsupportedDialect = "https://example.com/custom-dialect";
    private const string KnownDialect = "https://json-schema.org/draft/2020-12/schema";
    private const string ItemName = "Item/~";
    private const string ChildName = "child/~";
    private static readonly string[] InvalidValues = ["{ \"custom\": true }", "[{ \"custom\": true }]", "true", "42", "null"];

    public static IEnumerable<object[]> MalformedKeywordCases()
    {
        foreach (var version in new[] { "3.1.1", "3.2.0" })
            foreach (var format in new[] { OpenApiConstants.Json, OpenApiConstants.Yaml })
                foreach (var keyword in new[] { "$ref", "$dynamicRef", "$id" })
                    foreach (var value in InvalidValues)
                        foreach (var scope in new[] { "document", "declared", "inherited" })
                        {
                            yield return [version, format, keyword, value, scope];
                        }
    }

    [Theory]
    [MemberData(nameof(MalformedKeywordCases))]
    public void UnsupportedDialectPreservesMalformedReferenceKeywordsAndSiblings(
        string version, string format, string keyword, string value, string scope)
    {
        var result = Parse(version, format, keyword, value, scope);

        var schema = Assert.IsType<OpenApiSchema>(GetItem(result, scope));
        Assert.Equal("kept", schema.Description);
        Assert.Equal(JsonSchemaType.Object, schema.Type);
        Assert.Equal(JsonSchemaType.String, schema.Properties["value"].Type);
        AssertRawKeyword(schema, keyword, value);
        Assert.Contains(result.Diagnostic.Errors, error => error.Message.StartsWith("The JSON Schema dialect ", StringComparison.Ordinal));
        Assert.Contains(result.Diagnostic.Errors, error => error.Pointer == GetItemPointer(scope) + "/" + keyword);
        Assert.Equal(JsonSchemaType.Integer, result.Document.Components.Schemas["Unaffected"].Type);
        if (scope == "inherited")
        {
            Assert.Equal(JsonSchemaType.Boolean, result.Document.Components.Schemas[ItemName].Properties["unchanged"].Type);
        }
    }

    public static IEnumerable<object[]> ReferenceMetadataCases()
    {
        foreach (var version in new[] { "3.1.1", "3.2.0" })
            foreach (var format in new[] { OpenApiConstants.Json, OpenApiConstants.Yaml })
                foreach (var value in InvalidValues)
                {
                    yield return [version, format, "$ref", "$id", value];
                    yield return [version, format, "$ref", "$dynamicRef", value];
                    yield return [version, format, "$dynamicRef", "$id", value];
                }
    }

    [Theory]
    [MemberData(nameof(ReferenceMetadataCases))]
    public void MalformedReferenceMetadataDoesNotDiscardTheValidReference(
        string version, string format, string referenceKeyword, string malformedKeyword, string value)
    {
        var reference = referenceKeyword == "$ref" ? "#/components/schemas/Target" : "#target";
        var result = Parse(version, format, malformedKeyword, value, "document",
            $", \"{referenceKeyword}\": \"{reference}\"", $"\n{referenceKeyword}: '{reference}'");

        var schema = Assert.IsType<OpenApiSchemaReference>(GetItem(result, "document"));
        Assert.Same(result.Document.Components.Schemas["Target"], schema.Target);
        Assert.Equal("kept", schema.Reference.Description);
        Assert.Equal(JsonSchemaType.Object, schema.Reference.SchemaType);
        AssertRawKeyword(schema, malformedKeyword, value);
        Assert.Contains(result.Diagnostic.Errors, error => error.Pointer == GetItemPointer("document") + "/" + malformedKeyword);
        Assert.Equal("target", schema.Target.Description);
        Assert.False(schema.Target.UnrecognizedKeywords?.ContainsKey(malformedKeyword) ?? false);
    }

    public static IEnumerable<object[]> ReferencePrecedenceCases()
    {
        foreach (var version in new[] { "3.1.1", "3.2.0" })
            foreach (var format in new[] { OpenApiConstants.Json, OpenApiConstants.Yaml })
                foreach (var value in InvalidValues)
                {
                    yield return [version, format, value];
                }
    }

    [Theory]
    [MemberData(nameof(ReferencePrecedenceCases))]
    public void MalformedDirectReferenceDoesNotActivateItsDynamicReferenceSibling(string version, string format, string value)
    {
        var result = Parse(version, format, "$ref", value, "document",
            ", \"$dynamicRef\": \"#target\"", "\n$dynamicRef: '#target'");

        var schema = Assert.IsType<OpenApiSchema>(GetItem(result, "document"));
        Assert.Equal("#target", schema.DynamicRef);
        AssertRawKeyword(schema, "$ref", value);
        Assert.Contains(result.Diagnostic.Errors, error => error.Pointer == GetItemPointer("document") + "/$ref");
    }

    [Theory]
    [InlineData("3.1.1", OpenApiConstants.Json)]
    [InlineData("3.1.1", OpenApiConstants.Yaml)]
    [InlineData("3.2.0", OpenApiConstants.Json)]
    [InlineData("3.2.0", OpenApiConstants.Yaml)]
    public void UnsupportedDialectPreservesMalformedReferenceFormatBeforeReferenceConstruction(string version, string format)
    {
        const string reference = "https://example.com/schema#anchor/path";
        var result = Parse(version, format, "$ref", $"\"{reference}\"", "document");

        var schema = Assert.IsType<OpenApiSchema>(GetItem(result, "document"));
        Assert.Equal("kept", schema.Description);
        AssertRawKeyword(schema, "$ref", $"\"{reference}\"");
        Assert.Contains(result.Diagnostic.Errors, error => error.Pointer == GetItemPointer("document") + "/$ref");
        Assert.Equal(JsonSchemaType.Integer, result.Document.Components.Schemas["Unaffected"].Type);
    }

    public static IEnumerable<object[]> KnownDialectCases()
    {
        foreach (var version in new[] { "3.1.1", "3.2.0" })
            foreach (var format in new[] { OpenApiConstants.Json, OpenApiConstants.Yaml })
                foreach (var keyword in new[] { "$ref", "$dynamicRef", "$id" })
                    foreach (var value in new[] { InvalidValues[0], InvalidValues[1] })
                        foreach (var scope in new[] { "known", "knownChild" })
                        {
                            yield return [version, format, keyword, value, scope];
                        }
    }

    [Theory]
    [MemberData(nameof(KnownDialectCases))]
    public void KnownDialectKeepsItsExistingMalformedKeywordBehavior(
        string version, string format, string keyword, string value, string scope)
    {
        var result = Parse(version, format, keyword, value, scope);

        Assert.Contains(result.Diagnostic.Errors, error => error.Message == "Expected scalar value.");
        if (scope == "known")
        {
            Assert.False(result.Document.Components.Schemas?.ContainsKey(ItemName) ?? false);
            Assert.DoesNotContain(result.Diagnostic.Errors, error => error.Message.StartsWith("The JSON Schema dialect ", StringComparison.Ordinal));
        }
        else
        {
            var parent = result.Document.Components.Schemas[ItemName];
            Assert.Null(parent.Properties);
            Assert.Equal(JsonSchemaType.Integer, result.Document.Components.Schemas["Unaffected"].Type);
        }
    }

    [Theory]
    [InlineData("3.1.1", OpenApiConstants.Json)]
    [InlineData("3.1.1", OpenApiConstants.Yaml)]
    [InlineData("3.2.0", OpenApiConstants.Json)]
    [InlineData("3.2.0", OpenApiConstants.Yaml)]
    public void ValidReferenceStringsKeepDirectReferencePrecedenceAndIdentifierMetadata(string version, string format)
    {
        const string identifier = "https://example.com/item";
        var result = Parse(version, format, "$ref", "\"#/components/schemas/Target\"", "document",
            $", \"$dynamicRef\": \"#target\", \"$id\": \"{identifier}\"",
            $"\n$dynamicRef: '#target'\n$id: '{identifier}'");

        var schema = Assert.IsType<OpenApiSchemaReference>(GetItem(result, "document"));
        Assert.Same(result.Document.Components.Schemas["Target"], schema.Target);
        Assert.Equal("Target", schema.Reference.Id);
        Assert.Equal("#target", schema.Reference.DynamicRef);
        Assert.Equal(identifier, schema.Reference.SchemaId);
        Assert.DoesNotContain(result.Diagnostic.Errors, error => error.Pointer == GetItemPointer("document") + "/$ref");
    }

    [Theory]
    [InlineData("3.1.1", OpenApiConstants.Json)]
    [InlineData("3.1.1", OpenApiConstants.Yaml)]
    [InlineData("3.2.0", OpenApiConstants.Json)]
    [InlineData("3.2.0", OpenApiConstants.Yaml)]
    public void IdentifierStringRegistrationDoesNotDiscardFallbackSchema(string version, string format)
    {
        var result = Parse(version, format, "$id", "\"http://[\"", "document");

        var schema = Assert.IsType<OpenApiSchema>(GetItem(result, "document"));
        Assert.Equal("http://[", schema.Id);
        Assert.Equal("kept", schema.Description);
        Assert.Equal(JsonSchemaType.Integer, result.Document.Components.Schemas["Unaffected"].Type);
    }

    private static IOpenApiSchema GetItem(ReadResult result, string scope)
    {
        Assert.NotNull(result.Document);
        var root = result.Document.Components.Schemas[ItemName];
        return scope is "inherited" or "knownChild" ? root.Properties[ChildName] : root;
    }

    private static string GetItemPointer(string scope) =>
        "#/components/schemas/Item~1~0" + (scope is "inherited" or "knownChild" ? "/properties/child~1~0" : string.Empty);

    private static void AssertRawKeyword(IOpenApiSchema schema, string keyword, string value)
    {
        Assert.NotNull(schema.UnrecognizedKeywords);
        Assert.True(schema.UnrecognizedKeywords.TryGetValue(keyword, out var raw));
        if (value == "null")
        {
            Assert.True(raw.IsJsonNullSentinel());
        }
        else
        {
            Assert.True(JsonNode.DeepEquals(JsonNode.Parse(value), raw));
        }
    }

    private static ReadResult Parse(
        string version, string format, string keyword, string value, string scope,
        string extraJson = "", string extraYaml = "")
    {
        var schemaDeclaration = scope is "declared" ? UnsupportedDialect : scope is "knownChild" ? KnownDialect : null;
        var jsonDeclaration = schemaDeclaration is null ? string.Empty : $"\"$schema\": \"{schemaDeclaration}\",";
        var yamlDeclaration = schemaDeclaration is null ? string.Empty : $"$schema: {schemaDeclaration}\n";
        var jsonSchema = $$"""
            { {{jsonDeclaration}} "description": "kept", "type": "object", "properties": { "value": { "type": "string" } }, "{{keyword}}": {{value}}{{extraJson}} }
            """;
        var yamlSchema = $$"""
            {{yamlDeclaration}}description: kept
            type: object
            properties:
              value:
                type: string
            {{keyword}}: {{value}}{{extraYaml}}
            """;
        if (scope is "inherited" or "knownChild")
        {
            jsonSchema = $$"""
                { "$schema": "{{UnsupportedDialect}}", "type": "object", "properties": { "{{ChildName}}": {{jsonSchema}}, "unchanged": { "type": "boolean" } } }
                """;
            yamlSchema = $$"""
                $schema: {{UnsupportedDialect}}
                type: object
                properties:
                  '{{ChildName}}':
                {{Indent(yamlSchema, 4)}}
                  unchanged:
                    type: boolean
                """;
        }

        var documentDialect = scope == "document" ? UnsupportedDialect : KnownDialect;
        var settings = new OpenApiReaderSettings();
        string document;
        if (format == OpenApiConstants.Json)
        {
            document = $$"""
                {
                  "openapi": "{{version}}", "info": { "title": "test", "version": "1.0.0" },
                  "jsonSchemaDialect": "{{documentDialect}}",
                  "components": { "schemas": {
                    "{{ItemName}}": {{jsonSchema}},
                    "Unaffected": { "type": "integer" },
                    "Target": { "$dynamicAnchor": "target", "type": "string", "description": "target" }
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
                jsonSchemaDialect: {{documentDialect}}
                components:
                  schemas:
                    '{{ItemName}}':
                {{Indent(yamlSchema, 6)}}
                    Unaffected:
                      type: integer
                    Target:
                      $dynamicAnchor: target
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
