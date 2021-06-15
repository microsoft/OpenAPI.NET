using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Diff.Enums;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Diff.BusinessObjects
{
    public class ChangedSecurityRequirementBO : ComposedChangedBO
    {
        protected override ChangedElementTypeEnum GetElementType() => ChangedElementTypeEnum.SecurityRequirement;

        private readonly OpenApiSecurityRequirement _oldSecurityRequirement;
        private readonly OpenApiSecurityRequirement _newSecurityRequirement;

        public OpenApiSecurityRequirement Missing { get; set; }
        public OpenApiSecurityRequirement Increased { get; set; }
        public List<ChangedSecuritySchemeBO> Changed { get; set; }

        public ChangedSecurityRequirementBO(OpenApiSecurityRequirement newSecurityRequirement, OpenApiSecurityRequirement oldSecurityRequirement)
        {
            _newSecurityRequirement = newSecurityRequirement;
            _oldSecurityRequirement = oldSecurityRequirement;
            Changed = new List<ChangedSecuritySchemeBO>();
        }

        public override List<(string Identifier, ChangedBO Change)> GetChangedElements()
        {
            return new List<(string Identifier, ChangedBO Change)>(
                    Changed.Select(x => (x.NewSecurityScheme.Name ?? x.OldSecurityScheme.Name, (ChangedBO)x))
                )
                .Where(x => x.Change != null).ToList();
        }

        public override DiffResultBO IsCoreChanged()
        {
            if (Increased == null && Missing == null)
            {
                return new DiffResultBO(DiffResultEnum.NoChanges);
            }
            if (Increased == null)
            {
                return new DiffResultBO(DiffResultEnum.Compatible);
            }
            return new DiffResultBO(DiffResultEnum.Incompatible);
        }

        protected override List<ChangedInfoBO> GetCoreChanges() =>
            GetCoreChangeInfosOfComposed(Increased.Keys.ToList(), Missing.Keys.ToList(), x => x.Name);
    }
}
