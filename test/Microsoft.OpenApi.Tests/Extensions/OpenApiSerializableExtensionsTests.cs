using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.OpenApi.Tests.Extensions;

public class OpenApiSerializableExtensionsTests
{
    private const string TerseOutput = "{\"name\":\"param1\",\"in\":\"query\",\"description\":\"A sample parameter\",\"schema\":{\"type\":\"string\"}}";
    private const string PrettyOutput = "{\n  \"name\": \"param1\",\n  \"in\": \"query\",\n  \"description\": \"A sample parameter\",\n  \"schema\": {\n    \"type\": \"string\"\n  }\n}";

    private static OpenApiParameter CreateParameter() => new()
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

    // ——————————————————————————————————————————————————————————
    //  SerializeAsync to Stream with format and settings
    // ——————————————————————————————————————————————————————————

    [Fact]
    public async Task SerializeAsV31JsonToStreamWithTerseSettingsWorks()
    {
        var parameter = CreateParameter();
        var settings = new OpenApiJsonWriterSettings { Terse = true };

        using var stream = new MemoryStream();
        await parameter.SerializeAsync(stream, OpenApiSpecVersion.OpenApi3_1, OpenApiConstants.Json, settings);

        stream.Position = 0;
        using var reader = new StreamReader(stream);
        var output = await reader.ReadToEndAsync();

        Assert.Equal(TerseOutput, output);
    }

    [Fact]
    public async Task SerializeAsV31JsonToStreamWithNonTerseSettingsWorks()
    {
        var parameter = CreateParameter();
        var settings = new OpenApiJsonWriterSettings { Terse = false };

        using var stream = new MemoryStream();
        await parameter.SerializeAsync(stream, OpenApiSpecVersion.OpenApi3_1, OpenApiConstants.Json, settings);

        stream.Position = 0;
        using var reader = new StreamReader(stream);
        var output = await reader.ReadToEndAsync();

        Assert.Equal(PrettyOutput, output);
    }

    [Fact]
    public async Task SerializeAsV31JsonToStreamWithoutSettingsWorks()
    {
        var parameter = CreateParameter();

        using var stream = new MemoryStream();
        await parameter.SerializeAsync(stream, OpenApiSpecVersion.OpenApi3_1, OpenApiConstants.Json, null);

        stream.Position = 0;
        using var reader = new StreamReader(stream);
        var output = await reader.ReadToEndAsync();

        Assert.Equal(PrettyOutput, output);
    }

    // ——————————————————————————————————————————————————————————
    //  SerializeAsJsonAsync to Stream with settings
    // ——————————————————————————————————————————————————————————

    [Fact]
    public async Task SerializeAsJsonAsyncToStreamWithTerseSettingsWorks()
    {
        var parameter = CreateParameter();
        var settings = new OpenApiJsonWriterSettings { Terse = true };

        using var stream = new MemoryStream();
        await parameter.SerializeAsJsonAsync(stream, OpenApiSpecVersion.OpenApi3_1, settings);

        stream.Position = 0;
        using var reader = new StreamReader(stream);
        var output = await reader.ReadToEndAsync();

        Assert.Equal(TerseOutput, output);
    }

    [Fact]
    public async Task SerializeAsJsonAsyncToStreamWithNonTerseSettingsWorks()
    {
        var parameter = CreateParameter();
        var settings = new OpenApiJsonWriterSettings { Terse = false };

        using var stream = new MemoryStream();
        await parameter.SerializeAsJsonAsync(stream, OpenApiSpecVersion.OpenApi3_1, settings);

        stream.Position = 0;
        using var reader = new StreamReader(stream);
        var output = await reader.ReadToEndAsync();

        Assert.Equal(PrettyOutput, output);
    }

    [Fact]
    public async Task SerializeAsJsonAsyncToStreamWithoutSettingsWorks()
    {
        var parameter = CreateParameter();

        using var stream = new MemoryStream();
        await parameter.SerializeAsJsonAsync(stream, OpenApiSpecVersion.OpenApi3_1, null);

        stream.Position = 0;
        using var reader = new StreamReader(stream);
        var output = await reader.ReadToEndAsync();

        Assert.Equal(PrettyOutput, output);
    }

    // ——————————————————————————————————————————————————————————
    //  SerializeAsJsonAsync to string with settings
    // ——————————————————————————————————————————————————————————

    [Fact]
    public async Task SerializeAsJsonAsyncToStringWithTerseSettingsWorks()
    {
        var parameter = CreateParameter();
        var settings = new OpenApiJsonWriterSettings { Terse = true };

        var output = await parameter.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_1, settings, CancellationToken.None);

        Assert.Equal(TerseOutput, output);
    }

    [Fact]
    public async Task SerializeAsJsonAsyncToStringWithNonTerseSettingsWorks()
    {
        var parameter = CreateParameter();
        var settings = new OpenApiJsonWriterSettings { Terse = false };

        var output = await parameter.SerializeAsJsonAsync(OpenApiSpecVersion.OpenApi3_1, settings, CancellationToken.None);

        Assert.Equal(PrettyOutput, output);
    }

    // ——————————————————————————————————————————————————————————
    //  SerializeAsync to string with format and settings
    // ——————————————————————————————————————————————————————————

    [Fact]
    public async Task SerializeAsV31JsonToStringWithTerseSettingsWorks()
    {
        var parameter = CreateParameter();
        var settings = new OpenApiJsonWriterSettings { Terse = true };

        var output = await parameter.SerializeAsync(OpenApiSpecVersion.OpenApi3_1, OpenApiConstants.Json, settings, CancellationToken.None);

        Assert.Equal(TerseOutput, output);
    }

    [Fact]
    public async Task SerializeAsV31JsonToStringWithNonTerseSettingsWorks()
    {
        var parameter = CreateParameter();
        var settings = new OpenApiJsonWriterSettings { Terse = false };

        var output = await parameter.SerializeAsync(OpenApiSpecVersion.OpenApi3_1, OpenApiConstants.Json, settings, CancellationToken.None);

        Assert.Equal(PrettyOutput, output);
    }

    [Fact]
    public async Task SerializeAsV31JsonToStringWithDefaultWriterSettingsWorks()
    {
        var parameter = CreateParameter();
        var settings = new OpenApiWriterSettings();

        var output = await parameter.SerializeAsync(OpenApiSpecVersion.OpenApi3_1, OpenApiConstants.Json, settings, CancellationToken.None);

        Assert.Equal(PrettyOutput, output);
    }
}
