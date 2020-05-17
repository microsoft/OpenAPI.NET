// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Services
{
    /// <summary>
    /// Defines behavior for comparing properties of <see cref="OpenApiOAuthFlow"/>.
    /// </summary>
    public class OpenApiOAuthFlowComparer : OpenApiComparerBase<OpenApiOAuthFlow>
    {
        /// <summary>
        /// Executes comparision against source and target <see cref="OpenApiOAuthFlow"/>.
        /// </summary>
        /// <param name="sourceFlow">The source.</param>
        /// <param name="targetFlow">The target.</param>
        /// <param name="comparisonContext">Context under which to compare the source and target.</param>
        public override void Compare(OpenApiOAuthFlow sourceFlow, OpenApiOAuthFlow targetFlow,
            ComparisonContext comparisonContext)
        {
            if (sourceFlow == null && targetFlow == null)
            {
                return;
            }

            if (sourceFlow == null || targetFlow == null)
            {
                comparisonContext.AddOpenApiDifference(
                    new OpenApiDifference
                    {
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        SourceValue = sourceFlow,
                        TargetValue = targetFlow,
                        OpenApiComparedElementType = typeof(OpenApiOAuthFlow),
                        Pointer = comparisonContext.PathString
                    });

                return;
            }

            WalkAndCompare(comparisonContext, OpenApiConstants.AuthorizationUrl,
                () => Compare(sourceFlow.AuthorizationUrl, targetFlow.AuthorizationUrl, comparisonContext));

            WalkAndCompare(comparisonContext, OpenApiConstants.TokenUrl,
                () => Compare(sourceFlow.TokenUrl, targetFlow.TokenUrl, comparisonContext));

            WalkAndCompare(comparisonContext, OpenApiConstants.RefreshUrl,
                () => Compare(sourceFlow.RefreshUrl, targetFlow.RefreshUrl, comparisonContext));

            WalkAndCompare(comparisonContext, OpenApiConstants.Scopes,
                () => Compare(sourceFlow.Scopes, targetFlow.Scopes, comparisonContext));
        }
    }
}
