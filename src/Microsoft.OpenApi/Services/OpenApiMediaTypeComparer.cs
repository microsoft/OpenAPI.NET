// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Services
{
    /// <summary>
    /// Defines behavior for comparing properties of <see cref="OpenApiMediaType"/>.
    /// </summary>
    public class OpenApiMediaTypeComparer : OpenApiComparerBase<OpenApiMediaType>
    {
        /// <summary>
        /// Executes comparision against source and target <see cref="OpenApiMediaType"/>.
        /// </summary>
        /// <param name="sourceMediaType">The source.</param>
        /// <param name="targetMediaType">The target.</param>
        /// <param name="comparisonContext">Context under which to compare the source and target.</param>
        public override void Compare(
            OpenApiMediaType sourceMediaType,
            OpenApiMediaType targetMediaType,
            ComparisonContext comparisonContext)
        {
            if (sourceMediaType == null && targetMediaType == null)
            {
                return;
            }

            if (sourceMediaType == null || targetMediaType == null)
            {
                comparisonContext.AddOpenApiDifference(
                    new OpenApiDifference
                    {
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        SourceValue = sourceMediaType,
                        TargetValue = targetMediaType,
                        OpenApiComparedElementType = typeof(OpenApiMediaType),
                        Pointer = comparisonContext.PathString
                    });

                return;
            }

            comparisonContext
                .GetComparer<OpenApiSchema>()
                .Compare(sourceMediaType.Schema, targetMediaType.Schema, comparisonContext);

            WalkAndCompare(
                comparisonContext,
                OpenApiConstants.Encoding,
                () => comparisonContext
                    .GetComparer<IDictionary<string, OpenApiEncoding>>()
                    .Compare(sourceMediaType.Encoding, sourceMediaType.Encoding, comparisonContext));

            // To Do Compare Example
            // To Do Compare Examples
            // To Do Compare Extensions
        }
    }
}