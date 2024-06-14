using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Diff.Enums;
using Microsoft.OpenApi.Diff.Extensions;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Diff.BusinessObjects
{
    public class ChangedParametersBO : ComposedChangedBO
    {
        protected override ChangedElementTypeEnum GetElementType() => ChangedElementTypeEnum.Parameter;

        private readonly List<OpenApiParameter> _oldParameterList;
        private readonly List<OpenApiParameter> _newParameterList;
        private readonly DiffContextBO _context;
        public List<OpenApiParameter> Increased { get; set; }
        public List<OpenApiParameter> Missing { get; set; }
        public List<ChangedParameterBO> Changed { get; set; }

        public ChangedParametersBO(List<OpenApiParameter> oldParameterList, List<OpenApiParameter> newParameterList, DiffContextBO context)
        {
            _oldParameterList = oldParameterList;
            _newParameterList = newParameterList;
            _context = context;
            Increased = new List<OpenApiParameter>();
            Missing = new List<OpenApiParameter>();
            Changed = new List<ChangedParameterBO>();
        }

        public override List<(string Identifier, ChangedBO Change)> GetChangedElements()
        {
            return new List<(string Identifier, ChangedBO Change)> (
                    Changed.Select(x => (x.Name, (ChangedBO)x))
                    )
                .Where(x => x.Change != null).ToList();
        }

        public override DiffResultBO IsCoreChanged()
        {
            if (Increased.IsNullOrEmpty() && Missing.IsNullOrEmpty())
            {
                return new DiffResultBO(DiffResultEnum.NoChanges);
            }

            if (Increased.Any(x => x.Required) && Missing.IsNullOrEmpty())
                return new DiffResultBO(DiffResultEnum.Compatible);

            return new DiffResultBO(DiffResultEnum.Incompatible);
        }

        protected override List<ChangedInfoBO> GetCoreChanges() =>
            GetCoreChangeInfosOfComposed(Increased, Missing, x => x.Name);
    }
}
