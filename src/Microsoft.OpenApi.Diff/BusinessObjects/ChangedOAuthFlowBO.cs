using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Diff.Enums;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Diff.BusinessObjects
{
    public class ChangedOAuthFlowBO : ComposedChangedBO
    {
        protected override ChangedElementTypeEnum GetElementType() => ChangedElementTypeEnum.AuthFlow;

        public OpenApiOAuthFlow OldOAuthFlow { get; }
        public OpenApiOAuthFlow NewOAuthFlow { get; }

        public bool ChangedAuthorizationUrl { get; set; }
        public bool ChangedTokenUrl { get; set; }
        public bool ChangedRefreshUrl { get; set; }
        public ChangedExtensionsBO Extensions { get; set; }

        public ChangedOAuthFlowBO(OpenApiOAuthFlow oldOAuthFlow, OpenApiOAuthFlow newOAuthFlow)
        {
            OldOAuthFlow = oldOAuthFlow;
            NewOAuthFlow = newOAuthFlow;
        }

        public override List<(string Identifier, ChangedBO Change)> GetChangedElements()
        {
            return new List<(string Identifier, ChangedBO Change)>
                {
                    (null, Extensions)
                }
                .Where(x => x.Change != null).ToList();
        }

        public override DiffResultBO IsCoreChanged()
        {
            if (ChangedAuthorizationUrl || ChangedTokenUrl || ChangedRefreshUrl)
            {
                return new DiffResultBO(DiffResultEnum.Incompatible);
            }
            return new DiffResultBO(DiffResultEnum.NoChanges);
        }

        protected override List<ChangedInfoBO> GetCoreChanges() 
        {
            var returnList = new List<ChangedInfoBO>();
            var elementType = GetElementType();
            const TypeEnum changeType = TypeEnum.Changed;

            if (ChangedAuthorizationUrl)
                returnList.Add(new ChangedInfoBO(elementType, changeType, "AuthorizationUrl", OldOAuthFlow?.AuthorizationUrl.ToString(), NewOAuthFlow?.AuthorizationUrl.ToString()));

            if (ChangedTokenUrl)
                returnList.Add(new ChangedInfoBO(elementType, changeType, "TokenUrl", OldOAuthFlow?.TokenUrl.ToString(), NewOAuthFlow?.TokenUrl.ToString()));

            if (ChangedRefreshUrl)
                returnList.Add(new ChangedInfoBO(elementType, changeType, "RefreshUrl", OldOAuthFlow?.RefreshUrl.ToString(), NewOAuthFlow?.RefreshUrl.ToString()));

            return returnList;
        }
    }
}
