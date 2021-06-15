using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Diff.Enums;
using Microsoft.OpenApi.Diff.Extensions;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Diff.BusinessObjects
{
    public class ChangedContentBO : ComposedChangedBO
    {
        protected override ChangedElementTypeEnum GetElementType() => ChangedElementTypeEnum.Content;

        private readonly Dictionary<string, OpenApiMediaType> _oldContent;
        private readonly Dictionary<string, OpenApiMediaType> _newContent;
        private readonly DiffContextBO _context;
        
        public Dictionary<string, OpenApiMediaType> Increased { get; set; }
        public Dictionary<string, OpenApiMediaType> Missing { get; set; }
        public Dictionary<string, ChangedMediaTypeBO> Changed { get; set; }

        public ChangedContentBO(Dictionary<string, OpenApiMediaType> oldContent, Dictionary<string, OpenApiMediaType> newContent, DiffContextBO context) 
        {
            _oldContent = oldContent;
            _newContent = newContent;
            _context = context;
            Increased = new Dictionary<string, OpenApiMediaType>();
            Missing = new Dictionary<string, OpenApiMediaType>();
            Changed = new Dictionary<string, ChangedMediaTypeBO>();
        }

        public override List<(string Identifier, ChangedBO Change)> GetChangedElements()
        {
            return new List<(string Identifier, ChangedBO Change)>(
                    Changed.Select(x => (x.Key, (ChangedBO)x.Value))
                )
                .Where(x => x.Change != null).ToList();
        }

        public override DiffResultBO IsCoreChanged()
        {
            if (Increased.IsNullOrEmpty() && Missing.IsNullOrEmpty())
            {
                return new DiffResultBO(DiffResultEnum.NoChanges);
            }
            if (_context.IsRequest && Missing.IsNullOrEmpty() || _context.IsResponse && Increased.IsNullOrEmpty())
            {
                return new DiffResultBO(DiffResultEnum.Compatible);
            }
            return new DiffResultBO(DiffResultEnum.Incompatible);
        }

        protected override List<ChangedInfoBO> GetCoreChanges() =>
            GetCoreChangeInfosOfComposed(Increased.Keys.ToList(), Missing.Keys.ToList(), x => x);
    }
}
