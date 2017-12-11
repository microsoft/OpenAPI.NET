// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.IO;
using FluentAssertions;
using Microsoft.OpenApi.Models;
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
                    openApiDocV2,
                    options => options.Excluding(
                        s => s.SelectedMemberPath == nameof(OpenApiDocument.SpecVersion)));

                contextV2.ShouldBeEquivalentTo(contextV3);
            }
            
        }
    }
}