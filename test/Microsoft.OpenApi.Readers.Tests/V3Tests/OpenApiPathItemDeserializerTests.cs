// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V3Tests;

public class OpenApiPathItemDeserializerTests
{
    private const string SampleFolderPath = "V3Tests/Samples/OpenApiPathItem/";

    [Fact]
    public async Task ExtraneousOperationsAreParsedAsExtensionsIn30()
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
        Assert.DoesNotContain(new HttpMethod("Query"), pathItem.Operations);
        Assert.DoesNotContain(new HttpMethod("notify"), pathItem.Operations);
        Assert.DoesNotContain(new HttpMethod("subscribe"), pathItem.Operations);

        var additionalPathsExt = Assert.IsType<JsonNodeExtension>(Assert.Contains("x-oai-additionalOperations", pathItem.Extensions));

        var additionalOpsNode = Assert.IsType<JsonObject>(additionalPathsExt.Node);
        Assert.Contains("query", additionalOpsNode);
        Assert.Contains("notify", additionalOpsNode);
        Assert.Contains("subscribe", additionalOpsNode);
    }
}
