using System;
using System.Text.Json.Nodes;
using Microsoft.OpenApi.YamlReader;
using SharpYaml.Serialization;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests;

[Collection(YamlConverterGlobalSettingsCollection.Name)]
public class YamlConverterGlobalSettingsTests
{
    [Fact]
    public void ConversionLimitsDefaultToDocumentedValues()
    {
        Assert.Equal(64u, YamlConverter.DefaultMaxDepth);
        Assert.Equal(5_000_000u, YamlConverter.DefaultMaxNodeCount);
        Assert.Equal(5_000u, YamlConverter.DefaultMaxAliasExpansionNodeCount);
        Assert.Equal(YamlConverter.DefaultMaxDepth, YamlConverter.MaxDepth);
        Assert.Equal(YamlConverter.DefaultMaxNodeCount, YamlConverter.MaxNodeCount);
        Assert.Equal(YamlConverter.DefaultMaxAliasExpansionNodeCount, YamlConverter.MaxAliasExpansionNodeCount);
    }

    [Fact]
    public void SettingMaxDepthToZeroThrows()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => YamlConverter.MaxDepth = 0);
        Assert.Equal(YamlConverter.DefaultMaxDepth, YamlConverter.MaxDepth);
    }

    [Fact]
    public void SettingMaxNodeCountToZeroThrows()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => YamlConverter.MaxNodeCount = 0);
        Assert.Equal(YamlConverter.DefaultMaxNodeCount, YamlConverter.MaxNodeCount);
    }

    [Fact]
    public void SettingMaxAliasExpansionNodeCountToZeroThrows()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => YamlConverter.MaxAliasExpansionNodeCount = 0);
        Assert.Equal(YamlConverter.DefaultMaxAliasExpansionNodeCount, YamlConverter.MaxAliasExpansionNodeCount);
    }

    [Fact]
    public void SettingMaxDepthAboveSafeCeilingThrows()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => YamlConverter.MaxDepth = YamlConverter.MaximumAllowedDepth + 1);
        Assert.Equal(YamlConverter.DefaultMaxDepth, YamlConverter.MaxDepth);
    }

    [Fact]
    public void SettingMaxNodeCountAboveSafeCeilingThrows()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => YamlConverter.MaxNodeCount = YamlConverter.MaximumAllowedNodeCount + 1);
        Assert.Equal(YamlConverter.DefaultMaxNodeCount, YamlConverter.MaxNodeCount);
    }

    [Fact]
    public void RaisingMaxDepthAllowsDocumentsDeeperThanTheDefault()
    {
        const int depth = 70;
        YamlNode deeplyNested = new YamlScalarNode("value");
        for (var index = 0; index < depth; index++)
        {
            deeplyNested = new YamlSequenceNode(deeplyNested);
        }

        try
        {
            YamlConverter.MaxDepth = depth + 10;

            var jsonNode = deeplyNested.ToJsonNode();

            Assert.IsType<JsonArray>(jsonNode);
        }
        finally
        {
            YamlConverter.MaxDepth = YamlConverter.DefaultMaxDepth;
        }
    }
}
