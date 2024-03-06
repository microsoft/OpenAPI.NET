using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Diff.BusinessObjects;
using Microsoft.OpenApi.Diff.Compare.SchemaDiffResult;
using Microsoft.OpenApi.Diff.Enums;
using Microsoft.OpenApi.Diff.Extensions;
using Microsoft.OpenApi.Diff.Utils;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Diff.Compare
{
    public class SchemaDiff : ReferenceDiffCache<OpenApiSchema, ChangedSchemaBO>
    {
        private static readonly RefPointer<OpenApiSchema> RefPointer = new RefPointer<OpenApiSchema>(RefTypeEnum.Schemas);

        private readonly OpenApiComponents _leftComponents;
        private readonly OpenApiComponents _rightComponents;
        private readonly OpenApiDiff _openApiDiff;

        public SchemaDiff(OpenApiDiff openApiDiff)
        {
            _openApiDiff = openApiDiff;
            _leftComponents = openApiDiff.OldSpecOpenApi?.Components;
            _rightComponents = openApiDiff.NewSpecOpenApi?.Components;
        }

        public static SchemaDiffResult.SchemaDiffResult GetSchemaDiffResult(OpenApiDiff openApiDiff)
        {
            return GetSchemaDiffResult(null, openApiDiff);
        }

        public static SchemaDiffResult.SchemaDiffResult GetSchemaDiffResult(OpenApiSchema schema, OpenApiDiff openApiDiff)
        {
            switch (schema.GetSchemaType())
            {
                case SchemaTypeEnum.Schema:
                    return new SchemaDiffResult.SchemaDiffResult(openApiDiff);
                case SchemaTypeEnum.ArraySchema:
                    return new ArraySchemaDiffResult(openApiDiff);
                case SchemaTypeEnum.ComposedSchema:
                    return new ComposedSchemaDiffResult(openApiDiff);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected static OpenApiSchema ResolveComposedSchema(OpenApiComponents components, OpenApiSchema schema)
        {
            if (schema != null && schema.GetSchemaType() == SchemaTypeEnum.ComposedSchema)
            {
                var allOfSchemaList = schema.AllOf;
                if (!allOfSchemaList.IsNullOrEmpty())
                {
                    var refName = "allOfCombined-";
                    allOfSchemaList
                        .ToList()
                        .ForEach(x => refName += x.Reference?.ReferenceV3);
                    if (components.Schemas.ContainsKey(refName))
                        return components.Schemas[refName];
                    components.Schemas.Add(refName, new OpenApiSchema());

                    var allOfCombinedSchema = new OpenApiSchema();
                    allOfCombinedSchema = AddSchema(allOfCombinedSchema, schema);
                    foreach (var t in allOfSchemaList)
                    {
                        var allOfSchema = t;
                        allOfSchema =
                            RefPointer.ResolveRef(components, allOfSchema, allOfSchema.Reference?.ReferenceV3);
                        allOfSchema = ResolveComposedSchema(components, allOfSchema);
                        allOfCombinedSchema = AddSchema(allOfCombinedSchema, allOfSchema);
                    }
                    return allOfCombinedSchema;
                }
            }
            return schema;
        }

        protected static OpenApiSchema AddSchema(OpenApiSchema schema, OpenApiSchema fromSchema)
        {
            if (fromSchema.Properties != null)
            {
                if (schema.Properties == null)
                {
                    schema.Properties = new Dictionary<string, OpenApiSchema>();
                }

                foreach (var property in fromSchema.Properties)
                {
                    schema.Properties.Add(property);
                }
            }

            if (fromSchema.Required != null)
            {
                if (schema.Required == null)
                {
                    schema.Required = fromSchema.Required;
                }
                else
                {
                    foreach (var required in fromSchema.Required)
                    {
                        schema.Required.Add(required);
                    }
                }
            }

            schema.ReadOnly = fromSchema.ReadOnly;
            schema.WriteOnly = fromSchema.WriteOnly;
            schema.Deprecated = fromSchema.Deprecated;
            schema.Nullable = fromSchema.Nullable;

            if (fromSchema.ExclusiveMaximum != null)
            {
                schema.ExclusiveMaximum = fromSchema.ExclusiveMaximum;
            }
            if (fromSchema.ExclusiveMinimum != null)
            {
                schema.ExclusiveMinimum = fromSchema.ExclusiveMinimum;
            }
            if (fromSchema.UniqueItems != null)
            {
                schema.UniqueItems = fromSchema.UniqueItems;
            }
            if (fromSchema.Description != null)
            {
                schema.Description = fromSchema.Description;
            }
            if (fromSchema.Format != null)
            {
                schema.Format = fromSchema.Format;
            }
            if (fromSchema.Type != null)
            {
                schema.Type = fromSchema.Type;
            }
            if (fromSchema.Enum != null)
            {
                if (schema.Enum == null)
                {
                    schema.Enum = new List<IOpenApiAny>();
                }
                //noinspection unchecked
                foreach (var element in fromSchema.Enum)
                {
                    schema.Enum.Add(element);
                }
            }
            if (fromSchema.Extensions != null)
            {
                if (schema.Extensions == null)
                {
                    schema.Extensions = new Dictionary<string, IOpenApiExtension>();
                }
                foreach (var element in fromSchema.Extensions)
                {
                    schema.Extensions.Add(element);
                }
            }
            if (fromSchema.Discriminator != null)
            {
                if (schema.Discriminator == null)
                {
                    schema.Discriminator = new OpenApiDiscriminator();
                }
                var discriminator = schema.Discriminator;
                var fromDiscriminator = fromSchema.Discriminator;

                if (fromDiscriminator.PropertyName != null)
                {
                    discriminator.PropertyName = fromDiscriminator.PropertyName;
                }
                if (fromDiscriminator.Mapping != null)
                {
                    if (discriminator.Mapping == null)
                    {
                        discriminator.Mapping = new Dictionary<string, string>();
                    }
                    foreach (var element in fromDiscriminator.Mapping)
                    {
                        discriminator.Mapping.Add(element);
                    }
                }
            }
            if (fromSchema.Title != null)
            {
                schema.Title = fromSchema.Title;
            }
            if (fromSchema.AdditionalProperties != null)
            {
                schema.AdditionalProperties = fromSchema.AdditionalProperties;
            }
            if (fromSchema.Default != null)
            {
                schema.Default = fromSchema.Default;
            }
            if (fromSchema.Example != null)
            {
                schema.Example = fromSchema.Example;
            }
            if (fromSchema.ExternalDocs != null)
            {
                if (schema.ExternalDocs == null)
                {
                    schema.ExternalDocs = new OpenApiExternalDocs();
                }
                var externalDocs = schema.ExternalDocs;
                var fromExternalDocs = fromSchema.ExternalDocs;
                if (fromExternalDocs.Description != null)
                {
                    externalDocs.Description = fromExternalDocs.Description;
                }
                if (fromExternalDocs.Extensions != null)
                {
                    if (externalDocs.Extensions == null)
                    {
                        externalDocs.Extensions = new Dictionary<string, IOpenApiExtension>();
                    }

                    foreach (var element in fromSchema.Extensions)
                    {
                        schema.Extensions.Add(element);
                    }
                }
                if (fromExternalDocs.Url != null)
                {
                    externalDocs.Url = fromExternalDocs.Url;
                }
            }
            if (fromSchema.Maximum != null)
            {
                schema.Maximum = fromSchema.Maximum;
            }
            if (fromSchema.Minimum != null)
            {
                schema.Minimum = fromSchema.Minimum;
            }
            if (fromSchema.MaxItems != null)
            {
                schema.MaxItems = fromSchema.MaxItems;
            }
            if (fromSchema.MinItems != null)
            {
                schema.MinItems = fromSchema.MinItems;
            }
            if (fromSchema.MaxProperties != null)
            {
                schema.MaxProperties = fromSchema.MaxProperties;
            }
            if (fromSchema.MinProperties != null)
            {
                schema.MinProperties = fromSchema.MinProperties;
            }
            if (fromSchema.MaxLength != null)
            {
                schema.MaxLength = fromSchema.MaxLength;
            }
            if (fromSchema.MinLength != null)
            {
                schema.MinLength = fromSchema.MinLength;
            }
            if (fromSchema.MultipleOf != null)
            {
                schema.MultipleOf = fromSchema.MultipleOf;
            }
            if (fromSchema.Not != null)
            {
                if (schema.Not == null)
                {
                    schema.Not = AddSchema(new OpenApiSchema(), fromSchema.Not);
                }
                else
                {
                    AddSchema(schema.Not, fromSchema.Not);
                }
            }
            if (fromSchema.Pattern != null)
            {
                schema.Pattern = fromSchema.Pattern;
            }
            if (fromSchema.Xml != null)
            {
                if (schema.Xml == null)
                {
                    schema.Xml = new OpenApiXml();
                }
                var xml = schema.Xml;
                var fromXml = fromSchema.Xml;

                xml.Attribute = fromXml.Attribute;

                if (fromXml.Name != null)
                {
                    xml.Name = fromXml.Name;
                }
                if (fromXml.Namespace != null)
                {
                    xml.Namespace = fromXml.Namespace;
                }
                if (fromXml.Extensions != null)
                {
                    if (xml.Extensions == null)
                    {
                        xml.Extensions = new Dictionary<string, IOpenApiExtension>();
                    }
                    foreach (var element in fromXml.Extensions)
                    {
                        xml.Extensions.Add(element);
                    }
                }
                if (fromXml.Prefix != null)
                {
                    xml.Prefix = fromXml.Prefix;
                }

                xml.Wrapped = fromXml.Wrapped;
            }
            return schema;
        }

        private static string GetSchemaRef(OpenApiSchema schema)
        {
            return schema?.Reference?.ReferenceV3;
        }

        public ChangedSchemaBO Diff(HashSet<string> refSet, OpenApiSchema left, OpenApiSchema right, DiffContextBO context)
        {
            if (left == null && right == null)
            {
                return null;
            }
            return CachedDiff(refSet, left, right, GetSchemaRef(left), GetSchemaRef(right), context);
        }

        public ChangedSchemaBO GetTypeChangedSchema(
            OpenApiSchema left, OpenApiSchema right, DiffContextBO context)
        {
            var schemaDiffResult = GetSchemaDiffResult(_openApiDiff);
            schemaDiffResult.ChangedSchema.OldSchema = left;
            schemaDiffResult.ChangedSchema.NewSchema = right;
            schemaDiffResult.ChangedSchema.IsChangedType = true;
            schemaDiffResult.ChangedSchema.Context = context;

            return schemaDiffResult.ChangedSchema;
        }

        protected override ChangedSchemaBO ComputeDiff(
            HashSet<string> refSet, OpenApiSchema left, OpenApiSchema right, DiffContextBO context)
        {
            left = RefPointer.ResolveRef(_leftComponents, left, GetSchemaRef(left));
            right = RefPointer.ResolveRef(_rightComponents, right, GetSchemaRef(right));

            left = ResolveComposedSchema(_leftComponents, left);
            right = ResolveComposedSchema(_rightComponents, right);

            // If type of schemas are different, just set old & new schema, set changedType to true in
            // SchemaDiffResult and
            // return the object
            if ((left == null || right == null)
                || left.Type != right.Type
                || left.Format != right.Format)
            {
                return GetTypeChangedSchema(left, right, context);
            }

            // If schema type is same then get specific SchemaDiffResult and compare the properties
            var result = GetSchemaDiffResult(right, _openApiDiff);
            return result.Diff(refSet, _leftComponents, _rightComponents, left, right, context);
        }
    }
}
