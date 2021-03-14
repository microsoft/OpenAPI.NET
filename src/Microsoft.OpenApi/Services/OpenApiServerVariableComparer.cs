// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Services
{
    /// <summary>
    ///  Defines behavior for comparing properties of <see cref="OpenApiServerVariable"/>.
    /// </summary>
    public class OpenApiServerVariableComparer : OpenApiComparerBase<OpenApiServerVariable>
    {
        /// <summary>
        /// Executes comparision against source and target <see cref="OpenApiServerVariable"/>.
        /// </summary>
        /// <param name="sourceServerVariable">The source.</param>
        /// <param name="targetServerVariable">The target.</param>
        /// <param name="comparisonContext">Context under which to compare the source and target.</param>
        public override void Compare(
            OpenApiServerVariable sourceServerVariable,
            OpenApiServerVariable targetServerVariable,
            ComparisonContext comparisonContext)
        {
            if (sourceServerVariable == null && targetServerVariable == null)
            {
                return;
            }

            if (sourceServerVariable == null || targetServerVariable == null)
            {
                comparisonContext.AddOpenApiDifference(
                    new OpenApiDifference
                    {
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        SourceValue = sourceServerVariable,
                        TargetValue = targetServerVariable,
                        OpenApiComparedElementType = typeof(OpenApiServerVariable),
                        Pointer = comparisonContext.PathString
                    });

                return;
            }

            WalkAndCompare(comparisonContext, OpenApiConstants.Description,
                () => Compare(sourceServerVariable.Description, targetServerVariable.Description, comparisonContext));

            WalkAndCompare(comparisonContext, OpenApiConstants.Default,
                () => Compare(sourceServerVariable.Default, targetServerVariable.Default, comparisonContext));

            // To Do compare enum
            // To Do compare extensions
        }
    }
}
