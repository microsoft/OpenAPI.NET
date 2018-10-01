// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Services
{
    /// <summary>
    /// Defines behavior for comparing properties of <see cref="OpenApiComponents"/>.
    /// </summary>
    public class OpenApiComponentsComparer : OpenApiComparerBase<OpenApiComponents>
    {
        /// <summary>
        /// Executes comparision against source and target <see cref="OpenApiComponents"/>.
        /// </summary>
        /// <param name="sourceComponents">The source.</param>
        /// <param name="targetComponents">The target.</param>
        /// <param name="comparisonContext">Context under which to compare the source and target.</param>
        public override void Compare(
            OpenApiComponents sourceComponents,
            OpenApiComponents targetComponents,
            ComparisonContext comparisonContext)
        {
            if (sourceComponents == null && targetComponents == null)
            {
                return;
            }

            if (sourceComponents == null || targetComponents == null)
            {
                comparisonContext.AddOpenApiDifference(
                    new OpenApiDifference
                    {
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        SourceValue = sourceComponents,
                        TargetValue = targetComponents,
                        OpenApiComparedElementType = typeof(OpenApiComponents),
                        Pointer = comparisonContext.PathString
                    });

                return;
            }

            WalkAndCompare(
                comparisonContext,
                OpenApiConstants.Parameters,
                () => comparisonContext
                    .GetComparer<IDictionary<string, OpenApiParameter>>()
                    .Compare(sourceComponents.Parameters, targetComponents.Parameters, comparisonContext));

            WalkAndCompare(
                comparisonContext,
                OpenApiConstants.RequestBodies,
                () => comparisonContext
                    .GetComparer<IDictionary<string, OpenApiRequestBody>>()
                    .Compare(sourceComponents.RequestBodies, targetComponents.RequestBodies, comparisonContext));

            WalkAndCompare(
                comparisonContext,
                OpenApiConstants.Responses,
                () => comparisonContext
                    .GetComparer<IDictionary<string, OpenApiResponse>>()
                    .Compare(sourceComponents.Responses, targetComponents.Responses, comparisonContext));

            WalkAndCompare(
                comparisonContext,
                OpenApiConstants.Schemas,
                () => comparisonContext
                    .GetComparer<IDictionary<string, OpenApiSchema>>()
                    .Compare(sourceComponents.Schemas, targetComponents.Schemas, comparisonContext));

            WalkAndCompare(
                comparisonContext,
                OpenApiConstants.Headers,
                () => comparisonContext
                    .GetComparer<IDictionary<string, OpenApiHeader>>()
                    .Compare(sourceComponents.Headers, targetComponents.Headers, comparisonContext));

            // To Do compare Examples
            // To Do compare SecuritySchemes
            // To Do compare Links
            // To Do compare Callbacks
            // To Do compare Extensions
        }
    }
}