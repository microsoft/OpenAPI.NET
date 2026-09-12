using Xunit;

namespace Microsoft.OpenApi.Readers.Tests;

[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class YamlConverterGlobalSettingsCollection
{
    public const string Name = "YamlConverterGlobalSettings";
}
