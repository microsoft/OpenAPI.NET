// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using Microsoft.OpenApi.Interfaces;

namespace Microsoft.OpenApi.Services
{
    /// <summary>
    /// Defines behavior for comparing <see cref="IList{T}"/> where T is <see cref="IOpenApiElement"/>.
    /// </summary>
    public class OpenApiOrderedListComparer<T> : OpenApiComparerBase<IList<T>> where T : IOpenApiElement
    {
        /// <summary>
        /// Executes comparision against based on the order of the list for source and target <see cref="IList{T}"/>
        /// where T is <see cref="IOpenApiElement"/>.
        /// </summary>
        /// <param name="sourceFragment">The source.</param>
        /// <param name="targetFragment">The target.</param>
        /// <param name="comparisonContext">Context under which to compare the source and target.</param>
        public override void Compare(
            IList<T> sourceFragment,
            IList<T> targetFragment,
            ComparisonContext comparisonContext)
        {
            if (sourceFragment == null && targetFragment == null)
            {
                return;
            }

            if (sourceFragment == null || targetFragment == null)
            {
                comparisonContext.AddOpenApiDifference(
                    new OpenApiDifference
                    {
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        SourceValue = sourceFragment,
                        TargetValue = sourceFragment,
                        OpenApiComparedElementType = typeof(IList<T>),
                        Pointer = comparisonContext.PathString
                    });

                return;
            }

            for (var i = 0; i < sourceFragment.Count; i++)
            {
                if (i >= targetFragment.Count)
                {
                    WalkAndAddOpenApiDifference(
                        comparisonContext,
                        i.ToString(),
                        new OpenApiDifference
                        {
                            OpenApiDifferenceOperation = OpenApiDifferenceOperation.Remove,
                            SourceValue = sourceFragment[i],
                            OpenApiComparedElementType = typeof(T)
                        });
                }
                else
                {
                    WalkAndCompare(comparisonContext,
                        i.ToString(),
                        () => comparisonContext
                            .GetComparer<T>()
                            .Compare(sourceFragment[i], targetFragment[i], comparisonContext));
                }
            }

            if (targetFragment.Count <= sourceFragment.Count)
            {
                return;
            }

            // Loop through remaining elements in target that are not in source.
            for (var i = sourceFragment.Count; i < targetFragment.Count; i++)
            {
                WalkAndAddOpenApiDifference(
                    comparisonContext,
                    i.ToString(),
                    new OpenApiDifference
                    {
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Add,
                        TargetValue = targetFragment[i],
                        OpenApiComparedElementType = typeof(T)
                    });
            }
        }
    }
}
