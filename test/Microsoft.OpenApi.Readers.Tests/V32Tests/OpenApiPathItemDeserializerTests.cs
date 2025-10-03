// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V32Tests;

public class OpenApiPathItemDeserializerTests
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
        var getOp = Assert.Contains(HttpMethod.Get, pathItem.Operations);
        Assert.Equal("getPets", getOp.OperationId);
        
        var postOp = Assert.Contains(HttpMethod.Post, pathItem.Operations);
        Assert.Equal("createPet", postOp.OperationId);

        // Query operation should now be on one of the operations
        // Since the YAML structure changed, we need to check which operation has the query
        var queryOp = Assert.Contains(new HttpMethod("Query"), pathItem.Operations);
        Assert.Equal("Query pets with complex filters", queryOp.Summary);
        Assert.Equal("queryPets", queryOp.OperationId);
        Assert.Single(queryOp.Parameters);
        Assert.Equal("filter", queryOp.Parameters[0].Name);

        var notifyOp = Assert.Contains(new HttpMethod("notify"), pathItem.Operations);
        Assert.Equal("Notify about pet updates", notifyOp.Summary);
        Assert.Equal("notifyPetUpdates", notifyOp.OperationId);
        Assert.NotNull(notifyOp.RequestBody);

        var subscribeOp = Assert.Contains(new HttpMethod("subscribe"), pathItem.Operations);
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
        var getOp = Assert.Contains(HttpMethod.Get, pathItem.Operations);
        Assert.Equal("getPets", getOp.OperationId);

        // Query operation from extension should now be on the operation
        var queryOp = Assert.Contains(new HttpMethod("Query"), pathItem.Operations);
        Assert.Equal("Query pets with complex filters", queryOp.Summary);
        Assert.Equal("queryPets", queryOp.OperationId);

        // Additional operations from extension should now be on the operation
        var notifyOp = Assert.Contains(new HttpMethod("notify"), pathItem.Operations);
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
                    Responses = new OpenApiResponses
                    {
                        ["200"] = new OpenApiResponse { Description = "Success" }
                    }
                },
                [new HttpMethod("Query")] = new OpenApiOperation
                {
                    Summary = "Query operation",
                    OperationId = "queryOp",
                    Responses = new OpenApiResponses
                    {
                        ["200"] = new OpenApiResponse { Description = "Success" }
                    }
                },
                [new HttpMethod("notify")] = new OpenApiOperation
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
            Operations = new Dictionary<HttpMethod, OpenApiOperation>
            {
                [HttpMethod.Get] = new OpenApiOperation
                {
                    Summary = "Get operation",
                    OperationId = "getOp",
                    Responses = new OpenApiResponses
                    {
                        ["200"] = new OpenApiResponse { Description = "Success" }
                    }
                },
                [new HttpMethod("Query")] = new OpenApiOperation
                {
                    Summary = "Query operation",
                    OperationId = "queryOp",
                    Responses = new OpenApiResponses
                    {
                       ["200"] = new OpenApiResponse { Description = "Success" }
                    }
                },
                [new HttpMethod("notify")] = new OpenApiOperation
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
            Operations = new Dictionary<HttpMethod, OpenApiOperation>
            {
                [HttpMethod.Get] = new OpenApiOperation
                {
                    Summary = "Get operation",
                    OperationId = "getOp",
                    Responses = new OpenApiResponses()
                },
                [new HttpMethod("Query")] = new OpenApiOperation
                {
                    Summary = "Query operation",
                    OperationId = "queryOp"
                },
                [new HttpMethod("notify")] = new OpenApiOperation
                {
                    Summary = "Notify operation",
                    OperationId = "notifyOp"
                }
            }
        };

        // Act
        var copy = original.CreateShallowCopy();

        // Assert
        Assert.NotNull(original.Operations);
        Assert.NotNull(copy.Operations);
        Assert.Contains(HttpMethod.Get, original.Operations);
        Assert.Contains(HttpMethod.Get, copy.Operations);

        var copyQueryOp = Assert.Contains(new HttpMethod("Query"), copy.Operations);
        Assert.Contains(new HttpMethod("Query"), original.Operations);
        Assert.Equal("Query operation", copyQueryOp.Summary);
        Assert.Equal("queryOp", copyQueryOp.OperationId);
        Assert.Contains(new HttpMethod("notify"), original.Operations);
        var copyNotifyOp = Assert.Contains(new HttpMethod("notify"), copy.Operations);
        Assert.Equal("Notify operation", copyNotifyOp.Summary);
        Assert.Equal("notifyOp", copyNotifyOp.OperationId);
    }
}
