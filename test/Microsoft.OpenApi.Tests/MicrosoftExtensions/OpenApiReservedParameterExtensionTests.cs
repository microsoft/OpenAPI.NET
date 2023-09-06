using System;
using System.IO;
using Microsoft.OpenApi.MicrosoftExtensions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Writers;
using Xunit;

namespace Microsoft.OpenApi.Tests.MicrosoftExtensions;

public class OpenApiReservedParameterExtensionTests
{
    [Fact]
    public void Parses()
    {
        var oaiValue = new OpenApiBoolean(true);
        var value = OpenApiReservedParameterExtension.Parse(oaiValue);
        Assert.NotNull(value);
        Assert.True(value.IsReserved);
    }
    [Fact]
    public void Serializes()
    {
        var value = new OpenApiReservedParameterExtension
        {
            IsReserved = true
        };
        using TextWriter sWriter = new StringWriter();
        OpenApiJsonWriter writer = new(sWriter);

        value.Write(writer, OpenApiSpecVersion.OpenApi3_0);
        var result = sWriter.ToString();
        Assert.Equal("true", result, StringComparer.OrdinalIgnoreCase);
    }
}
