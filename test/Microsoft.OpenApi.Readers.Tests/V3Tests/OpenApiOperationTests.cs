// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.References;
using Microsoft.OpenApi.Reader;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V3Tests
{
    public class OpenApiOperationTests
    {
        private const string SampleFolderPath = "V3Tests/Samples/OpenApiOperation/";

        public OpenApiOperationTests()
        {
            OpenApiReaderRegistry.RegisterReader("yaml", new OpenApiYamlReader());
        }

        [Fact]
        public void OperationWithSecurityRequirementShouldReferenceSecurityScheme()
        {
            var result = OpenApiDocument.Load(Path.Combine(SampleFolderPath, "securedOperation.yaml"));

            var securityScheme = result.Document.Paths["/"].Operations[OperationType.Get].Security.First().Keys.First();

            securityScheme.Should().BeEquivalentTo(result.Document.Components.SecuritySchemes.First().Value, 
                options => options.Excluding(x => x.Reference));
        }

        [Fact]
        public void ParseOperationWithParameterWithNoLocationShouldSucceed()
        {
            // Act
            var operation = OpenApiModelFactory.Load<OpenApiOperation>(Path.Combine(SampleFolderPath, "operationWithParameterWithNoLocation.json"), OpenApiSpecVersion.OpenApi3_0, out _);
            var expectedOp = new OpenApiOperation
            {
                Tags =
                {
                    new OpenApiTagReference("user", null)
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
                            Type = JsonSchemaType.String
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
                            Type = JsonSchemaType.String
                        }
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
