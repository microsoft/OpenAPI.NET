// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// The validation rules for <see cref="OpenApiDocument"/>.
    /// </summary>
    [OpenApiRule]
    public static class OpenApiDocumentRules
    {
        /// <summary>
        /// The Info field is required.
        /// </summary>
        public static ValidationRule<OpenApiDocument> OpenApiDocumentFieldIsMissing =>
            new(nameof(OpenApiDocumentFieldIsMissing),
                (context, item) =>
                {
                    // info
                    if (item.Info == null)
                    {
                        context.Enter("info");
                        context.CreateError(nameof(OpenApiDocumentFieldIsMissing),
                            string.Format(SRResource.Validation_FieldIsRequired, "info", "document"));
                        context.Exit();
                    }
                });

        /// <summary>
        /// All references in the OpenAPI document must be valid.
        /// </summary>
        public static ValidationRule<OpenApiDocument> OpenApiDocumentReferencesAreValid =>
            new(nameof(OpenApiDocumentReferencesAreValid),
                static (context, item) =>
                {
                    const string RuleName = nameof(OpenApiDocumentReferencesAreValid);

                    var visitor = new OpenApiSchemaReferenceVisitor(RuleName, context);
                    var walker = new OpenApiWalker(visitor);

                    walker.Walk(item);
                });

        private sealed class OpenApiSchemaReferenceVisitor(
            string ruleName,
            IValidationContext context) : OpenApiVisitorBase
        {
            public override void Visit(IOpenApiReferenceHolder referenceHolder)
            {
                if (referenceHolder is OpenApiSchemaReference reference)
                {
                    ValidateSchemaReference(reference);
                }
            }

            public override void Visit(IOpenApiSchema schema)
            {
                if (schema is OpenApiSchemaReference reference)
                {
                    ValidateSchemaReference(reference);
                }
            }

            private void ValidateSchemaReference(OpenApiSchemaReference reference)
            {
                if (!reference.Reference.IsLocal)
                {
                    return;
                }

                try
                {
                    if (reference.RecursiveTarget is null)
                    {
                        var segments = GetSegments().ToArray();
                        EnterSegments(segments);
                        // The reference was not followed to a valid schema somewhere in the document
                        context.CreateWarning(ruleName, string.Format(SRResource.Validation_SchemaReferenceDoesNotExist, reference.Reference.ReferenceV3));
                        ExitSegments(segments.Length);
                    }
                }
                catch (InvalidOperationException ex)
                {
                    var segments = GetSegments().ToArray();
                    EnterSegments(segments);
                    context.CreateWarning(ruleName, ex.Message);
                    ExitSegments(segments.Length);
                }

                void ExitSegments(int length)
                {
                    for (var i = 0; i < length; i++)
                    {
                        context.Exit();
                    }
                }

                void EnterSegments(string[] segments)
                {
                    foreach (var segment in segments)
                    {
                        context.Enter(segment);
                    }
                }

                IEnumerable<string> GetSegments()
                {
                    foreach (var segment in this.PathString.Substring(2).Split('/'))
                    {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP1_0_OR_GREATER
                        yield return segment.Replace("~1", "/", StringComparison.OrdinalIgnoreCase).Replace("~0", "~", StringComparison.OrdinalIgnoreCase);
#else
                        yield return segment.Replace("~1", "/").Replace("~0", "~");
#endif
                    }
                    yield return "$ref";
                    // Trim off the leading "#/" as the context is already at the root of the document
                }
            }
        }
    }
}
