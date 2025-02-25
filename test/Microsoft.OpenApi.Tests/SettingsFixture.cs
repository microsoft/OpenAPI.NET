using Microsoft.OpenApi.Reader;

namespace Microsoft.OpenApi.Tests;
public static class SettingsFixture
{
    public static OpenApiReaderSettings ReaderSettings { get { var settings = new OpenApiReaderSettings(); settings.AddYamlReader() ; return settings; } }
}
