using System.Collections.Generic;
using System.Text.Json.Nodes;
using Xunit;
using Microsoft.OpenApi.Reader;

namespace Microsoft.OpenApi.Tests.Reader;

public class AnyListFieldMapParameterTests
{
    [Fact]
    public void ConstructorStoresPropertyDelegates()
    {
        var schema = new OpenApiSchema { Type = JsonSchemaType.Array };
        var values = new List<JsonNode> { JsonValue.Create("value")! };
        var owner = new TestOwner { Schema = schema };
        var parameter = new AnyListFieldMapParameter<TestOwner>(
            static current => current.Values,
            static (current, currentValues) => current.Values = currentValues,
            static current => current.Schema);

        parameter.PropertySetter(owner, values);

        Assert.Same(values, parameter.PropertyGetter(owner));
        Assert.NotNull(parameter.SchemaGetter);
        Assert.Same(schema, parameter.SchemaGetter(owner));
    }

    private sealed class TestOwner
    {
        public List<JsonNode> Values { get; set; } = [];
        public OpenApiSchema Schema { get; set; } = new();
    }
}
