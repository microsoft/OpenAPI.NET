// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OpenApi.Models;
using System.Collections.Generic;

namespace Microsoft.OpenApi.Services
{
    public static class OpenAPIDiff
    {
        public static List<OpenApiDifference> Compare(OpenApiDocument source, OpenApiDocument target)
        {
            var diffs = new List<OpenApiDifference>();
            return diffs;
        }
    }

    public class OpenApiDifference { }
}
