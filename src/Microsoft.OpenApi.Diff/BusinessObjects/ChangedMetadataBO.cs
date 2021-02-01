using System.Collections.Generic;
using Microsoft.OpenApi.Diff.Enums;

namespace Microsoft.OpenApi.Diff.BusinessObjects
{
    public class ChangedMetadataBO : ChangedBO
    {
        protected override ChangedElementTypeEnum GetElementType() => ChangedElementTypeEnum.Metadata;

        public string Left { get; }
        public string Right { get; }

        public ChangedMetadataBO(string left, string right)
        {
            Left = left;
            Right = right;
        }

        public override DiffResultBO IsChanged()
        {
            return Left == Right ? new DiffResultBO(DiffResultEnum.NoChanges) : new DiffResultBO(DiffResultEnum.Metadata);
        }

        protected override List<ChangedInfoBO> GetCoreChanges()
        {
            var returnList = new List<ChangedInfoBO>();
            var elementType = GetElementType();
            const TypeEnum changeType = TypeEnum.Changed;

            if (Left != Right)
                returnList.Add(new ChangedInfoBO(elementType, changeType, "Value", Left, Right));

            return returnList;
        }
    }
}
