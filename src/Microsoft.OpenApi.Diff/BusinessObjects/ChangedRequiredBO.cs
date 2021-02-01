using System.Collections.Generic;
using Microsoft.OpenApi.Diff.Enums;
using Microsoft.OpenApi.Diff.Extensions;

namespace Microsoft.OpenApi.Diff.BusinessObjects
{
    public class ChangedRequiredBO : ChangedListBO<string>
    {
        protected override ChangedElementTypeEnum GetElementType() => ChangedElementTypeEnum.Required;

        public ChangedRequiredBO(IList<string> oldValue, IList<string> newValue, DiffContextBO context) : base(oldValue, newValue, context)
        {
        }

        public override DiffResultBO IsItemsChanged()
        {
            if (Context.IsRequest && Increased.IsNullOrEmpty() 
                || Context.IsResponse && Missing.IsNullOrEmpty())
            {
                return new DiffResultBO(DiffResultEnum.Compatible);
            }
            return new DiffResultBO(DiffResultEnum.Incompatible);
        }
    }
}
