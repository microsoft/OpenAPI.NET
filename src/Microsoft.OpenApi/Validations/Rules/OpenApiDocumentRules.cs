// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Text.Json.Nodes;
using Microsoft.OpenApi.Reader;

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
                    context.Enter("info");
                    if (item.Info == null)
                    {
                        context.CreateError(nameof(OpenApiDocumentFieldIsMissing),
                            string.Format(SRResource.Validation_FieldIsRequired, "info", "document"));
                    }
                    context.Exit();
                });

        /// <summary>
        /// All references in the OpenAPI document must be valid.
        /// </summary>
        public static ValidationRule<OpenApiDocument> OpenApiDocumentReferencesAreValid =>
            new(nameof(OpenApiDocumentReferencesAreValid),
                static (context, item) =>
                {
                    const string RuleName = nameof(OpenApiDocumentReferencesAreValid);

                    JsonNode document;

                    using (var textWriter = new System.IO.StringWriter())
                    {
                        var writer = new OpenApiJsonWriter(textWriter);

                        item.SerializeAsV31(writer);

                        var json = textWriter.ToString();

                        document = JsonNode.Parse(json)!;
                    }

                    var visitor = new OpenApiSchemaReferenceVisitor(RuleName, context, document);
                    var walker = new OpenApiWalker(visitor);

                    walker.Walk(item);
                });

        private sealed class OpenApiSchemaReferenceVisitor(
            string ruleName,
            IValidationContext context,
            JsonNode document) : OpenApiVisitorBase
        {
            public override void Visit(IOpenApiReferenceHolder referenceHolder)
            {
                if (referenceHolder is OpenApiSchemaReference { Reference.IsLocal: true } reference)
                {
                    ValidateSchemaReference(reference);
                }
            }

            public override void Visit(IOpenApiSchema schema)
            {
                if (schema is OpenApiSchemaReference { Reference.IsLocal: true } reference)
                {
                    ValidateSchemaReference(reference);
                }
            }

            private void ValidateSchemaReference(OpenApiSchemaReference reference)
            {
                // Trim off the leading "#/" as the context is already at the root of the document
                var segment =
#if NET8_0_OR_GREATER
                    $"{PathString[2..]}/$ref";
#else
                    PathString.Substring(2) + "/$ref";
#endif

                try
                {
                    if (reference.RecursiveTarget is not null)
                    {
                        // The reference was followed to a valid schema somewhere in the document
                        return;
                    }
                }
                catch (InvalidOperationException ex)
                {
                    context.Enter(segment);
                    context.CreateWarning(ruleName, ex.Message);
                    context.Exit();

                    return;
                }

                var id = reference.Reference.ReferenceV3;

                if (id is { Length: > 0 } && !IsValidSchemaReference(id, document))
                {
                    var isValid = false;

                    // Sometimes ReferenceV3 is not a JSON valid JSON pointer, but the $ref
                    // associated with it still points to a valid location in the document.
                    // In these cases, we need to find it manually to verify that fact before
                    // generating a warning that the schema reference is indeed invalid.
                    // TODO Why is this, and can it be avoided?
                    var parent = Find(PathString, document);

                    if (parent?["$ref"] is { } @ref &&
                        @ref.GetValueKind() is System.Text.Json.JsonValueKind.String &&
                        @ref.GetValue<string>() is { Length: > 0 } refId)
                    {
                        id = refId;
                        isValid = IsValidSchemaReference(id, document);
                    }

                    if (!isValid)
                    {
                        context.Enter(segment);
                        context.CreateWarning(ruleName, string.Format(SRResource.Validation_SchemaReferenceDoesNotExist, id));
                        context.Exit();
                    }
                }

                static bool IsValidSchemaReference(string id, JsonNode baseNode)
                    => Find(id, baseNode) is not null;

                static JsonNode? Find(string id, JsonNode baseNode)
                {
                    var pointer = new JsonPointer(id.Replace("#/", "/"));
                    return pointer.Find(baseNode);
                }
            }
        }
    }
}
