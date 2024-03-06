using System.Collections.Generic;
using Microsoft.OpenApi.Diff.BusinessObjects;
using Microsoft.OpenApi.Diff.Enums;
using Microsoft.OpenApi.Diff.Extensions;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Diff.Compare.SchemaDiffResult
{
    public class ArraySchemaDiffResult : SchemaDiffResult
    {
        public ArraySchemaDiffResult(OpenApiDiff openApiDiff) : base("array", openApiDiff)
        {
        }

        public override ChangedSchemaBO Diff<T>(HashSet<string> refSet, OpenApiComponents leftComponents, OpenApiComponents rightComponents, T left,
            T right, DiffContextBO context)
        {
            if (left.GetSchemaType() != SchemaTypeEnum.ArraySchema
                || right.GetSchemaType() != SchemaTypeEnum.ArraySchema)
                return null;

            base.Diff(refSet, leftComponents, rightComponents, left, right, context);

            var diff = OpenApiDiff
                .SchemaDiff
                .Diff(
                    refSet,
                    left.Items,
                    right.Items,
                    context.CopyWithRequired(true));
            if (diff != null)
                ChangedSchema.Items = diff;

            return IsApplicable(context);
        }
    }
}
