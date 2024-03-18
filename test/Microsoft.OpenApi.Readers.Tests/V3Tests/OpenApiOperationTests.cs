// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.IO;
using System.Linq;
using FluentAssertions;
using Json.Schema;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;
using Microsoft.OpenApi.Readers.V3;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V3Tests
{
    public class OpenApiOperationTests
    {
        private const string SampleFolderPath = "V3Tests/Samples/OpenApiOperation/";

        [Fact]
        public void OperationWithSecurityRequirementShouldReferenceSecurityScheme()
        {
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "securedOperation.yaml"));
            var openApiDoc = new OpenApiStreamReader().Read(stream, out var diagnostic);

            var securityScheme = openApiDoc.Paths["/"].Operations[OperationType.Get].Security.First().Keys.First();

            securityScheme.Should().BeEquivalentTo(openApiDoc.Components.SecuritySchemes.First().Value, 
                options => options.Excluding(x => x.Reference.HostDocument));
        }

        [Fact]
        public void ParseOperationWithParameterWithNoLocationShouldSucceed()
        {
            // Arrange
            MapNode node;
            using (var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "operationWithParameterWithNoLocation.json")))
            {
                node = TestHelper.CreateYamlMapNode(stream);
            }

            // Act
            var operation = OpenApiV3Deserializer.LoadOperation(node);
            var expectedOp = new OpenApiOperation
            {
                Tags =
                {
                    new OpenApiTag
                    {
                        UnresolvedReference = false,
                        Reference = new()
                        {
                            Id = "user",
                            Type = ReferenceType.Tag
                        }
                    }
                },
                Summary = "Logs user into the system",
                Description = "",
                OperationId = "loginUser",
                Parameters =
                {
                    new OpenApiParameter
                    {
                        Name = "username",
                        Description = "The user name for login",
                        Required = true,
                        Schema = new JsonSchemaBuilder()
                                    .Type(SchemaValueType.String)
                    },
                    new OpenApiParameter
                    {
                        Name = "password",
                        Description = "The password for login in clear text",
                        In = ParameterLocation.Query,
                        Required = true,
                        Schema = new JsonSchemaBuilder()
                                    .Type(SchemaValueType.String)
                    }
                }
            };
            // Assert
            expectedOp.Should().BeEquivalentTo(operation, 
                options => options.Excluding(x => x.Tags[0].Reference.HostDocument)
                .Excluding(x => x.Tags[0].Extensions));
        }
    }
}
