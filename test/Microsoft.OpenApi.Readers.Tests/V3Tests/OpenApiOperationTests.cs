// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.OpenApi.Tests;
using Microsoft.OpenApi.Reader;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V3Tests
{
    public class OpenApiOperationTests
    {
        private const string SampleFolderPath = "V3Tests/Samples/OpenApiOperation/";

        [Fact]
        public async Task OperationWithSecurityRequirementShouldReferenceSecurityScheme()
        {
            var result = await OpenApiDocument.LoadAsync(Path.Join(SampleFolderPath, "securedOperation.yaml"), SettingsFixture.ReaderSettings, token: TestContext.Current.CancellationToken);

            var securityScheme = result.Document.Paths["/"].Operations[HttpMethod.Get].Security[0].Keys.First();
            Assert.Equivalent(result.Document.Components.SecuritySchemes.First().Value, securityScheme);
        }

        [Fact]
        public async Task ParseOperationWithParameterWithNoLocationShouldSucceed()
        {
            var openApiDocument = new OpenApiDocument
            {
                Tags = new HashSet<OpenApiTag> { new() { Name = "user" } }
            };
            // Act
            var operation = await OpenApiModelFactory.LoadAsync<OpenApiOperation>(Path.Join(SampleFolderPath, "operationWithParameterWithNoLocation.json"), OpenApiSpecVersion.OpenApi3_0, openApiDocument, SettingsFixture.ReaderSettings, token: TestContext.Current.CancellationToken);
            var expectedOp = new OpenApiOperation
            {
                Tags = new HashSet<OpenApiTagReference>()
                {
                    new OpenApiTagReference("user", openApiDocument)
                },
                Summary = "Logs user into the system",
                Description = "",
                OperationId = "loginUser",
                Parameters =
                [
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
                ]
            };

            // Assert
            OpenApiTestAssert.Equivalent(expectedOp, operation, nameof(OpenApiOperation.Tags));
        }
        [Fact]
        public void DeduplicatesTagReferences()
        {

            var openApiDocument = new OpenApiDocument
            {
                Tags = new HashSet<OpenApiTag> { new() { Name = "user" } }
            };
            // Act
            var expectedOp = new OpenApiOperation
            {
                Tags = new HashSet<OpenApiTagReference>()
                {
                    new OpenApiTagReference("user", openApiDocument),
                    new OpenApiTagReference("user", openApiDocument),
                },
                Summary = "Logs user into the system",
                Description = "",
                OperationId = "loginUser",
                Parameters =
                [
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
                ]
            };
            using var textWriter = new StringWriter();
            var writer = new OpenApiJsonWriter(textWriter);
            expectedOp.SerializeAsV3(writer);
            var result = textWriter.ToString();
            var parsedJson = JsonNode.Parse(result);
            var operationObject = Assert.IsType<JsonObject>(parsedJson);
            var tags = Assert.IsType<JsonArray>(operationObject["tags"]);
            Assert.Single(tags);
        }
    }
}
