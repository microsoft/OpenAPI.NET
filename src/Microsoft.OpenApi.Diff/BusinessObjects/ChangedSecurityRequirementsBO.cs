using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Diff.Enums;
using Microsoft.OpenApi.Diff.Extensions;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Diff.BusinessObjects
{
    public class ChangedSecurityRequirementsBO : ComposedChangedBO
    {
        protected override ChangedElementTypeEnum GetElementType() => ChangedElementTypeEnum.SecurityRequirement;

        private readonly IList<OpenApiSecurityRequirement> _oldSecurityRequirements;
        private readonly IList<OpenApiSecurityRequirement> _newSecurityRequirements;

        public List<OpenApiSecurityRequirement> Missing { get; set; }
        public List<OpenApiSecurityRequirement> Increased { get; set; }
        public List<ChangedSecurityRequirementBO> Changed { get; set; }

        public ChangedSecurityRequirementsBO(IList<OpenApiSecurityRequirement> oldSecurityRequirements, IList<OpenApiSecurityRequirement> newSecurityRequirements) 
        {
            _oldSecurityRequirements = oldSecurityRequirements;
            _newSecurityRequirements = newSecurityRequirements;
            Missing = new List<OpenApiSecurityRequirement>();
            Increased = new List<OpenApiSecurityRequirement>();
            Changed = new List<ChangedSecurityRequirementBO>();
        }

        public override List<(string Identifier, ChangedBO Change)> GetChangedElements()
        {
            return new List<(string Identifier, ChangedBO Change)>(
                    Changed.Select(x => (GetElementType().GetDisplayName(), (ChangedBO)x))
                )
                .Where(x => x.Change != null).ToList();
        }

        public override DiffResultBO IsCoreChanged()
        {
            if (Missing.IsNullOrEmpty() && Increased.IsNullOrEmpty())
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
            GetCoreChangeInfosOfComposed(Increased.SelectMany(x => x.Keys).ToList(), Missing.SelectMany(x => x.Keys).ToList(), x => x.Name);
    }
}
