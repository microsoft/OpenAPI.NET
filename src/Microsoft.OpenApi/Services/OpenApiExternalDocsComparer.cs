// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Services
{
    /// <summary>
    ///  Defines behavior for comparing properties of <see cref="OpenApiExternalDocs"/>.
    /// </summary>
    public class OpenApiExternalDocsComparer : OpenApiComparerBase<OpenApiExternalDocs>
    {
        /// <summary>
        /// Executes comparision against source and target <see cref="OpenApiExternalDocs"/>.
        /// </summary>
        /// <param name="sourceDocs">The source.</param>
        /// <param name="targetDocs">The target.</param>
        /// <param name="comparisonContext">Context under which to compare the source and target.</param>
        public override void Compare(OpenApiExternalDocs sourceDocs, OpenApiExternalDocs targetDocs,
            ComparisonContext comparisonContext)
        {
            if (sourceDocs == null && targetDocs == null)
            {
                return;
            }

            if (sourceDocs == null || targetDocs == null)
            {
                comparisonContext.AddOpenApiDifference(
                    new OpenApiDifference
                    {
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        SourceValue = sourceDocs,
                        TargetValue = targetDocs,
                        OpenApiComparedElementType = typeof(OpenApiExternalDocs),
                        Pointer = comparisonContext.PathString
                    });
                return;
            }

            WalkAndCompare(comparisonContext, OpenApiConstants.Description,
                () => Compare(sourceDocs.Description, targetDocs.Description, comparisonContext));

            WalkAndCompare(comparisonContext, OpenApiConstants.Url,
                () => Compare(sourceDocs.Url, targetDocs.Url, comparisonContext));
        }
    }
}