﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.OpenApi.Reader;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V3Tests
{
    [Collection("DefaultSettings")]
    public class OpenApiXmlTests
    {
        private const string SampleFolderPath = "V3Tests/Samples/OpenApiXml/";

        [Fact]
        public async Task ParseBasicXmlShouldSucceed()
        {
            // Act
            var xml = await OpenApiModelFactory.LoadAsync<OpenApiXml>(Resources.GetStream(Path.Combine(SampleFolderPath, "basicXml.yaml")), OpenApiSpecVersion.OpenApi3_0, new(), settings: SettingsFixture.ReaderSettings);

            // Assert
#pragma warning disable CS0618 // Type or member is obsolete
            Assert.Equivalent(
                new OpenApiXml
                {
                    Name = "name1",
                    Namespace = new Uri("http://example.com/schema/namespaceSample"),
                    Prefix = "samplePrefix",
                    Wrapped = true
                }, xml);
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }
}
