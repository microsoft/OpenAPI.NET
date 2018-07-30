// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Services
{
    /// <summary>
    /// Defines behavior for comparing properties of <see cref="OpenApiOperation"/>.
    /// </summary>
    public class OpenApiOperationComparer : OpenApiComparerBase<OpenApiOperation>
    {
        /// <summary>
        /// Executes comparision against source and target <see cref="OpenApiOperation"/>.
        /// </summary>
        /// <param name="sourceOperation">The source.</param>
        /// <param name="targetOperation">The target.</param>
        /// <param name="comparisonContext">Context under which to compare the source and target.</param>
        public override void Compare(
            OpenApiOperation sourceOperation,
            OpenApiOperation targetOperation,
            ComparisonContext comparisonContext)
        {
            if (sourceOperation == null && targetOperation == null)
            {
                return;
            }

            WalkAndCompare(
                comparisonContext,
                OpenApiConstants.Summary,
                () => Compare(sourceOperation?.Summary, targetOperation?.Summary, comparisonContext));

            WalkAndCompare(
                comparisonContext,
                OpenApiConstants.Summary,
                () => Compare(sourceOperation?.Description, targetOperation?.Description, comparisonContext));

            WalkAndCompare(
                comparisonContext,
                OpenApiConstants.OperationId,
                () => Compare(sourceOperation?.OperationId, targetOperation?.OperationId, comparisonContext));

            WalkAndCompare(
                comparisonContext,
                OpenApiConstants.Deprecated,
                () => Compare(sourceOperation?.Deprecated, targetOperation?.Deprecated, comparisonContext));

            WalkAndCompare(comparisonContext, OpenApiConstants.Parameters,
                () => comparisonContext
                    .GetComparer<IList<OpenApiParameter>>()
                    .Compare(sourceOperation?.Parameters, targetOperation?.Parameters, comparisonContext));


            // Compare Responses
            // Compare Request Body
            // Compare CallBack
            // Compare Security Requirements
            // Compare Extensions
            // Compare Servers
            // Compare External Docs
            // Compare Tags
        }
    }
}