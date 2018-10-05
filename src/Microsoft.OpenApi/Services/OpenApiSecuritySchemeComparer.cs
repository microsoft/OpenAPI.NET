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
        /// <param name="sourcecSecurityScheme">The source.</param>
        /// <param name="targetSecurityScheme">The target.</param>
        /// <param name="comparisonContext">Context under which to compare the source and target.</param>
        public override void Compare(
            OpenApiSecurityScheme sourcecSecurityScheme,
            OpenApiSecurityScheme targetSecurityScheme,
            ComparisonContext comparisonContext)
        {
            if (sourcecSecurityScheme == null && targetSecurityScheme == null)
            {
                return;
            }

            if (sourcecSecurityScheme == null || targetSecurityScheme == null)
            {
                comparisonContext.AddOpenApiDifference(
                    new OpenApiDifference
                    {
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        SourceValue = sourcecSecurityScheme,
                        TargetValue = targetSecurityScheme,
                        OpenApiComparedElementType = typeof(OpenApiSecurityScheme),
                        Pointer = comparisonContext.PathString
                    });

                return;
            }

            Compare<OpenApiSecurityScheme>(sourcecSecurityScheme.Reference, targetSecurityScheme.Reference,
                comparisonContext);

            WalkAndCompare(comparisonContext, OpenApiConstants.Description,
                () => Compare(sourcecSecurityScheme.Description, targetSecurityScheme.Description, comparisonContext));

            WalkAndCompare(comparisonContext, OpenApiConstants.Type,
                () => Compare<SecuritySchemeType>(sourcecSecurityScheme.Type, targetSecurityScheme.Type,
                    comparisonContext));

            WalkAndCompare(comparisonContext, OpenApiConstants.Name,
                () => Compare(sourcecSecurityScheme.Name, targetSecurityScheme.Name, comparisonContext));

            WalkAndCompare(comparisonContext, OpenApiConstants.In,
                () => Compare<ParameterLocation>(sourcecSecurityScheme.In, targetSecurityScheme.In, comparisonContext));

            WalkAndCompare(comparisonContext, OpenApiConstants.Scheme,
                () => Compare(sourcecSecurityScheme.Scheme, targetSecurityScheme.Scheme, comparisonContext));

            WalkAndCompare(comparisonContext, OpenApiConstants.BearerFormat,
                () => Compare(sourcecSecurityScheme.BearerFormat, targetSecurityScheme.BearerFormat,
                    comparisonContext));

            WalkAndCompare(comparisonContext, OpenApiConstants.OpenIdConnectUrl,
                () => Compare(sourcecSecurityScheme.OpenIdConnectUrl, targetSecurityScheme.OpenIdConnectUrl,
                    comparisonContext));

            WalkAndCompare(
                comparisonContext,
                OpenApiConstants.Flows,
                () => comparisonContext
                    .GetComparer<OpenApiOAuthFlows>()
                    .Compare(sourcecSecurityScheme.Flows, targetSecurityScheme.Flows, comparisonContext));
        }
    }
}