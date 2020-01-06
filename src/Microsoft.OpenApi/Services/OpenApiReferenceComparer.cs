// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Services
{
    /// <summary>
    /// Defines behavior for comparing properties of <see cref="OpenApiReference"/>.
    /// </summary>
    public class OpenApiReferenceComparer<T> : OpenApiComparerBase<OpenApiReference> where T : IOpenApiReferenceable
    {
        /// <summary>
        /// Compares <see cref="OpenApiReference"/> object.
        /// </summary>
        /// <param name="sourceReference">The source.</param>
        /// <param name="targetReference">The target.</param>
        /// <param name="comparisonContext">The context under which to compare the objects.</param>
        public override void Compare(
            OpenApiReference sourceReference,
            OpenApiReference targetReference,
            ComparisonContext comparisonContext)
        {
            if (sourceReference == null && targetReference == null)
            {
                return;
            }

            if (sourceReference == null || targetReference == null)
            {
                comparisonContext.AddOpenApiDifference(
                    new OpenApiDifference
                    {
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        SourceValue = sourceReference,
                        TargetValue = targetReference,
                        OpenApiComparedElementType = typeof(OpenApiReference),
                        Pointer = comparisonContext.PathString
                    });

                return;
            }

            if (sourceReference.Id != targetReference.Id || sourceReference.Type != targetReference.Type)
            {
                WalkAndAddOpenApiDifference(
                    comparisonContext,
                    OpenApiConstants.DollarRef,
                    new OpenApiDifference
                    {
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        SourceValue = sourceReference,
                        TargetValue = targetReference,
                        OpenApiComparedElementType = typeof(OpenApiReference)
                    });

                return;
            }

            var source = (T)comparisonContext.SourceDocument.ResolveReference(
                sourceReference);

            var target = (T)comparisonContext.TargetDocument.ResolveReference(
                targetReference);

            comparisonContext
                .GetComparer<T>()
                .Compare(source, target, comparisonContext);
        }
    }
}
