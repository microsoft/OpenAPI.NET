// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Services
{
    /// <summary>
    /// Defines behavior for comparing properties of <see cref="OpenApiPathItem"/>.
    /// </summary>
    public class OpenApiPathItemComparer : OpenApiComparerBase<OpenApiPathItem>
    {
        /// <summary>
        /// Executes comparision against source and target <see cref="OpenApiPathItem"/>.
        /// </summary>
        /// <param name="sourcePathItem">The source.</param>
        /// <param name="targetPathItem">The target.</param>
        /// <param name="comparisonContext">Context under which to compare the source and target.</param>
        public override void Compare(
            OpenApiPathItem sourcePathItem,
            OpenApiPathItem targetPathItem,
            ComparisonContext comparisonContext)
        {
            if (sourcePathItem == null && targetPathItem == null)
            {
                return;
            }

            WalkAndCompare(
                comparisonContext,
                OpenApiConstants.Summary,
                () => Compare(sourcePathItem?.Summary, targetPathItem?.Description, comparisonContext));

            WalkAndCompare(
                comparisonContext,
                OpenApiConstants.Description,
                () => Compare(sourcePathItem?.Description, targetPathItem?.Description, comparisonContext));

            comparisonContext.GetComparer<IDictionary<OperationType, OpenApiOperation>>()
                .Compare(sourcePathItem?.Operations, targetPathItem?.Operations, comparisonContext);

            // To Do Compare Servers
            // To Do Compare Parameters
            // To Do Compare Extensions
        }
    }
}