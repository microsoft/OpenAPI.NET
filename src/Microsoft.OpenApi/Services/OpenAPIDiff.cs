//---------------------------------------------------------------------
// <copyright file="OpenAPIDiff.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

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
