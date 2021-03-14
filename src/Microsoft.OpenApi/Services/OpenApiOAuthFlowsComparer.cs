// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Services
{
    /// <summary>
    /// Defines behavior for comparing properties of <see cref="OpenApiOAuthFlows"/>.
    /// </summary>
    public class OpenApiOAuthFlowsComparer : OpenApiComparerBase<OpenApiOAuthFlows>
    {
        /// <summary>
        /// Executes comparision against source and target <see cref="OpenApiOAuthFlows"/>.
        /// </summary>
        /// <param name="sourceFlows">The source.</param>
        /// <param name="targetFlows">The target.</param>
        /// <param name="comparisonContext">Context under which to compare the source and target.</param>
        public override void Compare(
            OpenApiOAuthFlows sourceFlows,
            OpenApiOAuthFlows targetFlows,
            ComparisonContext comparisonContext)
        {
            if (sourceFlows == null && targetFlows == null)
            {
                return;
            }

            if (sourceFlows == null || targetFlows == null)
            {
                comparisonContext.AddOpenApiDifference(
                    new OpenApiDifference
                    {
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        SourceValue = sourceFlows,
                        TargetValue = targetFlows,
                        OpenApiComparedElementType = typeof(OpenApiOAuthFlows),
                        Pointer = comparisonContext.PathString
                    });

                return;
            }

            WalkAndCompare(
                comparisonContext,
                OpenApiConstants.Implicit,
                () => comparisonContext
                    .GetComparer<OpenApiOAuthFlow>()
                    .Compare(sourceFlows.Implicit, targetFlows.Implicit, comparisonContext));

            WalkAndCompare(
                comparisonContext,
                OpenApiConstants.Password,
                () => comparisonContext
                    .GetComparer<OpenApiOAuthFlow>()
                    .Compare(sourceFlows.Password, targetFlows.Password, comparisonContext));

            WalkAndCompare(
                comparisonContext,
                OpenApiConstants.ClientCredentials,
                () => comparisonContext
                    .GetComparer<OpenApiOAuthFlow>()
                    .Compare(sourceFlows.ClientCredentials, targetFlows.ClientCredentials, comparisonContext));

            WalkAndCompare(
                comparisonContext,
                OpenApiConstants.AuthorizationCode,
                () => comparisonContext
                    .GetComparer<OpenApiOAuthFlow>()
                    .Compare(sourceFlows.AuthorizationCode, targetFlows.AuthorizationCode, comparisonContext));
        }
    }
}
