// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OpenApi.Readers.ParseNodes;
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

            var operation = openApiDoc.Paths["/pets"].Operations["get"];
            var schema = operation.Responses["200"].Content["application/json"].Schema;
            Assert.NotNull(schema);
        }

        [Fact]
        public void CreateSchemaFromInlineJsonSchema()
        {
            var jsonSchema = " { \"type\" : \"int\" } ";

            var context = new ParsingContext();
            var diagnostic = new OpenApiDiagnostic();

            var mapNode = new MapNode(context, diagnostic, jsonSchema);

            var schema = OpenApiV3Deserializer.LoadSchema(mapNode);

            Assert.NotNull(schema);
            Assert.Equal("int", schema.Type);
        }
    }
}