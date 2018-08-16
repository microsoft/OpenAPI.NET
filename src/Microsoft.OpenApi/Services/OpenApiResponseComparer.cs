// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Services
{
    /// <summary>
    /// Defines behavior for comparing properties of <see cref="OpenApiResponse"/>.
    /// </summary>
    public class OpenApiResponseComparer : OpenApiComparerBase<OpenApiResponse>
    {
        /// <summary>
        /// Executes comparision against source and target <see cref="OpenApiResponse"/>.
        /// </summary>
        /// <param name="sourceResponse">The source.</param>
        /// <param name="targetResponse">The target.</param>
        /// <param name="comparisonContext">Context under which to compare the source and target.</param>
        public override void Compare(
            OpenApiResponse sourceResponse,
            OpenApiResponse targetResponse,
            ComparisonContext comparisonContext)
        {
            if (sourceResponse == null && targetResponse == null)
            {
                return;
            }

            if (sourceResponse == null || targetResponse == null)
            {
                comparisonContext.AddOpenApiDifference(
                    new OpenApiDifference
                    {
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        SourceValue = sourceResponse,
                        TargetValue = targetResponse,
                        OpenApiComparedElementType = typeof(OpenApiResponse),
                        Pointer = comparisonContext.PathString
                    });

                return;
            }

            if (sourceResponse.Reference != null
                && targetResponse.Reference != null
                && sourceResponse.Reference.Id != targetResponse.Reference.Id)
            {
                WalkAndAddOpenApiDifference(
                    comparisonContext,
                    "$ref",
                    new OpenApiDifference
                    {
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        SourceValue = sourceResponse.Reference,
                        TargetValue = targetResponse.Reference,
                        OpenApiComparedElementType = typeof(OpenApiReference)
                    });

                return;
            }

            if (sourceResponse.Reference != null)
            {
                sourceResponse = (OpenApiResponse) comparisonContext.SourceDocument.ResolveReference(
                    sourceResponse.Reference);
            }

            if (targetResponse.Reference != null)
            {
                targetResponse = (OpenApiResponse) comparisonContext.TargetDocument.ResolveReference(
                    targetResponse.Reference);
            }

            WalkAndCompare(comparisonContext, OpenApiConstants.Description,
                () => Compare(sourceResponse.Description, targetResponse.Description, comparisonContext));

            WalkAndCompare(
                comparisonContext,
                OpenApiConstants.Content,
                () => comparisonContext
                    .GetComparer<IDictionary<string, OpenApiMediaType>>()
                    .Compare(sourceResponse.Content, targetResponse.Content, comparisonContext));

            WalkAndCompare(
                comparisonContext,
                OpenApiConstants.Headers,
                () => comparisonContext
                    .GetComparer<IDictionary<string, OpenApiHeader>>()
                    .Compare(sourceResponse.Headers, targetResponse.Headers, comparisonContext));

            // To Do Compare Link
            // To Do Compare Extensions
        }
    }
}