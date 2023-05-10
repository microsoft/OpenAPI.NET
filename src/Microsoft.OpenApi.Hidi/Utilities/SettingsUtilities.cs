// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.OData;

namespace Microsoft.OpenApi.Hidi.Utilities
{
    internal static class SettingsUtilities
    {
        internal static IConfiguration GetConfiguration(string settingsFile = null)
        {
            settingsFile ??= "appsettings.json";

            IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile(settingsFile, true)
            .Build();

            return config;
        }

        internal static OpenApiConvertSettings GetOpenApiConvertSettings(IConfiguration config, string metadataVersion = null)
        {
            if (config == null) { throw new System.ArgumentNullException(nameof(config)); }
            var settings = new OpenApiConvertSettings();
            if (!string.IsNullOrEmpty(metadataVersion))
                settings.SemVerVersion = metadataVersion;
            config.GetSection(nameof(OpenApiConvertSettings)).Bind(settings);
            return settings;
        }
    }
}
