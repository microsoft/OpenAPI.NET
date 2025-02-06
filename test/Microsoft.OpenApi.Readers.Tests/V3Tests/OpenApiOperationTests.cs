// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
        public async Task OperationWithSecurityRequirementShouldReferenceSecurityScheme()
        {
            var result = await OpenApiDocument.LoadAsync(Path.Combine(SampleFolderPath, "securedOperation.yaml"));

            var securityScheme = result.Document.Paths["/"].Operations[OperationType.Get].Security[0].Keys.First();
            Assert.Equivalent(result.Document.Components.SecuritySchemes.First().Value, securityScheme);
        }

        [Fact]
        public async Task ParseOperationWithParameterWithNoLocationShouldSucceed()
        {
            var openApiDocument = new OpenApiDocument
            {
                Tags = { new OpenApiTag() { Name = "user" } }
            };
            // Act
            var operation = await OpenApiModelFactory.LoadAsync<OpenApiOperation>(Path.Combine(SampleFolderPath, "operationWithParameterWithNoLocation.json"), OpenApiSpecVersion.OpenApi3_0, openApiDocument);
            var expectedOp = new OpenApiOperation
            {
                Tags =
                {
                    new OpenApiTagReference("user", openApiDocument)
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
                        Schema = new OpenApiSchema()
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
                        Schema = new OpenApiSchema()
                        {
                            Type = JsonSchemaType.String
                        }
                    }
                }
            };

            // Assert
            expectedOp.Should().BeEquivalentTo(operation, 
                options => 
                options.Excluding(x => x.Tags[0].Reference.HostDocument)
                        .Excluding(x => x.Tags[0].Reference)
                        .Excluding(x => x.Tags[0].Target)
                        .Excluding(x => x.Tags[0].Extensions));
        }
    }
}
