﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.IO;
using FluentAssertions;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V2Tests
{
    [Collection("DefaultSettings")]
    public class ComparisonTests
    {
        private const string SampleFolderPath = "V2Tests/Samples/";

        [Theory]
        [InlineData("minimal")]
        [InlineData("basic")]
        //[InlineData("definitions")]  //Currently broken due to V3 references not behaving the same as V2
        public void EquivalentV2AndV3DocumentsShouldProductEquivalentObjects(string fileName)
        {
            using (var streamV2 = File.OpenRead(Path.Combine(SampleFolderPath, $"{fileName}.v2.yaml")))
            using (var streamV3 = File.OpenRead(Path.Combine(SampleFolderPath, $"{fileName}.v3.yaml")))
            {
                var openApiDocV2 = new OpenApiStreamReader().Read(streamV2, out var contextV2);
                var openApiDocV3 = new OpenApiStreamReader().Read(streamV3, out var contextV3);

                // Everything in the DOM read from V2 and V3 documents should be equal
                // except the SpecVersion property (2.0 and 3.0.0)
                openApiDocV3.ShouldBeEquivalentTo(
                    openApiDocV2);

                contextV2.ShouldBeEquivalentTo(contextV3);
            }
        }
    }
}