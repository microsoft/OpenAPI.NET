// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.IO;
using FluentAssertions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V3Tests
{
    [Collection("DefaultSettings")]
    public class OpenApiXmlTests
    {
        private const string SampleFolderPath = "V3Tests/Samples/OpenApiXml/";

        public OpenApiXmlTests()
        {
            OpenApiReaderRegistry.RegisterReader("yaml", new OpenApiYamlReader());
        }

        [Fact]
        public void ParseBasicXmlShouldSucceed()
        {
            // Act
            var xml = OpenApiModelFactory.Load<OpenApiXml>(Resources.GetStream(Path.Combine(SampleFolderPath, "basicXml.yaml")), OpenApiSpecVersion.OpenApi3_0, "yaml", out _);

            // Assert
            xml.Should().BeEquivalentTo(
                new OpenApiXml
                {
                    Name = "name1",
                    Namespace = new Uri("http://example.com/schema/namespaceSample"),
                    Prefix = "samplePrefix",
                    Wrapped = true
                });
        }
    }
}
