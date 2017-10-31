// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Tests
{
    internal static class OpenApiDocumentTestHelper
    {
        public static void CreatePath(this OpenApiDocument doc, string key, Action<OpenApiPathItem> configure)
        {
            var pathItem = new OpenApiPathItem();
            configure(pathItem);
            doc.Paths.Add(key, pathItem);
        }
    }
}
