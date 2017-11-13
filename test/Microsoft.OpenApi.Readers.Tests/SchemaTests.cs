// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using FluentAssertions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;
using Microsoft.OpenApi.Readers.V3;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests
{
    public class SchemaTests
    {
        [Fact]
        public void CheckPetStoreApiInfo()
        {
            var stream = GetType().Assembly.GetManifestResourceStream(typeof(SchemaTests), "Samples.petstore30.yaml");

            var openApiDoc = new OpenApiStreamReader().Read(stream, out var context);

            var operation = openApiDoc.Paths["/pets"].Operations[OperationType.Get];
            var schema = operation.Responses["200"].Content["application/json"].Schema;
            schema.Should().NotBeNull();
        }

        [Fact]
        public void CreateSchemaFromInlineJsonSchema()
        {
            var jsonSchema = " { \"type\" : \"int\" } ";

            var context = new ParsingContext();
            var diagnostic = new OpenApiDiagnostic();

            var mapNode = new MapNode(context, diagnostic, jsonSchema);

            var schema = OpenApiV3Deserializer.LoadSchema(mapNode);

            schema.Should().NotBeNull();
            schema.Type.Should().Be("int");
        }
    }
}