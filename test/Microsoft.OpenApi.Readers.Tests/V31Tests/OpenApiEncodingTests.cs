// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.IO;
using System.Threading.Tasks;
using Microsoft.OpenApi.Reader;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V31Tests
{
    [Collection("DefaultSettings")]
    public class OpenApiEncodingTests
    {
        private const string SampleFolderPath = "V31Tests/Samples/OpenApiEncoding/";

        [Fact]
        public async Task ParseEncodingWithAllowReservedShouldSucceed()
        {
            // Act
            var encoding = await OpenApiModelFactory.LoadAsync<OpenApiEncoding>(Path.Combine(SampleFolderPath, "encodingWithAllowReserved.yaml"), OpenApiSpecVersion.OpenApi3_1, new(), SettingsFixture.ReaderSettings);

            // Assert
            Assert.Equivalent(
                new OpenApiEncoding
                {
                    ContentType = "application/x-www-form-urlencoded",
                    Style = ParameterStyle.Form,
                    Explode = true,
                    AllowReserved = true
                }, encoding);
        }
    }
}
