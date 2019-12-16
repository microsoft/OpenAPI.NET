// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Services
{
    /// <summary>
    /// Defines behavior for comparing properties of <see cref="OpenApiContact"/>.
    /// </summary>
    public class OpenApiContactComparer : OpenApiComparerBase<OpenApiContact>
    {
        /// <summary>
        /// Executes comparision against source and target <see cref="OpenApiContact"/>.
        /// </summary>
        /// <param name="sourceContact">The source.</param>
        /// <param name="targetContact">The target.</param>
        /// <param name="comparisonContext">Context under which to compare the source and target.</param>
        public override void Compare(
            OpenApiContact sourceContact,
            OpenApiContact targetContact,
            ComparisonContext comparisonContext)
        {
            if (sourceContact == null && targetContact == null)
            {
                return;
            }

            if (sourceContact == null || targetContact == null)
            {
                comparisonContext.AddOpenApiDifference(
                    new OpenApiDifference
                    {
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        SourceValue = sourceContact,
                        TargetValue = targetContact,
                        OpenApiComparedElementType = typeof(OpenApiContact),
                        Pointer = comparisonContext.PathString
                    });

                return;
            }

            WalkAndCompare(comparisonContext, OpenApiConstants.Name,
                () => Compare(sourceContact.Name, targetContact.Name, comparisonContext));

            WalkAndCompare(comparisonContext, OpenApiConstants.Email,
                () => Compare(sourceContact.Email, targetContact.Email, comparisonContext));

            WalkAndCompare(comparisonContext, OpenApiConstants.Url,
                () => Compare(sourceContact.Url, targetContact.Url, comparisonContext));
        }
    }
}