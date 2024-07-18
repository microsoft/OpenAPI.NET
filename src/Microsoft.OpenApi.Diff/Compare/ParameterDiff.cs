using System.Collections.Generic;
using Microsoft.OpenApi.Diff.BusinessObjects;
using Microsoft.OpenApi.Diff.Enums;
using Microsoft.OpenApi.Diff.Utils;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Diff.Compare
{
    public class ParameterDiff : ReferenceDiffCache<OpenApiParameter, ChangedParameterBO>
    {
        private static readonly RefPointer<OpenApiParameter> RefPointer = new RefPointer<OpenApiParameter>(RefTypeEnum.Parameters);
        private readonly OpenApiComponents _leftComponents;
        private readonly OpenApiComponents _rightComponents;
        private readonly OpenApiDiff _openApiDiff;

        public ParameterDiff(OpenApiDiff openApiDiff)
        {
            _openApiDiff = openApiDiff;
            _leftComponents = openApiDiff.OldSpecOpenApi?.Components;
            _rightComponents = openApiDiff.NewSpecOpenApi?.Components;
        }

        public ChangedParameterBO Diff(OpenApiParameter left, OpenApiParameter right, DiffContextBO context)
        {
            return CachedDiff(new HashSet<string>(), left, right, left.Reference?.ReferenceV3, right.Reference?.ReferenceV3, context);
        }

        protected override ChangedParameterBO ComputeDiff(HashSet<string> refSet, OpenApiParameter left, OpenApiParameter right, DiffContextBO context)
        {
            left = RefPointer.ResolveRef(_leftComponents, left, left.Reference?.ReferenceV3);
            right = RefPointer.ResolveRef(_rightComponents, right, right.Reference?.ReferenceV3);

            var changedParameter =
                new ChangedParameterBO(right.Name, right.In, left, right, context)
                {
                    IsChangeRequired = GetBooleanDiff(left.Required, right.Required),
                    IsDeprecated = !left.Deprecated && right.Deprecated,
                    ChangeAllowEmptyValue = GetBooleanDiff(left.AllowEmptyValue, right.AllowEmptyValue),
                    ChangeStyle = left.Style != right.Style,
                    ChangeExplode = GetBooleanDiff(left.Explode, right.Explode),
                    Schema = _openApiDiff
                        .SchemaDiff
                        .Diff(refSet, left.Schema, right.Schema, context.CopyWithRequired(true)),
                    Description = _openApiDiff
                        .MetadataDiff
                        .Diff(left.Description, right.Description, context),
                    Content = _openApiDiff
                        .ContentDiff
                        .Diff(left.Content, right.Content, context),
                    Extensions = _openApiDiff
                        .ExtensionsDiff
                        .Diff(left.Extensions, right.Extensions, context)
                };

            return ChangedUtils.IsChanged(changedParameter);
        }

        private static bool GetBooleanDiff(bool? left, bool? right)
        {
            var leftRequired = left ?? false;
            var rightRequired = right ?? false;
            return leftRequired != rightRequired;
        }
    }
}
