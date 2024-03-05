// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.OData;

namespace Microsoft.OpenApi.Hidi.Utilities
{
    internal static class SettingsUtilities
    {
        internal static IConfiguration GetConfiguration(string? settingsFile = null)
        {
            if (string.IsNullOrEmpty(settingsFile))
            {
                settingsFile = "appsettings.json";
            }

            var settingsFilePath = Path.Combine(Directory.GetCurrentDirectory(), settingsFile);

            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile(settingsFilePath, true)
                .Build();

            return config;
        }
        
        internal static OpenApiConvertSettings GetOpenApiConvertSettings(IConfiguration config, string? metadataVersion)
        {
            ArgumentNullException.ThrowIfNull(config);
            var settings = new OpenApiConvertSettings();
            if (!string.IsNullOrEmpty(metadataVersion))
                settings.SemVerVersion = metadataVersion;
            config.GetSection(nameof(OpenApiConvertSettings)).Bind(settings);
            return settings;
        }
    }
}
