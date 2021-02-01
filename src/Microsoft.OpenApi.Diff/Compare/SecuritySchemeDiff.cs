using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Diff.BusinessObjects;
using Microsoft.OpenApi.Diff.Utils;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Diff.Compare
{
    public class SecuritySchemeDiff : ReferenceDiffCache<OpenApiSecurityScheme, ChangedSecuritySchemeBO>
    {
        private readonly OpenApiDiff _openApiDiff;
        private readonly OpenApiComponents _leftComponents;
        private readonly OpenApiComponents _rightComponents;

        public SecuritySchemeDiff(OpenApiDiff openApiDiff)
        {
            _openApiDiff = openApiDiff;
            _leftComponents = openApiDiff.OldSpecOpenApi?.Components;
            _rightComponents = openApiDiff.NewSpecOpenApi?.Components;
        }

        public ChangedSecuritySchemeBO Diff(
          string leftSchemeRef,
          List<string> leftScopes,
          string rightSchemeRef,
          List<string> rightScopes,
          DiffContextBO context)
        {
            var leftSecurityScheme = _leftComponents.SecuritySchemes[leftSchemeRef];
            var rightSecurityScheme = _rightComponents.SecuritySchemes[rightSchemeRef];
            var changedSecuritySchemeOpt =
                CachedDiff(
                    new HashSet<string>(),
                    leftSecurityScheme,
                    rightSecurityScheme,
                    leftSchemeRef,
                    rightSchemeRef,
                    context);
            var changedSecurityScheme =
                changedSecuritySchemeOpt ?? new ChangedSecuritySchemeBO(leftSecurityScheme, rightSecurityScheme);
            changedSecurityScheme = GetCopyWithoutScopes(changedSecurityScheme);

            if (changedSecurityScheme != null
                && leftSecurityScheme.Type == SecuritySchemeType.OAuth2)
            {
                var changed = ChangedUtils.IsChanged(ListDiff.Diff(
                    new ChangedSecuritySchemeScopesBO(leftScopes, rightScopes)
                ));

                if (changed != null)
                    changedSecurityScheme.ChangedScopes = changed;
            }

            return ChangedUtils.IsChanged(changedSecurityScheme);
        }

        protected override ChangedSecuritySchemeBO ComputeDiff(
            HashSet<string> refSet,
            OpenApiSecurityScheme leftSecurityScheme,
            OpenApiSecurityScheme rightSecurityScheme,
            DiffContextBO context)
        {
            var changedSecurityScheme =
                new ChangedSecuritySchemeBO(leftSecurityScheme, rightSecurityScheme)
                {
                    Description = _openApiDiff
                        .MetadataDiff
                        .Diff(leftSecurityScheme.Description, rightSecurityScheme.Description, context)
                };

            switch (leftSecurityScheme.Type)
            {
                case SecuritySchemeType.ApiKey:
                    changedSecurityScheme.IsChangedIn =
                        !leftSecurityScheme.In.Equals(rightSecurityScheme.In);
                    break;
                case SecuritySchemeType.Http:
                    changedSecurityScheme.IsChangedScheme =
                        leftSecurityScheme.Scheme != rightSecurityScheme.Scheme;
                    changedSecurityScheme.IsChangedBearerFormat =
                        leftSecurityScheme.BearerFormat != rightSecurityScheme.BearerFormat;
                    break;
                case SecuritySchemeType.OAuth2:
                    changedSecurityScheme.OAuthFlows = _openApiDiff
                        .OAuthFlowsDiff
                        .Diff(leftSecurityScheme.Flows, rightSecurityScheme.Flows);
                    break;
                case SecuritySchemeType.OpenIdConnect:
                    changedSecurityScheme.IsChangedOpenIdConnectUrl =
                        leftSecurityScheme.OpenIdConnectUrl != rightSecurityScheme.OpenIdConnectUrl;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            changedSecurityScheme.Extensions = _openApiDiff
                .ExtensionsDiff
                .Diff(leftSecurityScheme.Extensions, rightSecurityScheme.Extensions, context);

            return changedSecurityScheme;
        }

        private static ChangedSecuritySchemeBO GetCopyWithoutScopes(ChangedSecuritySchemeBO original)
        {
            return new ChangedSecuritySchemeBO(
                    original.OldSecurityScheme, original.NewSecurityScheme)
            {
                IsChangedType = original.IsChangedType,
                IsChangedIn = original.IsChangedIn,
                IsChangedScheme = original.IsChangedScheme,
                IsChangedBearerFormat = original.IsChangedBearerFormat,
                Description = original.Description,
                OAuthFlows = original.OAuthFlows,
                IsChangedOpenIdConnectUrl = original.IsChangedOpenIdConnectUrl
            };
        }
    }
}
