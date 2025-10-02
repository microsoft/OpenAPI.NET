// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.IO;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.OpenApi.Reader;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V32Tests
{
    [Collection("DefaultSettings")]
    public class OpenApiExampleTests
    {
        private const string SampleFolderPath = "V32Tests/Samples/OpenApiExample/";

        [Fact]
        public async Task ParseExampleWithDataValueShouldSucceed()
        {
            // Arrange & Act
            var example = await OpenApiModelFactory.LoadAsync<OpenApiExample>(
                Path.Combine(SampleFolderPath, "exampleWithDataValue.yaml"), 
                OpenApiSpecVersion.OpenApi3_2, 
                new(), 
                SettingsFixture.ReaderSettings);

            // Assert
            Assert.NotNull(example);
            Assert.Equal("Example with dataValue", example.Summary);
            Assert.NotNull(example.DataValue);
            Assert.Equal("John Doe", example.DataValue["name"].GetValue<string>());
            Assert.Equal(30, example.DataValue["age"].GetValue<decimal>());
        }

        [Fact]
        public async Task ParseExampleWithSerializedValueShouldSucceed()
        {
            // Arrange & Act
            var example = await OpenApiModelFactory.LoadAsync<OpenApiExample>(
                Path.Combine(SampleFolderPath, "exampleWithSerializedValue.yaml"), 
                OpenApiSpecVersion.OpenApi3_2, 
                new(), 
                SettingsFixture.ReaderSettings);

            // Assert
            Assert.NotNull(example);
            Assert.Equal("Example with serializedValue", example.Summary);
            Assert.Equal("custom serialized string", example.SerializedValue);
        }
    }
}
