using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Diff.Enums;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Diff.BusinessObjects
{
    public class ChangedParameterBO : ComposedChangedBO
    {
        protected override ChangedElementTypeEnum GetElementType() => ChangedElementTypeEnum.Parameter;
       
        private readonly DiffContextBO _context;
        public ParameterLocation? In { get; set; }
        public string Name { get; set; }
        public OpenApiParameter OldParameter { get;  }
        public OpenApiParameter NewParameter { get; }
        public bool IsChangeRequired { get; set; }
        public bool IsDeprecated { get; set; }
        public bool ChangeStyle { get; set; }
        public bool ChangeExplode { get; set; }
        public bool ChangeAllowEmptyValue { get; set; }
        public ChangedMetadataBO Description { get; set; }
        public ChangedSchemaBO Schema { get; set; }
        public ChangedContentBO Content { get; set; }
        public ChangedExtensionsBO Extensions { get; set; }

        public ChangedParameterBO(string name, ParameterLocation? @in, OpenApiParameter oldParameter, OpenApiParameter newParameter, DiffContextBO context)
        {
            _context = context;
            Name = name;
            In = @in;
            OldParameter = oldParameter;
            NewParameter = newParameter;
        }

        public override List<(string Identifier, ChangedBO Change)> GetChangedElements()
        {
            return new List<(string Identifier, ChangedBO Change)>
                {
                    (null, Description),
                    (null, Schema),
                    (null, Content),
                    (null, Extensions)
                }
                .Where(x => x.Change != null).ToList();
        }

        public override DiffResultBO IsCoreChanged()
        {
            if (!IsChangeRequired
                && !IsDeprecated
                && !ChangeAllowEmptyValue
                && !ChangeStyle
                && !ChangeExplode)
            {
                return new DiffResultBO(DiffResultEnum.NoChanges);
            }
            if ((!IsChangeRequired || OldParameter.Required)
                && (!ChangeAllowEmptyValue || NewParameter.AllowEmptyValue)
                && !ChangeStyle
                && !ChangeExplode)
            {
                return new DiffResultBO(DiffResultEnum.Compatible);
            }
            return new DiffResultBO(DiffResultEnum.Incompatible);
        }

        protected override List<ChangedInfoBO> GetCoreChanges()
        {
            var returnList = new List<ChangedInfoBO>();
            var elementType = GetElementType();
            const TypeEnum changeType = TypeEnum.Changed;

            if (IsChangeRequired)
                returnList.Add(new ChangedInfoBO(elementType, changeType, "Required", OldParameter?.Required.ToString(), NewParameter?.Required.ToString()));

            if (IsDeprecated)
                returnList.Add(new ChangedInfoBO(elementType, changeType, "Deprecation", OldParameter?.Deprecated.ToString(), NewParameter?.Deprecated.ToString()));

            if (ChangeStyle)
                returnList.Add(new ChangedInfoBO(elementType, changeType, "Style", OldParameter?.Style.ToString(), NewParameter?.Style.ToString()));

            if (ChangeExplode)
                returnList.Add(new ChangedInfoBO(elementType, changeType, "Explode", OldParameter?.Explode.ToString(), NewParameter?.Explode.ToString()));

            if (ChangeAllowEmptyValue)
                returnList.Add(new ChangedInfoBO(elementType, changeType, "AllowEmptyValue", OldParameter?.AllowEmptyValue.ToString(), NewParameter?.AllowEmptyValue.ToString()));

            return returnList;
        }
    }
}
