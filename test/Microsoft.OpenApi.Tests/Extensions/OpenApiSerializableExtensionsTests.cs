using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.OpenApi.Tests.Extensions;

public class OpenApiSerializableExtensionsTests
{
    [Fact]
    public async Task UsesTheTerseOutputInformationFromSettingsTrue()
    {
        var parameter = new OpenApiParameter
        {
            Name = "param1",
            In = ParameterLocation.Query,
            Description = "A sample parameter",
            Required = false,
            Schema = new OpenApiSchema
            {
                Type = JsonSchemaType.String
            }
        };

        var settings = new OpenApiJsonWriterSettings
        {
            Terse = true
        };

        using var stream = new MemoryStream();
        await parameter.SerializeAsync(stream, OpenApiSpecVersion.OpenApi3_1, OpenApiConstants.Json, settings);

        stream.Position = 0;
        using var reader = new StreamReader(stream);
        var output = await reader.ReadToEndAsync();

        Assert.Equal("{\"name\":\"param1\",\"in\":\"query\",\"description\":\"A sample parameter\",\"schema\":{\"type\":\"string\"}}", output);
    }

    [Fact]
    public async Task UsesTheTerseOutputInformationFromSettingsFalse()
    {
        var parameter = new OpenApiParameter
        {
            Name = "param1",
            In = ParameterLocation.Query,
            Description = "A sample parameter",
            Required = false,
            Schema = new OpenApiSchema
            {
                Type = JsonSchemaType.String
            }
        };

        var settings = new OpenApiJsonWriterSettings
        {
            Terse = false
        };

        using var stream = new MemoryStream();
        await parameter.SerializeAsync(stream, OpenApiSpecVersion.OpenApi3_1, OpenApiConstants.Json, settings);

        stream.Position = 0;
        using var reader = new StreamReader(stream);
        var output = await reader.ReadToEndAsync();

        Assert.Equal("{\n  \"name\": \"param1\",\n  \"in\": \"query\",\n  \"description\": \"A sample parameter\",\n  \"schema\": {\n    \"type\": \"string\"\n  }\n}", output);
    }
    
    [Fact]
    public async Task UsesTheTerseOutputInformationFromSettingsNoSettings()
    {
        var parameter = new OpenApiParameter
        {
            Name = "param1",
            In = ParameterLocation.Query,
            Description = "A sample parameter",
            Required = false,
            Schema = new OpenApiSchema
            {
                Type = JsonSchemaType.String
            }
        };

        using var stream = new MemoryStream();
        await parameter.SerializeAsync(stream, OpenApiSpecVersion.OpenApi3_1, OpenApiConstants.Json, null);

        stream.Position = 0;
        using var reader = new StreamReader(stream);
        var output = await reader.ReadToEndAsync();

        Assert.Equal("{\n  \"name\": \"param1\",\n  \"in\": \"query\",\n  \"description\": \"A sample parameter\",\n  \"schema\": {\n    \"type\": \"string\"\n  }\n}", output);
    }
}
