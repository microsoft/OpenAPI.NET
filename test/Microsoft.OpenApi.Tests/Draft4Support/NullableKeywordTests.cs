using System.Text.Json.Nodes;
using Json.Schema;
using Microsoft.OpenApi.Draft4Support;
using Xunit;

namespace Microsoft.OpenApi.Tests.Draft4Support;

public class NullableKeywordTests
{
    private readonly EvaluationOptions _options = new EvaluationOptions { EvaluateAs = Draft4SupportData.Draft4Version };

    [Fact]
    public void NullableKeywordFalse_StringPasses()
    {
        var schema = new JsonSchemaBuilder()
            .Type(SchemaValueType.String)
            .Nullable(false);

        JsonNode instance = "foo";

        var result = schema.Evaluate(instance, _options);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void NullableKeywordFalse_NullFails()
    {
        var schema = new JsonSchemaBuilder()
            .Type(SchemaValueType.String)
            .Nullable(false);

        JsonNode instance = null;

        var result = schema.Evaluate(instance, _options);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void NullableKeywordTrue_StringPasses()
    {
        var schema = new JsonSchemaBuilder()
            .Type(SchemaValueType.String)
            .Nullable(true);

        JsonNode instance = "foo";

        var result = schema.Evaluate(instance, _options);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void NullableKeywordTrue_NullPasses()
    {
        var schema = new JsonSchemaBuilder()
            .Type(SchemaValueType.String)
            .Nullable(true);

        JsonNode instance = null;

        var result = schema.Evaluate(instance, _options);

        Assert.True(result.IsValid);
    }
}
