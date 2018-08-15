// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Services
{
    /// <summary>
    /// Defines behavior for comparing properties of <see cref="OpenApiParameter"/>.
    /// </summary>
    public class OpenApiParameterComparer : OpenApiComparerBase<OpenApiParameter>
    {
        /// <summary>
        /// Executes comparision against source and target <see cref="OpenApiParameter"/>.
        /// </summary>
        /// <param name="sourceParameter">The source.</param>
        /// <param name="targetParameter">The target.</param>
        /// <param name="comparisonContext">Context under which to compare the source and target.</param>
        public override void Compare(
            OpenApiParameter sourceParameter,
            OpenApiParameter targetParameter,
            ComparisonContext comparisonContext)
        {
            if (sourceParameter == null && targetParameter == null)
            {
            }

            // To Do Compare Schema
            // To Do Compare Content
            // To Do Compare Examples
            // To Do Compare parameter as IOpenApiExtensible
        }
    }
}