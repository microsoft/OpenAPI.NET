using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Diff.Enums;
using Microsoft.OpenApi.Diff.Extensions;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Diff.BusinessObjects
{
    public class ChangedSecuritySchemeBO : ComposedChangedBO
    {
        protected override ChangedElementTypeEnum GetElementType() => ChangedElementTypeEnum.SecurityScheme;

        public OpenApiSecurityScheme OldSecurityScheme { get; }
        public OpenApiSecurityScheme NewSecurityScheme { get; }

        public bool IsChangedType { get; set; }
        public bool IsChangedIn { get; set; }
        public bool IsChangedScheme { get; set; }
        public bool IsChangedBearerFormat { get; set; }
        public bool IsChangedOpenIdConnectUrl { get; set; }
        public ChangedSecuritySchemeScopesBO ChangedScopes { get; set; }
        public ChangedMetadataBO Description { get; set; }
        public ChangedOAuthFlowsBO OAuthFlows { get; set; }
        public ChangedExtensionsBO Extensions { get; set; }

        public ChangedSecuritySchemeBO(OpenApiSecurityScheme oldSecurityScheme, OpenApiSecurityScheme newSecurityScheme)
        {
            OldSecurityScheme = oldSecurityScheme;
            NewSecurityScheme = newSecurityScheme;
        }

        public override List<(string Identifier, ChangedBO Change)> GetChangedElements()
        {
            return new List<(string Identifier, ChangedBO Change)>
            {
                (null, Description), 
                (null, OAuthFlows), 
                (null, Extensions)
            }
                .Where(x => x.Change != null).ToList();
        }

        public override DiffResultBO IsCoreChanged()
        {
            if (!IsChangedType
                && !IsChangedIn
                && !IsChangedScheme
                && !IsChangedBearerFormat
                && !IsChangedOpenIdConnectUrl
                && (ChangedScopes == null || ChangedScopes.IsUnchanged()))
            {
                return new DiffResultBO(DiffResultEnum.NoChanges);
            }
            if (!IsChangedType
                && !IsChangedIn
                && !IsChangedScheme
                && !IsChangedBearerFormat
                && !IsChangedOpenIdConnectUrl
                && (ChangedScopes == null || ChangedScopes.Increased.IsNullOrEmpty()))
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

            if (IsChangedBearerFormat)
                returnList.Add(new ChangedInfoBO(elementType, changeType, "Bearer Format", OldSecurityScheme?.BearerFormat, NewSecurityScheme?.BearerFormat));

            if (IsChangedIn)
                returnList.Add(new ChangedInfoBO(elementType, changeType, "In", OldSecurityScheme?.In.ToString(), NewSecurityScheme?.In.ToString()));

            if (IsChangedOpenIdConnectUrl)
                returnList.Add(new ChangedInfoBO(elementType, changeType, "OpenIdConnect Url", OldSecurityScheme?.OpenIdConnectUrl.ToString(), NewSecurityScheme?.OpenIdConnectUrl.ToString()));

            if (IsChangedScheme)
                returnList.Add(new ChangedInfoBO(elementType, changeType, "Scheme", OldSecurityScheme?.Scheme, NewSecurityScheme?.Scheme));

            if (IsChangedType)
                returnList.Add(new ChangedInfoBO(elementType, changeType, "Type", OldSecurityScheme?.Type.ToString(), NewSecurityScheme?.Type.ToString()));

            return returnList;
        }
    }
}
