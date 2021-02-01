using System.Collections.Generic;
using Microsoft.OpenApi.Diff.Enums;
using Microsoft.OpenApi.Diff.Extensions;

namespace Microsoft.OpenApi.Diff.BusinessObjects
{
    public class ChangedEnumBO : ChangedListBO<string>
    {
        protected override ChangedElementTypeEnum GetElementType() => ChangedElementTypeEnum.Enum;

        public ChangedEnumBO(IList<string> oldValue, IList<string> newValue, DiffContextBO context) : base(oldValue, newValue, context)
        {
        }

        public override DiffResultBO IsItemsChanged()
        {
            if (Context.IsRequest && Missing.IsNullOrEmpty()
                || Context.IsResponse && Increased.IsNullOrEmpty())
            {
                return new DiffResultBO(DiffResultEnum.Compatible);
            }
            return new DiffResultBO(DiffResultEnum.Incompatible);
        }
    }
}
