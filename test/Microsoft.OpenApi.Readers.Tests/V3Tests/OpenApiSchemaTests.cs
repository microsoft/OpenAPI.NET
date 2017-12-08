// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using FluentAssertions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;
using Microsoft.OpenApi.Readers.V3;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V3Tests
{
    [Collection("DefaultSettings")]
    public class OpenApiSchemaTests
    {
        [Fact]
        public void ParseInlineBasicSchemaShouldSucceed()
        {
            var jsonSchema = @"{ ""type"" : ""integer"" }";

            var context = new ParsingContext();
            var diagnostic = new OpenApiDiagnostic();

            var mapNode = new MapNode(context, diagnostic, jsonSchema);

            var schema = OpenApiV3Deserializer.LoadSchema(mapNode);

            schema.ShouldBeEquivalentTo(
                new OpenApiSchema
                {
                    Type = "integer"
                });
        }
    }
}