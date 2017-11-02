// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

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

            // TODO: Add fluent assertion to make this assert possible without implementing equality ourselves.
            // Assert.Equal(openApiDocV2, openApiDocV3);
        }
    }
}