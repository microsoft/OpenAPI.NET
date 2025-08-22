// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;

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
                        // The reference was not followed to a valid schema somewhere in the document
                        context.Enter(GetSegment());
                        context.CreateWarning(ruleName, string.Format(SRResource.Validation_SchemaReferenceDoesNotExist, reference.Reference.ReferenceV3));
                        context.Exit();
                    }
                }
                catch (InvalidOperationException ex)
                {
                    context.Enter(GetSegment());
                    context.CreateWarning(ruleName, ex.Message);
                    context.Exit();
                }

                string GetSegment()
                {
                    // Trim off the leading "#/" as the context is already at the root of the document
                    return
#if NET8_0_OR_GREATER
                        $"{PathString[2..]}/$ref";
#else
                        PathString.Substring(2) + "/$ref";
#endif
                }
            }
        }
    }
}
