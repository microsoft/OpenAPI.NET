// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.OpenApi.Reader;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.OpenApiReaderTests;

public class JsonSchemaDialectFragmentRegressionTests
{
    private const string MetaSchemaUri = "https://example.com/meta";
    private const string KnownDialect = "https://json-schema.org/draft/2020-12/schema";
    private const string UnknownDialect = "https://example.com/unsupported";

    public static IEnumerable<object[]> FragmentCases()
    {
        (string Fragment, string Schema, bool Supported)[] cases =
        [
            ("#missing", $$"""{ "$schema": "{{KnownDialect}}" }""", false),
            ("#/$defs/missing", $$"""{ "$schema": "{{KnownDialect}}", "$defs": {} }""", false),
            ("#/$defs/selected", $$"""{ "$schema": "{{KnownDialect}}", "$defs": { "selected": {} } }""", true),
            ("#selected", $$"""{ "$schema": "{{KnownDialect}}", "$defs": { "selected": { "$anchor": "selected" } } }""", true),
            ("#selected", $$"""{ "$schema": "{{KnownDialect}}", "$defs": { "selected": { "$dynamicAnchor": "selected" } } }""", true),
            ("#/$defs/selected", $$"""{ "$schema": "{{KnownDialect}}", "$defs": { "selected": { "$id": "child", "$schema": "{{UnknownDialect}}" } } }""", false),
            ("#/$defs/selected", $$"""{ "$schema": "{{UnknownDialect}}", "$defs": { "selected": { "$id": "child", "$schema": "{{KnownDialect}}" } } }""", true),
            ("#selected", $$"""{ "$defs": { "selected": { "$anchor": "selected", "$schema": "{{KnownDialect}}" } } }""", true),
            ("#/$defs/selected", $$"""{ "$schema": "{{KnownDialect}}", "$defs": { "selected": true } }""", true),
            ("#selected", $$"""{ "$schema": "{{KnownDialect}}", "examples": [{ "$anchor": "selected" }] }""", false),
            ("#selected", $$"""{ "$schema": "{{KnownDialect}}", "x-custom": { "$anchor": "selected" } }""", false),
            ("#/examples/0", $$"""{ "$schema": "{{KnownDialect}}", "examples": [{ "$schema": "{{KnownDialect}}" }] }""", false),
            ("#/$defs/selected", $$"""{ "$schema": "{{KnownDialect}}", "$defs": { "selected": "not a schema" } }""", false),
            ("#selected", $$"""{ "$schema": "{{KnownDialect}}", "$defs": { "a": { "$anchor": "selected" }, "b": { "$anchor": "selected" } } }""", false),
            ("#selected", $$"""{ "$schema": "{{KnownDialect}}", "$defs": { "child": { "$id": "other", "$anchor": "selected" } } }""", false),
            ("#selected", $$"""{ "$schema": "{{KnownDialect}}", "$anchor": "selected", "$defs": { "child": { "$id": "other", "$anchor": "selected" } } }""", true),
            ("#%2F$defs%2Ffoo~1bar~0baz", $$"""{ "$defs": { "foo/bar~baz": { "$schema": "{{KnownDialect}}" } } }""", true),
            ("#/$defs/%252F", $$"""{ "$defs": { "%2F": { "$schema": "{{KnownDialect}}" } } }""", true),
            ("#/$defs/", $$"""{ "$defs": { "": { "$schema": "{{KnownDialect}}" } } }""", true),
            ("#/allOf/01", $$"""{ "$schema": "{{KnownDialect}}", "allOf": [{}, {}] }""", false),
            ("#/allOf/-", $$"""{ "$schema": "{{KnownDialect}}", "allOf": [{}] }""", false),
            ("#/allOf/9", $$"""{ "$schema": "{{KnownDialect}}", "allOf": [{}] }""", false),
            ("#/$defs/selected/type/missing", $$"""{ "$schema": "{{KnownDialect}}", "$defs": { "selected": { "type": "string" } } }""", false),
            ("#/$defs/selected", """{ "$schema": 42, "$defs": { "selected": {} } }""", false),
            ("#/$defs/selected", $$"""{ "$schema": "{{KnownDialect}}", "$defs": { "selected": { "$schema": 42 } } }""", false),
            ("#/$defs/selected", $$"""{ "$schema": "{{KnownDialect}}", "$defs": { "selected": { "$schema": "relative" } } }""", false),
            ("#selected", $$"""{ "$schema": "{{KnownDialect}}", "$defs": { "child": { "$id": "", "$anchor": "selected" } } }""", true),
            ("#selected", $$"""{ "$schema": "{{KnownDialect}}", "$defs": { "child": { "$id": "#invalid", "$anchor": "selected" } } }""", false),
            ("#selected", $$"""{ "$schema": "{{KnownDialect}}", "unevaluatedItems": { "$anchor": "selected" } }""", true),
            ("#not%20valid", $$"""{ "$schema": "{{KnownDialect}}", "$anchor": "not valid" }""", false),
            ("#selected", $$"""{ "$schema": "{{KnownDialect}}", "$id": "{{MetaSchemaUri}}#", "$anchor": "selected" }""", true),
            ("#selected", $$"""{ "$schema": "{{KnownDialect}}", "$id": "#", "$anchor": "selected" }""", true),
        ];

        foreach (var version in new[] { "3.1.1", "3.2.0" })
        {
            foreach (var (fragment, schema, supported) in cases)
            {
                yield return [version, fragment, schema, supported];
            }
        }
    }

    [Theory]
    [MemberData(nameof(FragmentCases))]
    public async Task DefaultLoaderResolvesTheSelectedSchemaRatherThanTheResourceRoot(
        string version,
        string fragment,
        string metaSchema,
        bool supported)
    {
        var dialect = MetaSchemaUri + fragment;
        var handler = new MetaSchemaHandler(metaSchema);
        using var client = new HttpClient(handler);
        var settings = new OpenApiReaderSettings
        {
            LoadExternalRefs = true,
            HttpClient = client,
        };
        using var input = CreateDocument(version, dialect);

        var result = await OpenApiDocument.LoadAsync(
            input,
            OpenApiConstants.Json,
            settings,
            TestContext.Current.CancellationToken);

        Assert.NotNull(result.Document);
        if (supported)
        {
            Assert.Empty(result.Diagnostic.Errors);
        }
        else
        {
            var error = Assert.Single(result.Diagnostic.Errors);
            Assert.Equal($"The JSON Schema dialect '{new Uri(dialect)}' is not supported.", error.Message);
            Assert.Equal("#/jsonSchemaDialect", error.Pointer);
        }

        Assert.Equal(1, handler.MetaSchemaRequests);
    }

    private static MemoryStream CreateDocument(string version, string dialect) => new(Encoding.UTF8.GetBytes($$"""
        {
          "openapi": "{{version}}",
          "info": { "title": "test", "version": "1.0.0" },
          "jsonSchemaDialect": "{{dialect}}",
          "components": { "schemas": { "Item": { "type": "string" } } }
        }
        """));

    private sealed class MetaSchemaHandler(string metaSchema) : HttpMessageHandler
    {
        internal int MetaSchemaRequests { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (request.RequestUri.GetLeftPart(UriPartial.Path) == MetaSchemaUri)
            {
                MetaSchemaRequests++;
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(metaSchema, Encoding.UTF8, "application/schema+json"),
                });
            }

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
        }
    }
}
