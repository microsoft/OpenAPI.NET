using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Diff.Enums;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Diff.BusinessObjects
{
    public class ChangedResponseBO : ComposedChangedBO
    {
        protected override ChangedElementTypeEnum GetElementType() => ChangedElementTypeEnum.Response;
       
        private readonly DiffContextBO _context;
        public OpenApiResponse OldApiResponse { get; }
        public OpenApiResponse NewApiResponse { get; }
        public ChangedMetadataBO Description { get; set; }
        public ChangedHeadersBO Headers { get; set; }
        public ChangedContentBO Content { get; set; }
        public ChangedExtensionsBO Extensions { get; set; }

        public ChangedResponseBO(OpenApiResponse oldApiResponse, OpenApiResponse newApiResponse, DiffContextBO context) 
        {
            OldApiResponse = oldApiResponse;
            NewApiResponse = newApiResponse;
            _context = context;
        }

        public override List<(string Identifier, ChangedBO Change)> GetChangedElements()
        {
            return new List<(string Identifier, ChangedBO Change)>
                {
                    (null, Description),
                    (null, Headers),
                    (null, Content),
                    (null, Extensions)
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
