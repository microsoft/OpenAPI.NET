// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using Microsoft.OpenApi.Any;
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

            WalkAndCompare(
                comparisonContext,
                OpenApiConstants.Schema,
                () => comparisonContext
                    .GetComparer<OpenApiSchema>()
                    .Compare(sourceMediaType.Schema, targetMediaType.Schema, comparisonContext));

            WalkAndCompare(
                comparisonContext,
                OpenApiConstants.Encoding,
                () => comparisonContext
                    .GetComparer<IDictionary<string, OpenApiEncoding>>()
                    .Compare(sourceMediaType.Encoding, sourceMediaType.Encoding, comparisonContext));

            WalkAndCompare(
                comparisonContext,
                OpenApiConstants.Examples,
                () => comparisonContext
                    .GetComparer<IDictionary<string, OpenApiExample>>()
                    .Compare(sourceMediaType.Examples, targetMediaType.Examples, comparisonContext));

            WalkAndCompare(
                comparisonContext,
                OpenApiConstants.Example,
                () => comparisonContext
                    .GetComparer<IOpenApiAny>()
                    .Compare(sourceMediaType.Example, targetMediaType.Example, comparisonContext));
        }
    }
}