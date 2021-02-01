using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Diff.Enums;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Diff.BusinessObjects
{
    public class ChangedMediaTypeBO : ComposedChangedBO
    {
        protected override ChangedElementTypeEnum GetElementType() => ChangedElementTypeEnum.MediaType;

        private readonly OpenApiSchema _oldSchema;
        private readonly OpenApiSchema _newSchema;
        private readonly DiffContextBO _context;

        public ChangedSchemaBO Schema { get; set; }

        public ChangedMediaTypeBO(OpenApiSchema oldSchema, OpenApiSchema newSchema, DiffContextBO context) 
        {
            _oldSchema = oldSchema;
            _newSchema = newSchema;
            _context = context;
        }

        public override List<(string Identifier, ChangedBO Change)> GetChangedElements()
        {
            return new List<(string Identifier, ChangedBO Change)>
                {
                    (null, Schema),
                }
                .Where(x => x.Change != null).ToList();
        }

        public override DiffResultBO IsCoreChanged()
        {
            return new DiffResultBO(DiffResultEnum.NoChanges);
        }

        protected override List<ChangedInfoBO> GetCoreChanges()
        {
            return new List<ChangedInfoBO>();
        }
    }
}
