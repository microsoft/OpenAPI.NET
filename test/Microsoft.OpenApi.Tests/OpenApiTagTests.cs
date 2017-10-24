// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

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
