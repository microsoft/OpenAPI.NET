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
            Assert.Equal("getPets", pathItem.Operations[HttpMethod.Get].OperationId);
            Assert.True(pathItem.Operations?.ContainsKey(HttpMethod.Post));
            Assert.Equal("createPet", pathItem.Operations[HttpMethod.Post].OperationId);

            // Query operation
            Assert.NotNull(pathItem.Query);
            Assert.Equal("Query pets with complex filters", pathItem.Query.Summary);
            Assert.Equal("queryPets", pathItem.Query.OperationId);
            Assert.Single(pathItem.Query.Parameters);
            Assert.Equal("filter", pathItem.Query.Parameters[0].Name);

            // Additional operations
            Assert.NotNull(pathItem.AdditionalOperations);
            Assert.Equal(2, pathItem.AdditionalOperations.Count);
            
            Assert.True(pathItem.AdditionalOperations.ContainsKey("notify"));
            var notifyOp = pathItem.AdditionalOperations["notify"];
            Assert.Equal("Notify about pet updates", notifyOp.Summary);
            Assert.Equal("notifyPetUpdates", notifyOp.OperationId);
            Assert.NotNull(notifyOp.RequestBody);
            
            Assert.True(pathItem.AdditionalOperations.ContainsKey("subscribe"));
            var subscribeOp = pathItem.AdditionalOperations["subscribe"];
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
            Assert.Equal("getPets", pathItem.Operations[HttpMethod.Get].OperationId);

            // Query operation from extension
            Assert.NotNull(pathItem.Query);
            Assert.Equal("Query pets with complex filters", pathItem.Query.Summary);
            Assert.Equal("queryPets", pathItem.Query.OperationId);

            // Additional operations from extension
            Assert.NotNull(pathItem.AdditionalOperations);
            Assert.Single(pathItem.AdditionalOperations);
            Assert.True(pathItem.AdditionalOperations.ContainsKey("notify"));
            var notifyOp = pathItem.AdditionalOperations["notify"];
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
                }
            };

            // Act
            var copy = original.CreateShallowCopy();

            // Assert
            Assert.NotNull(copy.Query);
            Assert.Equal("Query operation", copy.Query.Summary);
            Assert.Equal("queryOp", copy.Query.OperationId);

            Assert.NotNull(copy.AdditionalOperations);
            Assert.Single(copy.AdditionalOperations);
            Assert.Equal("Notify operation", copy.AdditionalOperations["notify"].Summary);
            Assert.Equal("notifyOp", copy.AdditionalOperations["notify"].OperationId);
        }
    }
}
