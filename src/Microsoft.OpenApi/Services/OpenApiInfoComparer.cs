// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Services
{
    /// <summary>
    /// Defines behavior for comparing properties of <see cref="OpenApiInfo"/>.
    /// </summary>
    public class OpenApiInfoComparer : OpenApiComparerBase<OpenApiInfo>
    {
        /// <summary>
        /// Executes comparision against source and target <see cref="OpenApiInfo"/>.
        /// </summary>
        /// <param name="sourceInfo">The source.</param>
        /// <param name="targetInfo">The target.</param>
        /// <param name="comparisonContext">Context under which to compare the source and target.</param>
        public override void Compare(
            OpenApiInfo sourceInfo,
            OpenApiInfo targetInfo,
            ComparisonContext comparisonContext)
        {
            if (sourceInfo == null && targetInfo == null)
            {
                return;
            }

            if (sourceInfo == null || targetInfo == null)
            {
                comparisonContext.AddOpenApiDifference(
                    new OpenApiDifference
                    {
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        SourceValue = sourceInfo,
                        TargetValue = targetInfo,
                        OpenApiComparedElementType = typeof(OpenApiInfo),
                        Pointer = comparisonContext.PathString
                    });

                return;
            }

            WalkAndCompare(comparisonContext, OpenApiConstants.Title,
                () => Compare(sourceInfo.Title, targetInfo.Title, comparisonContext));

            WalkAndCompare(comparisonContext, OpenApiConstants.Description,
                () => Compare(sourceInfo.Description, targetInfo.Description, comparisonContext));

            WalkAndCompare(comparisonContext, OpenApiConstants.TermsOfService,
                () => Compare(sourceInfo.TermsOfService, targetInfo.TermsOfService, comparisonContext));

            WalkAndCompare(comparisonContext, OpenApiConstants.Version,
                () => Compare(sourceInfo.Version, targetInfo.Version, comparisonContext));

            WalkAndCompare(
                comparisonContext,
                OpenApiConstants.Contact,
                () => comparisonContext
                    .GetComparer<OpenApiContact>()
                    .Compare(sourceInfo.Contact, targetInfo.Contact, comparisonContext));

            WalkAndCompare(
                comparisonContext,
                OpenApiConstants.License,
                () => comparisonContext
                    .GetComparer<OpenApiLicense>()
                    .Compare(sourceInfo.License, targetInfo.License, comparisonContext));
        }
    }
}