using Microsoft.OpenApi.Reader;

namespace Microsoft.OpenApi.Readers.Tests;
public static class SettingsFixture
{
    public static OpenApiReaderSettings ReaderSettings { get { var settings = new OpenApiReaderSettings(); settings.AddYamlReader() ; return settings; } }
}
