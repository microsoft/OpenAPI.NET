using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Diff.Enums;
using Microsoft.OpenApi.Diff.Extensions;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Diff.BusinessObjects
{
    public class ChangedAPIResponseBO : ComposedChangedBO
    {
        protected override ChangedElementTypeEnum GetElementType() => ChangedElementTypeEnum.Response;

        private readonly OpenApiResponses _oldApiResponses;
        private readonly OpenApiResponses _newApiResponses;
        private readonly DiffContextBO _context;
        
        public Dictionary<string, OpenApiResponse> Increased { get; set; }
        public Dictionary<string, OpenApiResponse> Missing { get; set; }
        public Dictionary<string, ChangedResponseBO> Changed { get; set; }
        public ChangedExtensionsBO Extensions { get; set; }

        public ChangedAPIResponseBO(OpenApiResponses oldApiResponses, OpenApiResponses newApiResponses, DiffContextBO context) 
        {
            _oldApiResponses = oldApiResponses;
            _newApiResponses = newApiResponses;
            _context = context;
            Increased = new Dictionary<string, OpenApiResponse>();
            Missing = new Dictionary<string, OpenApiResponse>();
            Changed = new Dictionary<string, ChangedResponseBO>();
        }

        public override List<(string Identifier, ChangedBO Change)> GetChangedElements()
        {
            return new List<(string Identifier, ChangedBO Change)>(
                    Changed.Select(x => (x.Key, (ChangedBO)x.Value))
                )
                {
                    (null, Extensions)
                }
                .Where(x => x.Change != null).ToList();
        }

        public override DiffResultBO IsCoreChanged()
        {
            if (Increased.IsNullOrEmpty() && Missing.IsNullOrEmpty())
            {
                return new DiffResultBO(DiffResultEnum.NoChanges);
            }
            if (!Increased.IsNullOrEmpty() && Missing.IsNullOrEmpty())
            {
                return new DiffResultBO(DiffResultEnum.Compatible);
            }
            return new DiffResultBO(DiffResultEnum.Incompatible);
        }

        protected override List<ChangedInfoBO> GetCoreChanges() =>
            GetCoreChangeInfosOfComposed(Increased.Keys.ToList(), Missing.Keys.ToList(), x => x);
    }
}
