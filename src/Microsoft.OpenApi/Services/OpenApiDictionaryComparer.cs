// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Interfaces;

namespace Microsoft.OpenApi.Services
{
    /// <summary>
    /// Defines behavior for comparing <see cref="IDictionary{TKey,TValue}"/> where TKey is <see cref="string"/>
    /// and TValue is <see cref="IOpenApiElement"/>.
    /// </summary>
    public class OpenApiDictionaryComparer<T> : OpenApiComparerBase<IDictionary<string, T>>
        where T : IOpenApiElement
    {
        /// <summary>
        /// Executes comparision against source and target <see cref="IDictionary{TKey, TValue}"/>
        /// where TKey is <see cref="string"/> and TValue is <see cref="IOpenApiElement"/>.
        /// </summary>
        /// <param name="sourceFragment">The source.</param>
        /// <param name="targetFragment">The target.</param>
        /// <param name="comparisonContext">Context under which to compare the source and target.</param>
        public override void Compare(
            IDictionary<string, T> sourceFragment,
            IDictionary<string, T> targetFragment,
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
                        TargetValue = targetFragment,
                        OpenApiComparedElementType = typeof(IDictionary<string, T>),
                        Pointer = comparisonContext.PathString
                    });

                return;
            }

            var newKeysInTarget = targetFragment.Keys.Except(sourceFragment.Keys).ToList();

            foreach (var newKeyInTarget in newKeysInTarget)
            {
                WalkAndAddOpenApiDifference(
                    comparisonContext,
                    newKeyInTarget,
                    new OpenApiDifference
                    {
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Add,
                        TargetValue = targetFragment[newKeyInTarget],
                        OpenApiComparedElementType = typeof(T)
                    });
            }

            foreach (var source in sourceFragment)
            {
                if (targetFragment.Keys.Contains(source.Key))
                {
                    WalkAndCompare(comparisonContext, source.Key,
                        () => comparisonContext
                            .GetComparer<T>()
                            .Compare(source.Value, targetFragment[source.Key], comparisonContext));
                }
                else
                {
                    WalkAndAddOpenApiDifference(
                        comparisonContext,
                        source.Key,
                        new OpenApiDifference
                        {
                            OpenApiDifferenceOperation = OpenApiDifferenceOperation.Remove,
                            SourceValue = source.Value,
                            OpenApiComparedElementType = typeof(T)
                        });
                }
            }
        }
    }
}
