// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Services
{
    /// <summary>
    ///  Defines behavior for comparing properties of <see cref="OpenApiEncoding"/>.
    /// </summary>
    public class OpenApiEncodingComparer : OpenApiComparerBase<OpenApiEncoding>
    {
        /// <summary>
        /// Executes comparision against source and target <see cref="OpenApiEncoding"/>.
        /// </summary>
        /// <param name="sourceEncoding">The source.</param>
        /// <param name="targetEncoding">The target.</param>
        /// <param name="comparisonContext">Context under which to compare the source and target.</param>
        public override void Compare(
            OpenApiEncoding sourceEncoding,
            OpenApiEncoding targetEncoding,
            ComparisonContext comparisonContext)
        {
            if (sourceEncoding == null && targetEncoding == null)
            {
                return;
            }

            if (sourceEncoding == null || targetEncoding == null)
            {
                comparisonContext.AddOpenApiDifference(
                    new OpenApiDifference
                    {
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        SourceValue = sourceEncoding,
                        TargetValue = targetEncoding,
                        OpenApiComparedElementType = typeof(OpenApiEncoding),
                        Pointer = comparisonContext.PathString
                    });

                return;
            }

            WalkAndCompare(comparisonContext, OpenApiConstants.ContentType,
                () => Compare(sourceEncoding.ContentType, targetEncoding.ContentType, comparisonContext));

            WalkAndCompare(comparisonContext, OpenApiConstants.Explode,
                () => Compare(sourceEncoding.Explode, targetEncoding.Explode, comparisonContext));

            WalkAndCompare(comparisonContext, OpenApiConstants.AllowReserved,
                () => Compare(sourceEncoding.AllowReserved, targetEncoding.AllowReserved, comparisonContext));

            WalkAndCompare(comparisonContext, OpenApiConstants.Style,
                () => Compare<ParameterStyle>(sourceEncoding.Style, targetEncoding.Style, comparisonContext));

            WalkAndCompare(
                comparisonContext,
                OpenApiConstants.Headers,
                () => comparisonContext
                    .GetComparer<IDictionary<string, OpenApiHeader>>()
                    .Compare(sourceEncoding.Headers, targetEncoding.Headers, comparisonContext));
        }
    }
}
