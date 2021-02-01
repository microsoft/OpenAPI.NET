using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Diff.Enums;
using Microsoft.OpenApi.Diff.Extensions;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Diff.BusinessObjects
{
    public class ChangedHeadersBO : ComposedChangedBO
    {
        protected override ChangedElementTypeEnum GetElementType() => ChangedElementTypeEnum.Header;

        private readonly IDictionary<string, OpenApiHeader> _oldHeaders;
        private readonly IDictionary<string, OpenApiHeader> _newHeaders;
        private readonly DiffContextBO _context;

        public Dictionary<string, OpenApiHeader> Increased { get; set; }
        public Dictionary<string, OpenApiHeader> Missing { get; set; }
        public Dictionary<string, ChangedHeaderBO> Changed { get; set; }

        public ChangedHeadersBO(IDictionary<string, OpenApiHeader> oldHeaders, IDictionary<string, OpenApiHeader> newHeaders, DiffContextBO context)
        {
            _oldHeaders = oldHeaders;
            _newHeaders = newHeaders;
            _context = context;
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
            if (Missing.IsNullOrEmpty())
            {
                return new DiffResultBO(DiffResultEnum.Compatible);
            }
            return new DiffResultBO(DiffResultEnum.Incompatible);
        }

        protected override List<ChangedInfoBO> GetCoreChanges() =>
            GetCoreChangeInfosOfComposed(Increased.Keys.ToList(), Missing.Keys.ToList(), x => x);
    }
}
