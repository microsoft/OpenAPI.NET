// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Services
{
    /// <summary>
    ///  Defines behavior for comparing properties of <see cref="OpenApiSecurityScheme"/>.
    /// </summary>
    public class OpenApiSecuritySchemeComparer : OpenApiComparerBase<OpenApiSecurityScheme>
    {
        /// <summary>
        /// Executes comparision against source and target <see cref="OpenApiSecurityScheme"/>.
        /// </summary>
        /// <param name="sourceSecurityScheme">The source.</param>
        /// <param name="targetSecurityScheme">The target.</param>
        /// <param name="comparisonContext">Context under which to compare the source and target.</param>
        public override void Compare(
            OpenApiSecurityScheme sourceSecurityScheme,
            OpenApiSecurityScheme targetSecurityScheme,
            ComparisonContext comparisonContext)
        {
            if (sourceSecurityScheme == null && targetSecurityScheme == null)
            {
                return;
            }

            if (sourceSecurityScheme == null || targetSecurityScheme == null)
            {
                comparisonContext.AddOpenApiDifference(
                    new OpenApiDifference
                    {
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        SourceValue = sourceSecurityScheme,
                        TargetValue = targetSecurityScheme,
                        OpenApiComparedElementType = typeof(OpenApiSecurityScheme),
                        Pointer = comparisonContext.PathString
                    });

                return;
            }

            if (sourceSecurityScheme.Reference != null
                && targetSecurityScheme.Reference != null
                && sourceSecurityScheme.Reference.Id != targetSecurityScheme.Reference.Id)
            {
                WalkAndAddOpenApiDifference(
                    comparisonContext,
                    OpenApiConstants.DollarRef,
                    new OpenApiDifference
                    {
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        SourceValue = sourceSecurityScheme.Reference,
                        TargetValue = targetSecurityScheme.Reference,
                        OpenApiComparedElementType = typeof(OpenApiReference)
                    });

                return;
            }

            if (sourceSecurityScheme.Reference != null)
            {
                sourceSecurityScheme = (OpenApiSecurityScheme)comparisonContext.SourceDocument.ResolveReference(
                    sourceSecurityScheme.Reference);
            }

            if (targetSecurityScheme.Reference != null)
            {
                targetSecurityScheme = (OpenApiSecurityScheme)comparisonContext.TargetDocument.ResolveReference(
                    targetSecurityScheme.Reference);
            }

            WalkAndCompare(comparisonContext, OpenApiConstants.Description,
                () => Compare(sourceSecurityScheme.Description, targetSecurityScheme.Description, comparisonContext));

            WalkAndCompare(comparisonContext, OpenApiConstants.Type,
                () => Compare<SecuritySchemeType>(sourceSecurityScheme.Type, targetSecurityScheme.Type,
                    comparisonContext));

            WalkAndCompare(comparisonContext, OpenApiConstants.Name,
                () => Compare(sourceSecurityScheme.Name, targetSecurityScheme.Name, comparisonContext));

            WalkAndCompare(comparisonContext, OpenApiConstants.In,
                () => Compare<ParameterLocation>(sourceSecurityScheme.In, targetSecurityScheme.In, comparisonContext));

            WalkAndCompare(comparisonContext, OpenApiConstants.Scheme,
                () => Compare(sourceSecurityScheme.Scheme, targetSecurityScheme.Scheme, comparisonContext));

            WalkAndCompare(comparisonContext, OpenApiConstants.BearerFormat,
                () => Compare(sourceSecurityScheme.BearerFormat, targetSecurityScheme.BearerFormat,
                    comparisonContext));

            WalkAndCompare(comparisonContext, OpenApiConstants.OpenIdConnectUrl,
                () => Compare(sourceSecurityScheme.OpenIdConnectUrl, targetSecurityScheme.OpenIdConnectUrl,
                    comparisonContext));

            WalkAndCompare(
                comparisonContext,
                OpenApiConstants.Flows,
                () => comparisonContext
                    .GetComparer<OpenApiOAuthFlows>()
                    .Compare(sourceSecurityScheme.Flows, targetSecurityScheme.Flows, comparisonContext));
        }
    }
}
