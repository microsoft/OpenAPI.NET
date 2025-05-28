using System.Text.Json.Nodes;
using Microsoft.OpenApi.Reader;
using Xunit;

namespace Microsoft.OpenApi.Tests.Reader;

public class MapNodeTests
{
    [Fact]
    public void DoesNotFailOnNullValue()
    {
        var jsonNode = JsonNode.Parse("{\"key\": null}");
        var mapNode = new MapNode(new ParsingContext(new()), jsonNode);

        Assert.NotNull(mapNode);
        Assert.Empty(mapNode);
    }
}
