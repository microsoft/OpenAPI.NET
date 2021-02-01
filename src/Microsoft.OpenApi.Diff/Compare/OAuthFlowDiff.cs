using Microsoft.OpenApi.Diff.BusinessObjects;
using Microsoft.OpenApi.Diff.Utils;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Diff.Compare
{
    public class OAuthFlowDiff
    {
        private readonly OpenApiDiff _openApiDiff;
        public OAuthFlowDiff(OpenApiDiff openApiDiff)
        {
            _openApiDiff = openApiDiff;
        }

        public ChangedOAuthFlowBO Diff(OpenApiOAuthFlow left, OpenApiOAuthFlow right)
        {
            var changedOAuthFlow = new ChangedOAuthFlowBO(left, right);
            if (left != null && right != null)
            {
                changedOAuthFlow.ChangedAuthorizationUrl = left.AuthorizationUrl != right.AuthorizationUrl;
                changedOAuthFlow.ChangedTokenUrl = left.TokenUrl != right.TokenUrl;
                changedOAuthFlow.ChangedRefreshUrl = left.RefreshUrl != right.RefreshUrl;
            }

            changedOAuthFlow.Extensions = _openApiDiff
                .ExtensionsDiff
                .Diff(left?.Extensions, right?.Extensions);

            return ChangedUtils.IsChanged(changedOAuthFlow);
        }
    }
}
