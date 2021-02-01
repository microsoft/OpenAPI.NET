using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Diff.Enums;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Diff.BusinessObjects
{
    public class ChangedHeaderBO : ComposedChangedBO
    {
        protected override ChangedElementTypeEnum GetElementType() => ChangedElementTypeEnum.Header;

        public OpenApiHeader OldHeader { get; }
        public OpenApiHeader NewHeader { get; }
        private readonly DiffContextBO _context;

        public bool Required { get; set; }
        public bool Deprecated { get; set; }
        public bool Style { get; set; }
        public bool Explode { get; set; }
        public ChangedMetadataBO Description { get; set; }
        public ChangedSchemaBO Schema { get; set; }
        public ChangedContentBO Content { get; set; }
        public ChangedExtensionsBO Extensions { get; set; }

        public ChangedHeaderBO(OpenApiHeader oldHeader, OpenApiHeader newHeader, DiffContextBO context)
        {
            OldHeader = oldHeader;
            NewHeader = newHeader;
            _context = context;
        }

        public override List<(string Identifier, ChangedBO Change)> GetChangedElements()
        {
            return new List<(string Identifier, ChangedBO Change)>
                {
                    ("Description", Description),
                    ("Schema", Schema),
                    ("Content", Content),
                    (null, Extensions)
                }
                .Where(x => x.Change != null).ToList();
        }

        public override DiffResultBO IsCoreChanged()
        {
            if (!Required && !Deprecated && !Style && !Explode)
            {
                return new DiffResultBO(DiffResultEnum.NoChanges);
            }
            if (!Required && !Style && !Explode)
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

            if (Required)
                returnList.Add(new ChangedInfoBO(elementType, changeType, "Required", OldHeader?.Required.ToString(), NewHeader?.Required.ToString()));

            if (Deprecated)
                returnList.Add(new ChangedInfoBO(elementType, changeType, "Deprecation", OldHeader?.Deprecated.ToString(), NewHeader?.Deprecated.ToString()));

            if (Style)
                returnList.Add(new ChangedInfoBO(elementType, changeType, "Style", OldHeader?.Style.ToString(), NewHeader?.Style.ToString()));

            if (Explode)
                returnList.Add(new ChangedInfoBO(elementType, changeType, "Explode", OldHeader?.Explode.ToString(), NewHeader?.Explode.ToString()));

            return returnList;
        }
    }
}
