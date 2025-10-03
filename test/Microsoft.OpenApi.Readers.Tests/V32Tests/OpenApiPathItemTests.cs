// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Tests;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V32Tests
{
    public class OpenApiPathItemTests
    {
        private const string SampleFolderPath = "V32Tests/Samples/OpenApiPathItem/";

        [Fact]
        public async Task ParsePathItemWithQueryAndAdditionalOperationsV32Works()
        {
            // Arrange & Act
            var result = await OpenApiDocument.LoadAsync($"{SampleFolderPath}pathItemWithQueryAndAdditionalOperations.yaml", SettingsFixture.ReaderSettings);
            var pathItem = result.Document.Paths["/pets"];

            // Assert
            Assert.Equal("Pet operations", pathItem.Summary);
            Assert.Equal("Operations available for pets", pathItem.Description);
            
            // Regular operations
            Assert.True(pathItem.Operations?.ContainsKey(HttpMethod.Get));
            var getOp = pathItem.Operations[HttpMethod.Get];
            Assert.Equal("getPets", getOp.OperationId);
            
            Assert.True(pathItem.Operations?.ContainsKey(HttpMethod.Post));
            var postOp = pathItem.Operations[HttpMethod.Post];
            Assert.Equal("createPet", postOp.OperationId);

            // Query operation should now be on one of the operations
            // Since the YAML structure changed, we need to check which operation has the query
            Assert.NotNull(getOp.Query);
            Assert.Equal("Query pets with complex filters", getOp.Query.Summary);
            Assert.Equal("queryPets", getOp.Query.OperationId);
            Assert.Single(getOp.Query.Parameters);
            Assert.Equal("filter", getOp.Query.Parameters[0].Name);

            // Additional operations should now be on one of the operations
            Assert.NotNull(getOp.AdditionalOperations);
            Assert.Equal(2, getOp.AdditionalOperations.Count);
            
            Assert.True(getOp.AdditionalOperations.ContainsKey("notify"));
            var notifyOp = getOp.AdditionalOperations["notify"];
            Assert.Equal("Notify about pet updates", notifyOp.Summary);
            Assert.Equal("notifyPetUpdates", notifyOp.OperationId);
            Assert.NotNull(notifyOp.RequestBody);
            
            Assert.True(getOp.AdditionalOperations.ContainsKey("subscribe"));
            var subscribeOp = getOp.AdditionalOperations["subscribe"];
            Assert.Equal("Subscribe to pet events", subscribeOp.Summary);
            Assert.Equal("subscribePetEvents", subscribeOp.OperationId);
            Assert.Single(subscribeOp.Parameters);
            Assert.Equal("events", subscribeOp.Parameters[0].Name);
        }

        [Fact]
        public async Task ParsePathItemWithV32ExtensionsWorks()
        {
            // Arrange & Act
            var result = await OpenApiDocument.LoadAsync($"{SampleFolderPath}pathItemWithV32Extensions.yaml", SettingsFixture.ReaderSettings);
            var pathItem = result.Document.Paths["/pets"];

            // Assert
            Assert.Equal("Pet operations", pathItem.Summary);
            Assert.Equal("Operations available for pets", pathItem.Description);

            // Regular operations
            Assert.True(pathItem.Operations?.ContainsKey(HttpMethod.Get));
            var getOp = pathItem.Operations[HttpMethod.Get];
            Assert.Equal("getPets", getOp.OperationId);

            // Query operation from extension should now be on the operation
            Assert.NotNull(getOp.Query);
            Assert.Equal("Query pets with complex filters", getOp.Query.Summary);
            Assert.Equal("queryPets", getOp.Query.OperationId);

            // Additional operations from extension should now be on the operation
            Assert.NotNull(getOp.AdditionalOperations);
            Assert.Single(getOp.AdditionalOperations);
            Assert.True(getOp.AdditionalOperations.ContainsKey("notify"));
            var notifyOp = getOp.AdditionalOperations["notify"];
            Assert.Equal("Notify about pet updates", notifyOp.Summary);
            Assert.Equal("notifyPetUpdates", notifyOp.OperationId);
        }

        [Fact]
        public async Task SerializeV32PathItemToV31ProducesExtensions()
        {
            // Arrange
            var pathItem = new OpenApiPathItem
            {
                Summary = "Test path",
                Operations = new Dictionary<HttpMethod, OpenApiOperation>
                {
                    [HttpMethod.Get] = new OpenApiOperation
                    {
                        Summary = "Get operation",
                        OperationId = "getOp",
                        Query = new OpenApiOperation
                        {
                            Summary = "Query operation",
                            OperationId = "queryOp",
                            Responses = new OpenApiResponses
                            {
                                ["200"] = new OpenApiResponse { Description = "Success" }
                            }
                        },
                        AdditionalOperations = new Dictionary<string, OpenApiOperation>
                        {
                            ["notify"] = new OpenApiOperation
                            {
                                Summary = "Notify operation", 
                                OperationId = "notifyOp",
                                Responses = new OpenApiResponses
                                {
                                    ["200"] = new OpenApiResponse { Description = "Success" }
                                }
                            }
                        },
                        Responses = new OpenApiResponses
                        {
                            ["200"] = new OpenApiResponse { Description = "Success" }
                        }
                    }
                }
            };

            // Act
            var yaml = await pathItem.SerializeAsYamlAsync(OpenApiSpecVersion.OpenApi3_1);

            // Assert
            Assert.Contains("x-oas-query:", yaml);
            Assert.Contains("x-oas-additionalOperations:", yaml);
            Assert.Contains("queryOp", yaml);
            Assert.Contains("notifyOp", yaml);
        }

        [Fact]
        public async Task SerializeV32PathItemToV3ProducesExtensions()
        {
            // Arrange
            var pathItem = new OpenApiPathItem
            {
                Summary = "Test path",
                Operations = new Dictionary<HttpMethod, OpenApiOperation>
                {
                    [HttpMethod.Get] = new OpenApiOperation
                    {
                        Summary = "Get operation",
                        OperationId = "getOp",
                        Query = new OpenApiOperation
                        {
                            Summary = "Query operation",
                            OperationId = "queryOp",
                            Responses = new OpenApiResponses
                            {
                                ["200"] = new OpenApiResponse { Description = "Success" }
                            }
                        },
                        AdditionalOperations = new Dictionary<string, OpenApiOperation>
                        {
                            ["notify"] = new OpenApiOperation
                            {
                                Summary = "Notify operation",
                                OperationId = "notifyOp", 
                                Responses = new OpenApiResponses
                                {
                                    ["200"] = new OpenApiResponse { Description = "Success" }
                                }
                            }
                        },
                        Responses = new OpenApiResponses
                        {
                            ["200"] = new OpenApiResponse { Description = "Success" }
                        }
                    }
                }
            };

            // Act
            var yaml = await pathItem.SerializeAsYamlAsync(OpenApiSpecVersion.OpenApi3_0);

            // Assert
            Assert.Contains("x-oas-query:", yaml);
            Assert.Contains("x-oas-additionalOperations:", yaml);
            Assert.Contains("queryOp", yaml);
            Assert.Contains("notifyOp", yaml);
        }

        [Fact]
        public void PathItemShallowCopyIncludesV32Fields()
        {
            // Arrange
            var original = new OpenApiPathItem
            {
                Summary = "Original",
                Operations = new Dictionary<HttpMethod, OpenApiOperation>
                {
                    [HttpMethod.Get] = new OpenApiOperation
                    {
                        Summary = "Get operation",
                        OperationId = "getOp",
                        Query = new OpenApiOperation
                        {
                            Summary = "Query operation",
                            OperationId = "queryOp"
                        },
                        AdditionalOperations = new Dictionary<string, OpenApiOperation>
                        {
                            ["notify"] = new OpenApiOperation
                            {
                                Summary = "Notify operation",
                                OperationId = "notifyOp"
                            }
                        },
                        Responses = new OpenApiResponses()
                    }
                }
            };

            // Act
            var copy = original.CreateShallowCopy();

            // Assert
            var originalGetOp = original.Operations![HttpMethod.Get];
            var copyGetOp = copy.Operations![HttpMethod.Get];
            
            Assert.NotNull(copyGetOp.Query);
            Assert.Equal("Query operation", copyGetOp.Query.Summary);
            Assert.Equal("queryOp", copyGetOp.Query.OperationId);

            Assert.NotNull(copyGetOp.AdditionalOperations);
            Assert.Single(copyGetOp.AdditionalOperations);
            Assert.Equal("Notify operation", copyGetOp.AdditionalOperations["notify"].Summary);
            Assert.Equal("notifyOp", copyGetOp.AdditionalOperations["notify"].OperationId);
        }
    }
}
