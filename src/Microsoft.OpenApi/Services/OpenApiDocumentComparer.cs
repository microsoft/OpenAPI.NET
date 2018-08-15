// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Services
{
    /// <summary>
    /// Defines behavior for comparing properties of <see cref="OpenApiDocument"/>.
    /// </summary>
    public class OpenApiDocumentComparer : OpenApiComparerBase<OpenApiDocument>
    {
        /// <summary>
        /// Executes comparision against source and target <see cref="OpenApiDocument"/>.
        /// </summary>
        /// <param name="sourceDocument">The source.</param>
        /// <param name="targetDocument">The target</param>
        /// <param name="comparisonContext">Context under which to compare the source and target.</param>
        public override void Compare(
            OpenApiDocument sourceDocument,
            OpenApiDocument targetDocument,
            ComparisonContext comparisonContext)
        {
            WalkAndCompare(
                comparisonContext,
                OpenApiConstants.Paths,
                () => comparisonContext
                    .GetComparer<OpenApiPaths>()
                    .Compare(sourceDocument.Paths, targetDocument.Paths, comparisonContext));

            // To Do Compare Info
            // To Do Compare Servers
            // To Do Compare Components
            // To Do Compare Security Requirements
            // To Do Compare Tags
            // To Do Compare External Docs
            // To Do Compare Extensions
        }
    }
}