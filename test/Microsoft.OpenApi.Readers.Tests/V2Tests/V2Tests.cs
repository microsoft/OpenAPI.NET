// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using FluentAssertions;
using Microsoft.OpenApi.Models;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V2Tests
{
    public class V2Tests
    {
        [Theory]
        [InlineData("simplest")]
        [InlineData("host")]
        public void Tests(string fileName)
        {
            var v2stream = GetType()
                .Assembly.GetManifestResourceStream(typeof(V2Tests), $"V2Samples.{fileName}.2.yaml");
            var v3stream = GetType()
                .Assembly.GetManifestResourceStream(typeof(V2Tests), $"V2Samples.{fileName}.3.yaml");

            var openApiDocV2 = new OpenApiStreamReader().Read(v2stream, out var contextV2);
            var openApiDocV3 = new OpenApiStreamReader().Read(v3stream, out var contextV3);

            // Everything in the DOM read from V2 and V3 documents should be equal
            // except the SpecVersion property (2.0 and 3.0.0)
            openApiDocV3.ShouldBeEquivalentTo(
                openApiDocV2,
                options => options.Excluding(
                    s => s.SelectedMemberPath == nameof(OpenApiDocument.SpecVersion)));
        }
    }
}