// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
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
        /// <param name="targetDocument">The target.</param>
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

            WalkAndCompare(
                comparisonContext,
                OpenApiConstants.Components,
                () => comparisonContext
                    .GetComparer<OpenApiComponents>()
                    .Compare(sourceDocument.Components, targetDocument.Components, comparisonContext));

            WalkAndCompare(
                comparisonContext,
                OpenApiConstants.Components,
                () => comparisonContext
                    .GetComparer<IList<OpenApiServer>>()
                    .Compare(sourceDocument.Servers, targetDocument.Servers, comparisonContext));

            WalkAndCompare(
                comparisonContext,
                OpenApiConstants.Info,
                () => comparisonContext
                    .GetComparer<OpenApiInfo>()
                    .Compare(sourceDocument.Info, targetDocument.Info, comparisonContext));

            WalkAndCompare(
                comparisonContext,
                OpenApiConstants.Security,
                () => comparisonContext
                    .GetComparer<IList<OpenApiSecurityRequirement>>()
                    .Compare(sourceDocument.SecurityRequirements, targetDocument.SecurityRequirements,
                        comparisonContext));

            WalkAndCompare(
                comparisonContext,
                OpenApiConstants.Tags,
                () => comparisonContext
                    .GetComparer<IList<OpenApiTag>>()
                    .Compare(sourceDocument.Tags, targetDocument.Tags, comparisonContext));

            WalkAndCompare(
                comparisonContext,
                OpenApiConstants.ExternalDocs,
                () => comparisonContext
                    .GetComparer<OpenApiExternalDocs>()
                    .Compare(sourceDocument.ExternalDocs, targetDocument.ExternalDocs, comparisonContext));
        }
    }
}
