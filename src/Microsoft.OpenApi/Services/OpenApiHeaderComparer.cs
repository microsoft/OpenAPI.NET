// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Services
{
    /// <summary>
    ///  Defines behavior for comparing properties of <see cref="OpenApiHeader"/>.
    /// </summary>
    public class OpenApiHeaderComparer : OpenApiComparerBase<OpenApiHeader>
    {
        /// <summary>
        /// Executes comparision against source and target <see cref="OpenApiHeader"/>.
        /// </summary>
        /// <param name="sourceHeader">The source.</param>
        /// <param name="targetHeader">The target.</param>
        /// <param name="comparisonContext">Context under which to compare the source and target.</param>
        public override void Compare(
            OpenApiHeader sourceHeader,
            OpenApiHeader targetHeader,
            ComparisonContext comparisonContext)
        {
            if (sourceHeader == null && targetHeader == null)
            {
                return;
            }

            if (sourceHeader == null || targetHeader == null)
            {
                comparisonContext.AddOpenApiDifference(
                    new OpenApiDifference
                    {
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        SourceValue = sourceHeader,
                        TargetValue = targetHeader,
                        OpenApiComparedElementType = typeof(OpenApiHeader),
                        Pointer = comparisonContext.PathString
                    });

                return;
            }

            if (sourceHeader.Reference != null
                && targetHeader.Reference != null
                && sourceHeader.Reference.Id != targetHeader.Reference.Id)
            {
                WalkAndAddOpenApiDifference(
                    comparisonContext,
                    OpenApiConstants.DollarRef,
                    new OpenApiDifference
                    {
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        SourceValue = sourceHeader.Reference,
                        TargetValue = targetHeader.Reference,
                        OpenApiComparedElementType = typeof(OpenApiReference)
                    });

                return;
            }

            if (sourceHeader.Reference != null)
            {
                sourceHeader = (OpenApiHeader)comparisonContext.SourceDocument.ResolveReference(
                    sourceHeader.Reference);
            }

            if (targetHeader.Reference != null)
            {
                targetHeader = (OpenApiHeader)comparisonContext.TargetDocument.ResolveReference(
                    targetHeader.Reference);
            }

            WalkAndCompare(comparisonContext, OpenApiConstants.Description,
                () => Compare(sourceHeader.Description, targetHeader.Description, comparisonContext));

            WalkAndCompare(comparisonContext, OpenApiConstants.Required,
                () => Compare(sourceHeader.Required, targetHeader.Required, comparisonContext));

            WalkAndCompare(comparisonContext, OpenApiConstants.Deprecated,
                () => Compare(sourceHeader.Deprecated, targetHeader.Deprecated, comparisonContext));

            WalkAndCompare(comparisonContext, OpenApiConstants.AllowEmptyValue,
                () => Compare(sourceHeader.AllowEmptyValue, targetHeader.AllowEmptyValue, comparisonContext));

            WalkAndCompare(comparisonContext, OpenApiConstants.Explode,
                () => Compare(sourceHeader.Explode, targetHeader.Explode, comparisonContext));

            WalkAndCompare(comparisonContext, OpenApiConstants.AllowReserved,
                () => Compare(sourceHeader.AllowReserved, targetHeader.AllowReserved, comparisonContext));

            WalkAndCompare(
                comparisonContext,
                OpenApiConstants.Content,
                () => comparisonContext
                    .GetComparer<IDictionary<string, OpenApiMediaType>>()
                    .Compare(sourceHeader.Content, targetHeader.Content, comparisonContext));

            WalkAndCompare(
                comparisonContext,
                OpenApiConstants.Schema,
                () => comparisonContext
                    .GetComparer<OpenApiSchema>()
                    .Compare(sourceHeader.Schema, targetHeader.Schema, comparisonContext));

            WalkAndCompare(
                comparisonContext,
                OpenApiConstants.Examples,
                () => comparisonContext
                    .GetComparer<IDictionary<string, OpenApiExample>>()
                    .Compare(sourceHeader.Examples, targetHeader.Examples, comparisonContext));

            WalkAndCompare(
                comparisonContext,
                OpenApiConstants.Example,
                () => comparisonContext
                    .GetComparer<IOpenApiAny>()
                    .Compare(sourceHeader.Example, targetHeader.Example, comparisonContext));
        }
    }
}
