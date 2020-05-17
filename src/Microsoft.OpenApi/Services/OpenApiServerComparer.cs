// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Services
{
    /// <summary>
    ///  Defines behavior for comparing properties of <see cref="OpenApiServer"/>.
    /// </summary>
    public class OpenApiServerComparer : OpenApiComparerBase<OpenApiServer>
    {
        /// <summary>
        /// Executes comparision against source and target <see cref="OpenApiServer"/>.
        /// </summary>
        /// <param name="sourceServer">The source.</param>
        /// <param name="targetServer">The target.</param>
        /// <param name="comparisonContext">Context under which to compare the source and target.</param>
        public override void Compare(
            OpenApiServer sourceServer,
            OpenApiServer targetServer,
            ComparisonContext comparisonContext)
        {
            if (sourceServer == null && targetServer == null)
            {
                return;
            }

            if (sourceServer == null || targetServer == null)
            {
                comparisonContext.AddOpenApiDifference(
                    new OpenApiDifference
                    {
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        SourceValue = sourceServer,
                        TargetValue = targetServer,
                        OpenApiComparedElementType = typeof(OpenApiServer),
                        Pointer = comparisonContext.PathString
                    });

                return;
            }

            WalkAndCompare(comparisonContext, OpenApiConstants.Description,
                () => Compare(sourceServer.Description, targetServer.Description, comparisonContext));

            WalkAndCompare(comparisonContext, OpenApiConstants.Url,
                () => Compare(sourceServer.Url, targetServer.Url, comparisonContext));

            WalkAndCompare(
                comparisonContext,
                OpenApiConstants.Variables,
                () => comparisonContext
                    .GetComparer<IDictionary<string, OpenApiServerVariable>>()
                    .Compare(sourceServer.Variables, sourceServer.Variables, comparisonContext));
        }
    }
}
