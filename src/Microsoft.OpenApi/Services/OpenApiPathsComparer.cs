// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Linq;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Services
{
    /// <summary>
    /// Defines behavior for comparing properties of <see cref="OpenApiPaths"/>.
    /// </summary>
    public class OpenApiPathsComparer : OpenApiComparerBase<OpenApiPaths>
    {
        /// <summary>
        /// Executes comparision against source and target <see cref="OpenApiPaths"/>.
        /// </summary>
        /// <param name="sourcePaths">The source.</param>
        /// <param name="targetPaths">The target.</param>
        /// <param name="comparisonContext">Context under which to compare the source and target.</param>
        public override void Compare(
            OpenApiPaths sourcePaths,
            OpenApiPaths targetPaths,
            ComparisonContext comparisonContext)
        {
            if (sourcePaths == null && targetPaths == null)
            {
                return;
            }

            if (sourcePaths == null || targetPaths == null)
            {
                comparisonContext.AddOpenApiDifference(
                    new OpenApiDifference
                    {
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        SourceValue = sourcePaths,
                        TargetValue = targetPaths,
                        OpenApiComparedElementType = typeof(OpenApiPaths),
                        Pointer = comparisonContext.PathString
                    });

                return;
            }

            var newPathKeysInTarget = targetPaths.Keys.Except(sourcePaths?.Keys).ToList();

            foreach (var newPathKeyInTarget in newPathKeysInTarget)
            {
                WalkAndAddOpenApiDifference(
                    comparisonContext,
                    newPathKeyInTarget,
                    new OpenApiDifference
                    {
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Add,
                        TargetValue = targetPaths[newPathKeyInTarget],
                        OpenApiComparedElementType = typeof(OpenApiPathItem)
                    });
            }

            foreach (var sourcePathKey in sourcePaths.Keys)
            {
                if (targetPaths.ContainsKey(sourcePathKey))
                {
                    WalkAndCompare(
                        comparisonContext,
                        sourcePathKey,
                        () => comparisonContext
                            .GetComparer<OpenApiPathItem>()
                            .Compare(sourcePaths[sourcePathKey], targetPaths[sourcePathKey], comparisonContext));
                }
                else
                {
                    WalkAndAddOpenApiDifference(
                        comparisonContext,
                        sourcePathKey,
                        new OpenApiDifference
                        {
                            OpenApiDifferenceOperation = OpenApiDifferenceOperation.Remove,
                            SourceValue = sourcePaths[sourcePathKey],
                            OpenApiComparedElementType = typeof(OpenApiPathItem)
                        });
                }
            }
        }
    }
}