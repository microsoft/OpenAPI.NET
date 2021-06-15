using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Diff.Enums;
using Microsoft.OpenApi.Diff.Extensions;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Diff.BusinessObjects
{
    public class ChangedPathsBO : ComposedChangedBO
    {
        protected override ChangedElementTypeEnum GetElementType() => ChangedElementTypeEnum.Path;
        
        private readonly Dictionary<string, OpenApiPathItem> _oldPathMap;
        private readonly Dictionary<string, OpenApiPathItem> _newPathMap;

        public Dictionary<string, OpenApiPathItem> Increased { get; set; }
        public Dictionary<string, OpenApiPathItem> Missing { get; set; }
        public Dictionary<string, ChangedPathBO> Changed { get; set; }

        public ChangedPathsBO(Dictionary<string, OpenApiPathItem> oldPathMap, Dictionary<string, OpenApiPathItem> newPathMap) 
        {
            _oldPathMap = oldPathMap;
            _newPathMap = newPathMap;
            Increased = new Dictionary<string, OpenApiPathItem>();
            Missing = new Dictionary<string, OpenApiPathItem>();
            Changed = new Dictionary<string, ChangedPathBO>();
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
            GetCoreChangeInfosOfComposed(Increased.ToList(), Missing.ToList(), x => x.Key);
    }
}
