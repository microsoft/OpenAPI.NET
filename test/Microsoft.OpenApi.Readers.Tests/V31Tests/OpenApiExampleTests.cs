// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.IO;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.OpenApi.Reader;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V31Tests
{
    [Collection("DefaultSettings")]
    public class OpenApiExampleTests
    {
        private const string SampleFolderPath = "V31Tests/Samples/OpenApiExample/";

        [Fact]
        public async Task ParseExampleWithDataValueExtensionShouldSucceed()
        {
            // Arrange & Act
            var example = await OpenApiModelFactory.LoadAsync<OpenApiExample>(
                Path.Combine(SampleFolderPath, "exampleWithDataValue.yaml"), 
                OpenApiSpecVersion.OpenApi3_1, 
                new(), 
                SettingsFixture.ReaderSettings);

            // Assert
            Assert.NotNull(example);
            Assert.Equal("Example with dataValue (extension)", example.Summary);
            Assert.NotNull(example.DataValue);
            Assert.Equal("Jane Smith", example.DataValue["name"].GetValue<string>());
            Assert.Equal(25, example.DataValue["age"].GetValue<decimal>());
        }

        [Fact]
        public async Task ParseExampleWithSerializedValueExtensionShouldSucceed()
        {
            // Arrange & Act
            var example = await OpenApiModelFactory.LoadAsync<OpenApiExample>(
                Path.Combine(SampleFolderPath, "exampleWithSerializedValue.yaml"), 
                OpenApiSpecVersion.OpenApi3_1, 
                new(), 
                SettingsFixture.ReaderSettings);

            // Assert
            Assert.NotNull(example);
            Assert.Equal("Example with serializedValue (extension)", example.Summary);
            Assert.Equal("custom serialized string with extension", example.SerializedValue);
        }
    }
}
