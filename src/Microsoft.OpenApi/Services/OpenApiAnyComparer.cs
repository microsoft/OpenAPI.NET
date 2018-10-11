// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.IO;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Services
{
    /// <summary>
    ///  Defines behavior for comparing properties of <see cref="IOpenApiAny"/>.
    /// </summary>
    public class OpenApiAnyComparer : OpenApiComparerBase<IOpenApiAny>
    {
        /// <summary>
        /// Executes comparision against source and target <see cref="IOpenApiAny"/>.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <param name="comparisonContext">Context under which to compare the source and target.</param>
        public override void Compare(
            IOpenApiAny source,
            IOpenApiAny target,
            ComparisonContext comparisonContext)
        {
            if (source == null && target == null)
            {
                return;
            }

            if (source == null || target == null)
            {
                comparisonContext.AddOpenApiDifference(
                    new OpenApiDifference
                    {
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        SourceValue = source,
                        TargetValue = target,
                        OpenApiComparedElementType = typeof(IOpenApiAny),
                        Pointer = comparisonContext.PathString
                    });

                return;
            }

            var sourceStringWriter = new StringWriter();
            var sourceWriter = new OpenApiJsonWriter(sourceStringWriter);

            source.Write(sourceWriter, OpenApiSpecVersion.OpenApi3_0);
            var sourceValue = sourceStringWriter.GetStringBuilder().ToString();

            var targetStringWriter = new StringWriter();
            var targetWriter = new OpenApiJsonWriter(targetStringWriter);

            target.Write(targetWriter, OpenApiSpecVersion.OpenApi3_0);
            var targetValue = targetStringWriter.GetStringBuilder().ToString();

            if (string.IsNullOrWhiteSpace(sourceValue) && string.IsNullOrWhiteSpace(targetValue))
            {
                return;
            }

            if (string.Compare(sourceValue, targetValue, StringComparison.CurrentCultureIgnoreCase) != 0)
            {
                comparisonContext.AddOpenApiDifference(new OpenApiDifference
                {
                    OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                    OpenApiComparedElementType = typeof(IOpenApiAny),
                    SourceValue = source,
                    TargetValue = target,
                    Pointer = comparisonContext.PathString
                });
            }
        }
    }
}