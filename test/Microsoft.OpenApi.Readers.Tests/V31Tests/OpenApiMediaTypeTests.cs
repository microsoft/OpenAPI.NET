// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.IO;
using System.Threading.Tasks;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Tests;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V31Tests
{
    [Collection("DefaultSettings")]
    public class OpenApiMediaTypeTests
    {
        private const string SampleFolderPath = "V31Tests/Samples/OpenApiMediaType/";

        [Fact]
        public async Task ParseMediaTypeWithXOaiItemSchemaShouldSucceed()
        {
            // Act
            var mediaType = await OpenApiModelFactory.LoadAsync<OpenApiMediaType>(
                Path.Combine(SampleFolderPath, "mediaTypeWithXOaiItemSchema.yaml"),
                OpenApiSpecVersion.OpenApi3_1,
                new(),
                SettingsFixture.ReaderSettings);

            // Assert
            var expected = new OpenApiMediaType
            {
                Schema = new OpenApiSchema
                {
                    Type = JsonSchemaType.Array
                },
                ItemSchema = new OpenApiSchema
                {
                    Type = JsonSchemaType.String,
                    MaxLength = 100
                }
            };
            Assert.Equivalent(expected, mediaType);
        }
    }
}
