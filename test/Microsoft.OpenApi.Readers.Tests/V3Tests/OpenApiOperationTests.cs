// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.IO;
using System.Linq;
using FluentAssertions;
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

            var securityRequirement = openApiDoc.Paths["/"].Operations[OperationType.Get].Security.First();

            Assert.Same(securityRequirement.Keys.First(), openApiDoc.Components.SecuritySchemes.First().Value);
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

            // Assert
            operation.Should().BeEquivalentTo(new OpenApiOperation
            {
                Tags =
                {
                    new OpenApiTag
                    {
                        UnresolvedReference = true,
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
                        Schema = new()
                        {
                            Type = "string"
                        }
                    },
                    new OpenApiParameter
                    {
                        Name = "password",
                        Description = "The password for login in clear text",
                        In = ParameterLocation.Query,
                        Required = true,
                        Schema = new()
                        {
                            Type = "string"
                        }
                    }
                }
            });
        }
    }
}
