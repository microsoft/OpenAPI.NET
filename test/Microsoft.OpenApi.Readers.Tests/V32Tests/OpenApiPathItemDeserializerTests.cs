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
}
