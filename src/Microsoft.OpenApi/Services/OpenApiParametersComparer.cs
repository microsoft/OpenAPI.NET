// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Services
{
    /// <summary>
    /// Defines behavior for comparing properties of <see cref="IList{T}"/>
    /// where T is<see cref="OpenApiParameter"/>.
    /// </summary>
    public class OpenApiParametersComparer : OpenApiComparerBase<IList<OpenApiParameter>>
    {
        /// <summary>
        /// Executes comparision against source and target <see cref="IList{T}"/>
        /// where T is<see cref="OpenApiParameter"/>.
        /// </summary>
        /// <param name="sourceParameters">The source.</param>
        /// <param name="targetParameters">The target.</param>
        /// <param name="comparisonContext">Context under which to compare the source and target.</param>
        public override void Compare(
            IList<OpenApiParameter> sourceParameters,
            IList<OpenApiParameter> targetParameters,
            ComparisonContext comparisonContext)
        {
            if (sourceParameters == null && targetParameters == null)
            {
                return;
            }

            if (sourceParameters == null || targetParameters == null)
            {
                comparisonContext.AddOpenApiDifference(
                    new OpenApiDifference
                    {
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        SourceValue = sourceParameters,
                        TargetValue = targetParameters,
                        OpenApiComparedElementType = typeof(IList<OpenApiParameter>),
                        Pointer = comparisonContext.PathString
                    });

                return;
            }

            var removedParameters = sourceParameters?.Where(
                sourceParam => !targetParameters.Any(
                    targetParam => sourceParam.Name == targetParam.Name && sourceParam.In == targetParam.In)).ToList();

            for (var i = removedParameters.Count - 1; i >= 0; i--)
            {
                WalkAndAddOpenApiDifference(
                    comparisonContext,
                    sourceParameters.IndexOf(removedParameters[i]).ToString(),
                    new OpenApiDifference
                    {
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Remove,
                        SourceValue = removedParameters[i],
                        OpenApiComparedElementType = typeof(OpenApiParameter)
                    });
            }

            var newParametersInTarget = targetParameters?.Where(
                targetParam => !sourceParameters.Any(
                    sourceParam => sourceParam.Name == targetParam.Name && sourceParam.In == targetParam.In)).ToList();

            foreach (var newParameterInTarget in newParametersInTarget)
            {
                WalkAndAddOpenApiDifference(
                    comparisonContext,
                    targetParameters.IndexOf(newParameterInTarget).ToString(),
                    new OpenApiDifference
                    {
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Add,
                        TargetValue = newParameterInTarget,
                        OpenApiComparedElementType = typeof(OpenApiParameter)
                    });
            }

            foreach (var sourceParameter in sourceParameters)
            {
                var targetParameter = targetParameters
                    .FirstOrDefault(param => param.Name == sourceParameter.Name && param.In == sourceParameter.In);

                if (targetParameter == null)
                {
                    continue;
                }

                WalkAndCompare(
                    comparisonContext,
                    targetParameters.IndexOf(targetParameter).ToString(),
                    () => comparisonContext
                        .GetComparer<OpenApiParameter>()
                        .Compare(sourceParameter, targetParameter, comparisonContext));
            }
        }
    }
}