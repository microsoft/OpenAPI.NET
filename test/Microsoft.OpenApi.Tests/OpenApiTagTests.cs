// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.OpenApi.Tests
{
    public class OpenApiTagTests
    {
        public static OpenApiTag BasicTag = new OpenApiTag();
        public static OpenApiTag AdvanceTag = new OpenApiTag()
        {
            Name = "pet",
            Description = "Pets operations",
            ExternalDocs = OpenApiExternalDocsTests.AdvanceExDocs
        };
    }
}
