using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Diff.Enums;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Diff.BusinessObjects
{
    public class ChangedOAuthFlowsBO : ComposedChangedBO
    {
        protected override ChangedElementTypeEnum GetElementType() => ChangedElementTypeEnum.AuthFlow;

        private readonly OpenApiOAuthFlows _oldOAuthFlows;
        private readonly OpenApiOAuthFlows _newOAuthFlows;

        public ChangedOAuthFlowBO ImplicitOAuthFlow { get; set; }
        public ChangedOAuthFlowBO PasswordOAuthFlow { get; set; }
        public ChangedOAuthFlowBO ClientCredentialOAuthFlow { get; set; }
        public ChangedOAuthFlowBO AuthorizationCodeOAuthFlow { get; set; }
        public ChangedExtensionsBO Extensions { get; set; }

        public ChangedOAuthFlowsBO(OpenApiOAuthFlows oldOAuthFlows, OpenApiOAuthFlows newOAuthFlows)
        {
            _oldOAuthFlows = oldOAuthFlows;
            _newOAuthFlows = newOAuthFlows;
        }

        public override List<(string Identifier, ChangedBO Change)> GetChangedElements()
        {
            return new List<(string Identifier, ChangedBO Change)>
                {
                    ("Implicit", ImplicitOAuthFlow),
                    ("Password", PasswordOAuthFlow),
                    ("ClientCredential", ClientCredentialOAuthFlow),
                    ("AuthorizationCode", AuthorizationCodeOAuthFlow),
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
