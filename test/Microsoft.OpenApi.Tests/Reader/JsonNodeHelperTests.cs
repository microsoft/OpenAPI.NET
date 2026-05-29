using System.Text.Json.Nodes;
using Microsoft.OpenApi.Reader;
using Xunit;

namespace Microsoft.OpenApi.Tests.Reader;

public class JsonNodeHelperTests
{
    [Fact]
    public void DoesNotFailOnNullValue()
    {
        var jsonObject = JsonNode.Parse("{\"key\": null}")?.AsObject();
        var context = new ParsingContext(new());
        JsonNode actual = null;

        jsonObject.ParseMap(
            new object(),
            new FixedFieldMap<object>(),
            new PatternFieldMap<object>(),
            new OpenApiDocument(),
            context,
            (_, _, value) => actual = value);

        Assert.True(actual.IsJsonNullSentinel());
    }
}
