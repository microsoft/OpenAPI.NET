// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Services
{
    /// <summary>
    ///  Defines behavior for comparing properties of <see cref="OpenApiExample"/>.
    /// </summary>
    public class OpenApiExampleComparer : OpenApiComparerBase<OpenApiExample>
    {
        /// <summary>
        /// Executes comparision against source and target <see cref="OpenApiExample"/>.
        /// </summary>
        /// <param name="sourceExample">The source.</param>
        /// <param name="targetExample">The target.</param>
        /// <param name="comparisonContext">Context under which to compare the source and target.</param>
        public override void Compare(
            OpenApiExample sourceExample,
            OpenApiExample targetExample,
            ComparisonContext comparisonContext)
        {
            if (sourceExample == null && targetExample == null)
            {
                return;
            }

            if (sourceExample == null || targetExample == null)
            {
                comparisonContext.AddOpenApiDifference(
                    new OpenApiDifference
                    {
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        SourceValue = sourceExample,
                        TargetValue = targetExample,
                        OpenApiComparedElementType = typeof(OpenApiExample),
                        Pointer = comparisonContext.PathString
                    });

                return;
            }

            if (sourceExample.Reference != null
                && targetExample.Reference != null
                && sourceExample.Reference.Id != targetExample.Reference.Id)
            {
                WalkAndAddOpenApiDifference(
                    comparisonContext,
                    OpenApiConstants.DollarRef,
                    new OpenApiDifference
                    {
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        SourceValue = sourceExample.Reference,
                        TargetValue = targetExample.Reference,
                        OpenApiComparedElementType = typeof(OpenApiReference)
                    });

                return;
            }

            if (sourceExample.Reference != null)
            {
                sourceExample = (OpenApiExample)comparisonContext.SourceDocument.ResolveReference(
                    sourceExample.Reference);
            }

            if (targetExample.Reference != null)
            {
                targetExample = (OpenApiExample)comparisonContext.TargetDocument.ResolveReference(
                    targetExample.Reference);
            }

            WalkAndCompare(comparisonContext, OpenApiConstants.Description,
                () => Compare(sourceExample.Description, targetExample.Description, comparisonContext));

            WalkAndCompare(comparisonContext, OpenApiConstants.Summary,
                () => Compare(sourceExample.Summary, targetExample.Summary, comparisonContext));

            WalkAndCompare(comparisonContext, OpenApiConstants.ExternalValue,
                () => Compare(sourceExample.ExternalValue, targetExample.ExternalValue, comparisonContext));

            WalkAndCompare(
                comparisonContext,
                OpenApiConstants.Value,
                () => comparisonContext
                    .GetComparer<IOpenApiAny>()
                    .Compare(sourceExample.Value, targetExample.Value, comparisonContext));
        }
    }
}
