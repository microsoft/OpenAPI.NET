// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Services
{
    /// <summary>
    /// Defines behavior for comparing properties of <see cref="IDictionary{TKey,TValue}"/>
    /// where TKey is<see cref="OperationType"/> and TValue is <see cref="OpenApiOperation"/>.
    /// </summary>
    public class OpenApiOperationsComparer : OpenApiComparerBase<IDictionary<OperationType, OpenApiOperation>>
    {
        /// <summary>
        /// Executes comparision against source and target <see cref="IDictionary{TKey,TValue}"/>
        /// where TKey is<see cref="OperationType"/> and TValue is <see cref="OpenApiOperation"/>.
        /// </summary>
        /// <param name="sourceOperations">The source.</param>
        /// <param name="targetOperations">The target.</param>
        /// <param name="comparisonContext">Context under which to compare the source and target.</param>
        public override void Compare(
            IDictionary<OperationType, OpenApiOperation> sourceOperations,
            IDictionary<OperationType, OpenApiOperation> targetOperations,
            ComparisonContext comparisonContext)
        {
            if (sourceOperations == null && targetOperations == null)
            {
                return;
            }

            if (sourceOperations == null || targetOperations == null)
            {
                comparisonContext.AddOpenApiDifference(
                    new OpenApiDifference
                    {
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        SourceValue = sourceOperations,
                        TargetValue = targetOperations,
                        OpenApiComparedElementType = typeof(IDictionary<OperationType, OpenApiOperation>),
                        Pointer = comparisonContext.PathString
                    });

                return;
            }

            var newOperationKeysInTarget = targetOperations.Keys.Except(sourceOperations.Keys).ToList();

            foreach (var newOperationKeyInTarget in newOperationKeysInTarget)
            {
                WalkAndAddOpenApiDifference(
                    comparisonContext,
                    newOperationKeyInTarget.GetDisplayName(),
                    new OpenApiDifference
                    {
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Add,
                        TargetValue = new KeyValuePair<OperationType, OpenApiOperation>(
                            newOperationKeyInTarget,
                            targetOperations[newOperationKeyInTarget]),
                        OpenApiComparedElementType = typeof(KeyValuePair<OperationType, OpenApiOperation>)
                    });
            }

            foreach (var sourceOperation in sourceOperations)
            {
                if (targetOperations.Keys.Contains(sourceOperation.Key))
                {
                    WalkAndCompare(comparisonContext, sourceOperation.Key.GetDisplayName(),
                        () => comparisonContext
                            .GetComparer<OpenApiOperation>()
                            .Compare(sourceOperation.Value, targetOperations[sourceOperation.Key], comparisonContext));
                }
                else
                {
                    WalkAndAddOpenApiDifference(
                        comparisonContext,
                        sourceOperation.Key.GetDisplayName(),
                        new OpenApiDifference
                        {
                            OpenApiDifferenceOperation = OpenApiDifferenceOperation.Remove,
                            SourceValue = sourceOperation,
                            OpenApiComparedElementType = typeof(KeyValuePair<OperationType, OpenApiOperation>)
                        });
                }
            }
        }
    }
}