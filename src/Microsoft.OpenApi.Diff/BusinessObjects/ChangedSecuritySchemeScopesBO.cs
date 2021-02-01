using System.Collections.Generic;
using Microsoft.OpenApi.Diff.Enums;

namespace Microsoft.OpenApi.Diff.BusinessObjects
{
    public class ChangedSecuritySchemeScopesBO : ChangedListBO<string>
    {
        protected override ChangedElementTypeEnum GetElementType() => ChangedElementTypeEnum.SecuritySchemeScope;

        public ChangedSecuritySchemeScopesBO(List<string> oldValue, List<string> newValue) : base(oldValue, newValue, null)
        {
        }

        public override DiffResultBO IsItemsChanged()
        {
            return new DiffResultBO(DiffResultEnum.Incompatible);
        }
    }
}
