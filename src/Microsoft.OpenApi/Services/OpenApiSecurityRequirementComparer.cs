// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Services
{
    /// <summary>
    /// Defines behavior for comparing properties of <see cref="OpenApiSecurityRequirement"/>.
    /// </summary>
    public class OpenApiSecurityRequirementComparer : OpenApiComparerBase<OpenApiSecurityRequirement>
    {
        /// <summary>
        /// Executes comparision against source and target <see cref="OpenApiSecurityRequirement"/>.
        /// </summary>
        /// <param name="sourceSecurityRequirement">The source.</param>
        /// <param name="targetSecurityRequirement">The target.</param>
        /// <param name="comparisonContext">Context under which to compare the source and target.</param>
        public override void Compare(
            OpenApiSecurityRequirement sourceSecurityRequirement,
            OpenApiSecurityRequirement targetSecurityRequirement,
            ComparisonContext comparisonContext)
        {
            if (sourceSecurityRequirement == null && targetSecurityRequirement == null)
            {
                return;
            }

            if (sourceSecurityRequirement == null || targetSecurityRequirement == null)
            {
                comparisonContext.AddOpenApiDifference(
                    new OpenApiDifference
                    {
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        SourceValue = sourceSecurityRequirement,
                        TargetValue = targetSecurityRequirement,
                        OpenApiComparedElementType = typeof(OpenApiSecurityRequirement),
                        Pointer = comparisonContext.PathString
                    });

                return;
            }

            var newSecuritySchemesInTarget = targetSecurityRequirement.Keys
                .Where(targetReq => sourceSecurityRequirement.Keys.All(
                    sourceReq => sourceReq.Reference.Id != targetReq.Reference.Id)).ToList();

            foreach (var newSecuritySchemeInTarget in newSecuritySchemesInTarget)
            {
                WalkAndAddOpenApiDifference(
                    comparisonContext,
                    newSecuritySchemeInTarget.Reference.Id,
                    new OpenApiDifference
                    {
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Add,
                        TargetValue = targetSecurityRequirement[newSecuritySchemeInTarget],
                        OpenApiComparedElementType = typeof(IList<string>)
                    });
            }

            foreach (var sourceSecurityScheme in sourceSecurityRequirement.Keys)
            {
                var targetSecurityScheme =
                    targetSecurityRequirement.Keys.FirstOrDefault(
                        i => i.Reference.Id == sourceSecurityScheme.Reference.Id);

                if (targetSecurityScheme == null)
                {
                    WalkAndAddOpenApiDifference(
                        comparisonContext,
                        sourceSecurityScheme.Reference.Id,
                        new OpenApiDifference
                        {
                            OpenApiDifferenceOperation = OpenApiDifferenceOperation.Remove,
                            SourceValue = sourceSecurityRequirement[sourceSecurityScheme],
                            OpenApiComparedElementType = typeof(IList<string>)
                        });
                }
                else
                {
                    WalkAndCompare(comparisonContext,
                        sourceSecurityScheme.Reference.Id,
                        () => comparisonContext
                            .GetComparer<OpenApiSecurityScheme>()
                            .Compare(sourceSecurityScheme, targetSecurityScheme, comparisonContext));
                }
            }
        }
    }
}