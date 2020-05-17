// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Services
{
    /// <summary>
    ///  Defines behavior for comparing properties of <see cref="OpenApiTag"/>.
    /// </summary>
    public class OpenApiTagComparer : OpenApiComparerBase<OpenApiTag>
    {
        /// <summary>
        /// Executes comparision against source and target <see cref="OpenApiTag"/>.
        /// </summary>
        /// <param name="sourceTag">The source.</param>
        /// <param name="targetTag">The target.</param>
        /// <param name="comparisonContext">Context under which to compare the source and target.</param>
        public override void Compare(OpenApiTag sourceTag, OpenApiTag targetTag, ComparisonContext comparisonContext)
        {
            if (sourceTag == null && targetTag == null)
            {
                return;
            }

            if (sourceTag == null || targetTag == null)
            {
                comparisonContext.AddOpenApiDifference(
                    new OpenApiDifference
                    {
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        SourceValue = sourceTag,
                        TargetValue = targetTag,
                        OpenApiComparedElementType = typeof(OpenApiTag),
                        Pointer = comparisonContext.PathString
                    });

                return;
            }

            WalkAndCompare(
                comparisonContext,
                OpenApiConstants.ExternalDocs,
                () => comparisonContext
                    .GetComparer<OpenApiExternalDocs>()
                    .Compare(sourceTag.ExternalDocs, targetTag.ExternalDocs, comparisonContext));

            WalkAndCompare(comparisonContext, OpenApiConstants.Description,
                () => Compare(sourceTag.Description, targetTag.Description, comparisonContext));

            WalkAndCompare(comparisonContext, OpenApiConstants.Name,
                () => Compare(sourceTag.Name, targetTag.Name, comparisonContext));
        }
    }
}
