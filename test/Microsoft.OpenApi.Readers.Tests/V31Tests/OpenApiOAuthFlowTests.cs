// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.OpenApi.Reader;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V31Tests
{
    [Collection("DefaultSettings")]
    public class OpenApiOAuthFlowTests
    {
        private const string SampleFolderPath = "V31Tests/Samples/OpenApiSecurityScheme/";

        [Fact]
        public async Task ParseOAuth2SecuritySchemeWithDeviceAuthorizationUrlShouldSucceed()
        {
            // Act
            var securityScheme = await OpenApiModelFactory.LoadAsync<OpenApiSecurityScheme>(
                Path.Combine(SampleFolderPath, "oauth2SecuritySchemeWithDeviceUrl.yaml"),
                OpenApiSpecVersion.OpenApi3_1,
                new(),
                SettingsFixture.ReaderSettings);

            // Assert
            Assert.NotNull(securityScheme);
            Assert.Equal(SecuritySchemeType.OAuth2, securityScheme.Type);
            Assert.NotNull(securityScheme.Flows?.AuthorizationCode);
            Assert.Equal(new Uri("https://example.com/api/oauth/dialog"), securityScheme.Flows.AuthorizationCode.AuthorizationUrl);
            Assert.Equal(new Uri("https://example.com/api/oauth/token"), securityScheme.Flows.AuthorizationCode.TokenUrl);
            Assert.Equal(new Uri("https://example.com/api/oauth/device"), securityScheme.Flows.AuthorizationCode.DeviceAuthorizationUrl);
            Assert.NotNull(securityScheme.Flows.AuthorizationCode.Scopes);
            Assert.Equal(2, securityScheme.Flows.AuthorizationCode.Scopes.Count);
        }
    }
}
