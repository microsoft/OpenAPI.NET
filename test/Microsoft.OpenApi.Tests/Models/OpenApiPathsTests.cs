// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.IO;
using Xunit;

namespace Microsoft.OpenApi.Tests.Models
{
    [Collection("DefaultSettings")]
    public class OpenApiPathsTests
    {
        [Fact]
        public void SerializePaths_WithRfc6570Operator_BelowV32_Throws()
        {
            var paths = new OpenApiPaths
            {
                ["/files/{+path}"] = new OpenApiPathItem()
            };

            var writer = new OpenApiJsonWriter(new StringWriter());

            Assert.Throws<OpenApiException>(() => paths.SerializeAsV3(writer));
            Assert.Throws<OpenApiException>(() => paths.SerializeAsV31(writer));
            Assert.Throws<OpenApiException>(() => paths.SerializeAsV2(writer));
        }

        [Fact]
        public void SerializePaths_WithExplodeOperator_BelowV32_Throws()
        {
            var paths = new OpenApiPaths
            {
                ["/files/{path*}"] = new OpenApiPathItem()
            };

            var writer = new OpenApiJsonWriter(new StringWriter());

            Assert.Throws<OpenApiException>(() => paths.SerializeAsV3(writer));
            Assert.Throws<OpenApiException>(() => paths.SerializeAsV31(writer));
            Assert.Throws<OpenApiException>(() => paths.SerializeAsV2(writer));
        }

        [Fact]
        public void SerializePaths_WithRfc6570Operator_V32_Succeeds()
        {
            var paths = new OpenApiPaths
            {
                ["/files/{+path}"] = new OpenApiPathItem()
            };

            var writer = new OpenApiJsonWriter(new StringWriter());

            paths.SerializeAsV32(writer);
        }

        [Fact]
        public void SerializePaths_WithoutOperators_BelowV32_Succeeds()
        {
            var paths = new OpenApiPaths
            {
                ["/files/{path}"] = new OpenApiPathItem()
            };

            var writer = new OpenApiJsonWriter(new StringWriter());

            paths.SerializeAsV3(writer);
            paths.SerializeAsV31(writer);
            paths.SerializeAsV2(writer);
        }
    }
}
