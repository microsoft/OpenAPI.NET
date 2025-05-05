using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Xunit;

namespace Microsoft.OpenApi.Tests.Extensions;

public class OpenApiServerExtensionsTests
{
    [Fact]
    public void ShouldSubstituteServerVariableWithProvidedValues()
    {
        var variable = new OpenApiServer
        {
            Url = "http://example.com/api/{version}",
            Description = string.Empty,
            Variables = new OrderedDictionary<string, OpenApiServerVariable>
            {
                { "version", new OpenApiServerVariable { Default = "v1", Enum = ["v1", "v2"]} }
            }
        };

        var url = variable.ReplaceServerUrlVariables(new OrderedDictionary<string, string> {{"version", "v2"}});
        
        Assert.Equal("http://example.com/api/v2", url);
    }

    [Fact]
    public void ShouldSubstituteServerVariableWithDefaultValues()
    {
        var variable = new OpenApiServer
        {
            Url = "http://example.com/api/{version}",
            Description = string.Empty,
            Variables = new OrderedDictionary<string, OpenApiServerVariable>
            {
                { "version", new OpenApiServerVariable { Default = "v1", Enum = ["v1", "v2"]} }
            }
        };

        var url = variable.ReplaceServerUrlVariables(new OrderedDictionary<string, string>());
        
        Assert.Equal("http://example.com/api/v1", url);
    }

    [Fact]
    public void ShouldFailIfNoValueIsAvailable()
    {
        var variable = new OpenApiServer
        {
            Url = "http://example.com/api/{version}",
            Description = string.Empty,
            Variables = new OrderedDictionary<string, OpenApiServerVariable>
            {
                { "version", new OpenApiServerVariable { Enum = ["v1", "v2"]} }
            }
        };

        Assert.Throws<ArgumentException>(() =>
        {
            variable.ReplaceServerUrlVariables(new OrderedDictionary<string, string>());
        });
    }

    [Fact]
    public void ShouldFailIfProvidedValueIsNotInEnum()
    {
        var variable = new OpenApiServer
        {
            Url = "http://example.com/api/{version}",
            Description = string.Empty,
            Variables = new OrderedDictionary<string, OpenApiServerVariable>
            {
                { "version", new OpenApiServerVariable { Enum = ["v1", "v2"]} }
            }
        };

        Assert.Throws<ArgumentException>(() =>
        {
            variable.ReplaceServerUrlVariables(new OrderedDictionary<string, string> {{"version", "v3"}});
        });
    }

    [Fact]
    public void ShouldFailIfEnumIsEmpty()
    {
        var variable = new OpenApiServer
        {
            Url = "http://example.com/api/{version}",
            Description = string.Empty,
            Variables = new OrderedDictionary<string, OpenApiServerVariable>
            {
                { "version", new OpenApiServerVariable { Enum = []} }
            }
        };

        Assert.Throws<ArgumentException>(() =>
        {
            variable.ReplaceServerUrlVariables(new OrderedDictionary<string, string> {{"version", "v1"}});
        });
    }
}
