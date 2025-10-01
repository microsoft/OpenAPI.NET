// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.IO;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using FluentAssertions;
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
            example.Should().NotBeNull();
            example.Summary.Should().Be("Example with dataValue (extension)");
            example.DataValue.Should().NotBeNull();
            example.DataValue["name"].GetValue<string>().Should().Be("Jane Smith");
            example.DataValue["age"].GetValue<decimal>().Should().Be(25);
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
            example.Should().NotBeNull();
            example.Summary.Should().Be("Example with serializedValue (extension)");
            example.SerializedValue.Should().Be("custom serialized string with extension");
        }
    }
}
