using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using Microsoft.OpenApi.Reader;
using Xunit;

namespace Microsoft.OpenApi.Tests.Reader;

public class JsonNodeHelperTests
{
    [Fact]
    public void CreateListUsesOriginalIndicesAndOmitsNullResults()
    {
        var array = JsonNode.Parse("""[true, null, {}, "ignored", {}, {}]""");
        var context = new ParsingContext(new());
        var locations = new List<string>();
        context.StartObject("items");

        var actual = array.CreateList((_, _, c) =>
        {
            var location = c.GetLocation();
            locations.Add(location);
            return locations.Count == 2 ? null : location;
        }, new OpenApiDocument(), context);

        Assert.Equal(new[] { "#/items/2", "#/items/4", "#/items/5" }, locations);
        Assert.Equal(new[] { "#/items/2", "#/items/5" }, actual);
        Assert.Equal("#/items", context.GetLocation());
    }

    [Fact]
    public void CreateListRestoresContextWhenMapperThrows()
    {
        var array = JsonNode.Parse("[{}]");
        var context = new ParsingContext(new());
        var expected = new InvalidOperationException("Mapping failed.");
        context.StartObject("items");
        string actualLocation = null;

        var actual = Assert.Throws<InvalidOperationException>(() => array.CreateList<string>((_, _, c) =>
        {
            actualLocation = c.GetLocation();
            throw expected;
        }, new OpenApiDocument(), context));

        Assert.Same(expected, actual);
        Assert.Equal("#/items/0", actualLocation);
        Assert.Equal("#/items", context.GetLocation());
    }

    [Fact]
    public void CreateListRestoresContextAfterNestedLists()
    {
        var array = JsonNode.Parse("""[{"children": [{}, {}]}, {"children": [{}]}]""");
        var context = new ParsingContext(new());
        context.StartObject("items");

        var actual = array.CreateList((node, document, c) =>
        {
            c.StartObject("children");
            try
            {
                return node["children"].CreateList((_, _, childContext) => childContext.GetLocation(), document, c);
            }
            finally
            {
                c.EndObject();
            }
        }, new OpenApiDocument(), context);

        Assert.Collection(actual,
            first => Assert.Equal(new[] { "#/items/0/children/0", "#/items/0/children/1" }, first),
            second => Assert.Equal(new[] { "#/items/1/children/0" }, second));
        Assert.Equal("#/items", context.GetLocation());
    }

    [Theory]
    [InlineData("[]")]
    [InlineData("[null, true, 1, \"ignored\", []]")]
    public void CreateListSkipsNonObjectsWithoutChangingContext(string input)
    {
        var array = JsonNode.Parse(input);
        var context = new ParsingContext(new());
        context.StartObject("items");

        var actual = array.CreateList<string>((_, _, _) => throw new InvalidOperationException("Mapper must not be called."), new OpenApiDocument(), context);

        Assert.Empty(actual);
        Assert.Equal("#/items", context.GetLocation());
    }

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
