// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Services
{
    /// <summary>
    /// Defines behavior for comparing properties of <see cref="OpenApiSchema"/>.
    /// </summary>
    public class OpenApiSchemaComparer : OpenApiComparerBase<OpenApiSchema>
    {
        /// <summary>
        /// Executes comparision against source and target <see cref="OpenApiSchema"/>.
        /// </summary>
        /// <param name="sourceSchema">The source.</param>
        /// <param name="targetSchema">The target.</param>
        /// <param name="comparisonContext">Context under which to compare the source and target.</param>
        public override void Compare(
            OpenApiSchema sourceSchema,
            OpenApiSchema targetSchema,
            ComparisonContext comparisonContext)
        {
            if (sourceSchema == null && targetSchema == null)
            {
                return;
            }

            if (sourceSchema == null || targetSchema == null)
            {
                comparisonContext.AddOpenApiDifference(
                    new OpenApiDifference
                    {
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        SourceValue = sourceSchema,
                        TargetValue = targetSchema,
                        OpenApiComparedElementType = typeof(OpenApiSchema),
                        Pointer = comparisonContext.PathString
                    });

                return;
            }

            if (comparisonContext.SourceSchemaLoop.Contains(sourceSchema)
                || comparisonContext.SourceSchemaLoop.Contains(targetSchema))
            {
                return; // Loop detected, this schema has already been walked.
            }

            comparisonContext.SourceSchemaLoop.Push(sourceSchema);
            comparisonContext.TargetSchemaLoop.Push(targetSchema);

            if (sourceSchema.Reference != null
                && targetSchema.Reference != null
                && sourceSchema.Reference.Id != targetSchema.Reference.Id)
            {
                WalkAndAddOpenApiDifference(
                    comparisonContext,
                    OpenApiConstants.DollarRef,
                    new OpenApiDifference
                    {
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        SourceValue = sourceSchema.Reference?.Id,
                        TargetValue = targetSchema.Reference?.Id,
                        OpenApiComparedElementType = typeof(string)
                    });

                return;
            }

            if (sourceSchema.Reference != null)
            {
                sourceSchema = (OpenApiSchema) comparisonContext.SourceDocument.ResolveReference(
                    sourceSchema.Reference);
            }

            if (targetSchema.Reference != null)
            {
                targetSchema = (OpenApiSchema) comparisonContext.TargetDocument.ResolveReference(
                    targetSchema.Reference);
            }

            WalkAndCompare(
                comparisonContext,
                OpenApiConstants.Title,
                () => Compare(sourceSchema.Title, targetSchema.Title, comparisonContext));

            WalkAndCompare(
                comparisonContext,
                OpenApiConstants.Maximum,
                () => Compare(sourceSchema.Maximum, targetSchema.Maximum, comparisonContext));

            WalkAndCompare(comparisonContext, OpenApiConstants.MultipleOf,
                () => Compare(sourceSchema.MultipleOf, targetSchema.MultipleOf, comparisonContext));

            WalkAndCompare(comparisonContext, OpenApiConstants.ExclusiveMaximum,
                () => Compare(sourceSchema.ExclusiveMaximum, targetSchema.ExclusiveMaximum, comparisonContext));

            WalkAndCompare(comparisonContext, OpenApiConstants.Minimum,
                () => Compare(sourceSchema.Minimum, targetSchema.Minimum, comparisonContext));

            WalkAndCompare(comparisonContext, OpenApiConstants.ExclusiveMinimum,
                () => Compare(sourceSchema.ExclusiveMinimum, targetSchema.ExclusiveMinimum, comparisonContext));

            WalkAndCompare(comparisonContext, OpenApiConstants.MaxLength,
                () => Compare(sourceSchema.MaxLength, targetSchema.MaxLength, comparisonContext));

            WalkAndCompare(comparisonContext, OpenApiConstants.MinLength,
                () => Compare(sourceSchema.MinLength, targetSchema.MinLength, comparisonContext));

            WalkAndCompare(comparisonContext, OpenApiConstants.MaxItems,
                () => Compare(sourceSchema.MaxItems, targetSchema.MaxItems, comparisonContext));

            WalkAndCompare(comparisonContext, OpenApiConstants.MinItems,
                () => Compare(sourceSchema.MinItems, targetSchema.MinItems, comparisonContext));

            WalkAndCompare(comparisonContext, OpenApiConstants.Format,
                () => Compare(sourceSchema.Format, targetSchema.Format, comparisonContext));

            if (sourceSchema.Type != targetSchema.Type)
            {
                WalkAndAddOpenApiDifference(
                    comparisonContext,
                    OpenApiConstants.Type,
                    new OpenApiDifference
                    {
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        SourceValue = sourceSchema.Type,
                        TargetValue = targetSchema.Type,
                        OpenApiComparedElementType = typeof(string)
                    });

                return;
            }

            if (sourceSchema.Items != null && targetSchema.Items != null)
            {
                WalkAndCompare(
                    comparisonContext,
                    OpenApiConstants.Items,
                    () => comparisonContext
                        .GetComparer<OpenApiSchema>()
                        .Compare(sourceSchema.Items, targetSchema.Items, comparisonContext));
            }

            if (sourceSchema.Reference != null
                && targetSchema.Reference != null
                && sourceSchema.Reference.Id != targetSchema.Reference.Id)
            {
                WalkAndAddOpenApiDifference(
                    comparisonContext,
                    OpenApiConstants.DollarRef,
                    new OpenApiDifference
                    {
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        SourceValue = sourceSchema.Reference?.Id,
                        TargetValue = targetSchema.Reference?.Id,
                        OpenApiComparedElementType = typeof(string)
                    });

                return;
            }

            if (sourceSchema.Reference != null)
            {
                sourceSchema = (OpenApiSchema) comparisonContext.SourceDocument.ResolveReference(
                    sourceSchema.Reference);
            }

            if (targetSchema.Reference != null)
            {
                targetSchema = (OpenApiSchema) comparisonContext.TargetDocument.ResolveReference(
                    targetSchema.Reference);
            }

            if (targetSchema.Properties != null)
            {
                IEnumerable<string> newPropertiesInTarget = sourceSchema.Properties == null
                    ? targetSchema.Properties.Keys
                    : targetSchema.Properties.Keys.Except(sourceSchema.Properties.Keys)
                        .ToList();

                WalkAndCompare(comparisonContext, OpenApiConstants.Properties, () =>
                {
                    foreach (var newPropertyInTarget in newPropertiesInTarget)
                    {
                        WalkAndAddOpenApiDifference(
                            comparisonContext,
                            newPropertyInTarget,
                            new OpenApiDifference
                            {
                                OpenApiDifferenceOperation = OpenApiDifferenceOperation.Add,
                                TargetValue = new KeyValuePair<string, OpenApiSchema>(newPropertyInTarget,
                                    targetSchema.Properties[newPropertyInTarget]),
                                OpenApiComparedElementType = typeof(KeyValuePair<string, OpenApiSchema>)
                            });
                    }
                });
            }

            if (sourceSchema.Properties != null)
            {
                WalkAndCompare(comparisonContext, OpenApiConstants.Properties, () =>
                {
                    foreach (var sourceSchemaProperty in sourceSchema.Properties)
                    {
                        if (targetSchema.Properties.ContainsKey(sourceSchemaProperty.Key))
                        {
                            WalkAndCompare(
                                comparisonContext,
                                sourceSchemaProperty.Key,
                                () => comparisonContext
                                    .GetComparer<OpenApiSchema>()
                                    .Compare(sourceSchemaProperty.Value,
                                        targetSchema.Properties[sourceSchemaProperty.Key], comparisonContext));
                        }
                        else
                        {
                            WalkAndAddOpenApiDifference(
                                comparisonContext,
                                sourceSchemaProperty.Key,
                                new OpenApiDifference
                                {
                                    OpenApiDifferenceOperation = OpenApiDifferenceOperation.Remove,
                                    SourceValue = sourceSchemaProperty,
                                    OpenApiComparedElementType = typeof(KeyValuePair<string, OpenApiSchema>)
                                });
                        }
                    }
                });
            }

            // To Do Compare schema.AllOf
            // To Do Compare schema.AnyOf

            comparisonContext.SourceSchemaLoop.Pop();
            comparisonContext.TargetSchemaLoop.Pop();
        }
    }
}