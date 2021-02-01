using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Diff.Enums;
using Microsoft.OpenApi.Diff.Extensions;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Diff.BusinessObjects
{
    public class ChangedOpenApiBO : ComposedChangedBO
    {
        protected override ChangedElementTypeEnum GetElementType() => ChangedElementTypeEnum.OpenApi;
        public string OldSpecIdentifier { get; set; }
        public string NewSpecIdentifier { get; set; }
        public OpenApiDocument OldSpecOpenApi { get; set; }
        public OpenApiDocument NewSpecOpenApi { get; set; }
        public List<EndpointBO> NewEndpoints { get; set; }
        public List<EndpointBO> MissingEndpoints { get; set; }
        public List<ChangedOperationBO> ChangedOperations { get; set; }
        public ChangedExtensionsBO ChangedExtensions { get; set; }

        public ChangedOpenApiBO(string oldSpecIdentifier, string newSpecIdentifier) 
        {
            NewEndpoints = new List<EndpointBO>();
            MissingEndpoints = new List<EndpointBO>();
            ChangedOperations = new List<ChangedOperationBO>();
            OldSpecIdentifier = oldSpecIdentifier;
            NewSpecIdentifier = newSpecIdentifier;
        }
        public List<EndpointBO> GetDeprecatedEndpoints()
        {
            return ChangedOperations
                .Where(x => x.IsDeprecated)
                .Select(x => x.ConvertToEndpoint())
                .ToList();
        }

        public override List<(string Identifier, ChangedBO Change)> GetChangedElements()
        {
            return new List<(string Identifier, ChangedBO Change)> (
                    ChangedOperations.Select(x => (x.PathUrl, (ChangedBO)x))
                    )
                {
                    (null, ChangedExtensions)
                }
                .Where(x => x.Change != null).ToList();
        }

        public override DiffResultBO IsCoreChanged()
        {
            if (NewEndpoints.IsNullOrEmpty() && MissingEndpoints.IsNullOrEmpty())
            {
                return new DiffResultBO(DiffResultEnum.NoChanges);
            }
            if (MissingEndpoints.IsNullOrEmpty())
            {
                return new DiffResultBO(DiffResultEnum.Compatible);
            }
            return new DiffResultBO(DiffResultEnum.Incompatible);
        }

        protected override List<ChangedInfoBO> GetCoreChanges() =>
            GetCoreChangeInfosOfComposed(NewEndpoints, MissingEndpoints, x => x.PathUrl);
    }
}
