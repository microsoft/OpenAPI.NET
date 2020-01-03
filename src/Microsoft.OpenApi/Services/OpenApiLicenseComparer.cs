// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Services
{
    /// <summary>
    /// Defines behavior for comparing properties of <see cref="OpenApiLicense"/>.
    /// </summary>
    public class OpenApiLicenseComparer : OpenApiComparerBase<OpenApiLicense>
    {
        /// <summary>
        /// Executes comparision against source and target <see cref="OpenApiLicense"/>.
        /// </summary>
        /// <param name="sourceLicense">The source.</param>
        /// <param name="targetLicense">The target.</param>
        /// <param name="comparisonContext">Context under which to compare the source and target.</param>
        public override void Compare(
            OpenApiLicense sourceLicense,
            OpenApiLicense targetLicense,
            ComparisonContext comparisonContext)
        {
            if (sourceLicense == null && targetLicense == null)
            {
                return;
            }

            if (sourceLicense == null || targetLicense == null)
            {
                comparisonContext.AddOpenApiDifference(
                    new OpenApiDifference
                    {
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        SourceValue = sourceLicense,
                        TargetValue = targetLicense,
                        OpenApiComparedElementType = typeof(OpenApiLicense),
                        Pointer = comparisonContext.PathString
                    });

                return;
            }

            WalkAndCompare(comparisonContext, OpenApiConstants.Name,
                () => Compare(sourceLicense.Name, targetLicense.Name, comparisonContext));

            WalkAndCompare(comparisonContext, OpenApiConstants.Url,
                () => Compare(sourceLicense.Url, targetLicense.Url, comparisonContext));
        }
    }
}