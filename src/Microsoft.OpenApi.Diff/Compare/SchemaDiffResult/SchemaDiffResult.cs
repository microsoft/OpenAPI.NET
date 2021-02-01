using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Diff.BusinessObjects;
using Microsoft.OpenApi.Diff.Enums;
using Microsoft.OpenApi.Diff.Extensions;
using Microsoft.OpenApi.Diff.Utils;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Diff.Compare.SchemaDiffResult
{
    public class SchemaDiffResult
    {
        public ChangedSchemaBO ChangedSchema { get; set; }
        public OpenApiDiff OpenApiDiff { get; set; }

        public SchemaDiffResult(OpenApiDiff openApiDiff)
        {
            OpenApiDiff = openApiDiff;
            ChangedSchema = new ChangedSchemaBO();
        }

        public SchemaDiffResult(string type, OpenApiDiff openApiDiff) : this(openApiDiff)
        {
            ChangedSchema.Type = type;
        }

        public virtual ChangedSchemaBO Diff<T>(
          HashSet<string> refSet,
          OpenApiComponents leftComponents,
          OpenApiComponents rightComponents,
          T left,
          T right,
          DiffContextBO context)
            where T : OpenApiSchema
        {
            var leftEnumStrings = left.Enum.Select(x => ((IOpenApiPrimitive)x)?.GetValueString()).ToList();
            var rightEnumStrings = right.Enum.Select(x => ((IOpenApiPrimitive)x)?.GetValueString()).ToList();
            var leftDefault= (IOpenApiPrimitive)left.Default;
            var rightDefault = (IOpenApiPrimitive)right.Default;
            
            var changedEnum =
                ListDiff.Diff(new ChangedEnumBO(leftEnumStrings, rightEnumStrings, context));

            ChangedSchema.Context = context;
            ChangedSchema.OldSchema = left;
            ChangedSchema.NewSchema = right;
            ChangedSchema.IsChangeDeprecated = !left.Deprecated && right.Deprecated;
            ChangedSchema.IsChangeTitle = left.Title != right.Title;
            ChangedSchema.Required = ListDiff.Diff(new ChangedRequiredBO(left.Required.ToList(), right.Required.ToList(), context));
            ChangedSchema.IsChangeDefault = leftDefault?.GetValueString() != rightDefault?.GetValueString();
            ChangedSchema.Enumeration = changedEnum;
            ChangedSchema.IsChangeFormat = left.Format != right.Format;
            ChangedSchema.ReadOnly = new ChangedReadOnlyBO(left.ReadOnly, right.ReadOnly, context);
            ChangedSchema.WriteOnly = new ChangedWriteOnlyBO(left.WriteOnly, right.WriteOnly, context);
            ChangedSchema.MaxLength = new ChangedMaxLengthBO(left.MaxLength, right.MaxLength, context);

            var extendedDiff = OpenApiDiff.ExtensionsDiff.Diff(left.Extensions, right.Extensions, context);
            if (extendedDiff != null)
                ChangedSchema.Extensions = extendedDiff;
            var metaDataDiff = OpenApiDiff.MetadataDiff.Diff(left.Description, right.Description, context);
            if (metaDataDiff != null)
                ChangedSchema.Description = metaDataDiff;

            var leftProperties = left.Properties;
            var rightProperties = right.Properties;
            var propertyDiff = MapKeyDiff<string, OpenApiSchema>.Diff(leftProperties, rightProperties);

            foreach (var s in propertyDiff.SharedKey)
            {
                var diff = OpenApiDiff
                    .SchemaDiff
                    .Diff(refSet, leftProperties[s], rightProperties[s], Required(context, s, right.Required));

                if (diff != null)
                    ChangedSchema.ChangedProperties.Add(s, diff);
            }

            CompareAdditionalProperties(refSet, left, right, context);

            var allIncreasedProperties = FilterProperties(TypeEnum.Added, propertyDiff.Increased, context);
            foreach (var (key, value) in allIncreasedProperties)
            {
                ChangedSchema.IncreasedProperties.Add(key, value);
            }
            var allMissingProperties = FilterProperties(TypeEnum.Removed, propertyDiff.Missing, context);
            foreach (var (key, value) in allMissingProperties)
            {
                ChangedSchema.MissingProperties.Add(key, value);
            }

            return IsApplicable(context);
        }

        private static DiffContextBO Required(DiffContextBO context, string key, ICollection<string> required)
        {
            return context.CopyWithRequired(required != null && required.Contains(key));
        }

        private void CompareAdditionalProperties(HashSet<string> refSet, OpenApiSchema leftSchema, OpenApiSchema rightSchema, DiffContextBO context)
        {
            var left = leftSchema.AdditionalProperties;
            var right = rightSchema.AdditionalProperties;
            if (left != null || right != null)
            {
                var apChangedSchema = new ChangedSchemaBO
                {
                    Context = context,
                    OldSchema = left,
                    NewSchema = right
                };
                if (left != null && right != null)
                {
                    var addPropChangedSchemaOp =
                        OpenApiDiff
                            .SchemaDiff
                            .Diff(refSet, left, right, context.CopyWithRequired(false));
                    apChangedSchema = addPropChangedSchemaOp ?? apChangedSchema;
                }
                var changed = ChangedUtils.IsChanged(apChangedSchema);
                if (changed != null)
                    ChangedSchema.AddProp = changed;
            }
        }
        private Dictionary<string, OpenApiSchema> FilterProperties(TypeEnum type, Dictionary<string, OpenApiSchema> properties, DiffContextBO context)
        {
            var result = new Dictionary<string, OpenApiSchema>();

            foreach (var (key, value) in properties)
            {
                if (IsPropertyApplicable(value, context)
                    && OpenApiDiff
                        .ExtensionsDiff.IsParentApplicable(type,
                            value,
                            value?.Extensions ?? new Dictionary<string, IOpenApiExtension>(),
                            context))
                {
                    result.Add(key, value);
                }
                else
                {
                    // Child property is not applicable, so required cannot be applied
                    ChangedSchema.Required.Increased.Remove(key);
                }
            }


            return result;
        }

        private static bool IsPropertyApplicable(OpenApiSchema schema, DiffContextBO context)
        {
            return !(context.IsResponse && schema.WriteOnly) && !(context.IsRequest && schema.ReadOnly);
        }

        protected ChangedSchemaBO IsApplicable(DiffContextBO context)
        {
            if (ChangedSchema.ReadOnly.IsUnchanged()
                && ChangedSchema.WriteOnly.IsUnchanged()
                && !IsPropertyApplicable(ChangedSchema.NewSchema, context))
            {
                return null;
            }
            return ChangedUtils.IsChanged(ChangedSchema);
        }
    }
}
