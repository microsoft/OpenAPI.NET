using System.Collections.Generic;
using Microsoft.OpenApi.Diff.Enums;

namespace Microsoft.OpenApi.Diff.BusinessObjects
{
    public class ChangedReadOnlyBO : ChangedBO
    {
        protected override ChangedElementTypeEnum GetElementType() => ChangedElementTypeEnum.ReadOnly;

        private readonly DiffContextBO _context;
        private readonly bool _oldValue;
        private readonly bool _newValue;

        public ChangedReadOnlyBO(bool? oldValue, bool? newValue, DiffContextBO context)
        {
            _context = context;
            _oldValue = oldValue ?? false;
            _newValue = newValue ?? false;
        }

        public override DiffResultBO IsChanged()
        {
            if (_oldValue == _newValue)
            {
                return new DiffResultBO(DiffResultEnum.NoChanges);
            }
            if (_context.IsResponse)
            {
                return new DiffResultBO(DiffResultEnum.Compatible);
            }
            if (_context.IsRequest)
            {
                if (_newValue)
                {
                    return new DiffResultBO(DiffResultEnum.Incompatible);
                }

                return _context.IsRequired ? new DiffResultBO(DiffResultEnum.Incompatible) : new DiffResultBO(DiffResultEnum.Compatible);
            }
            return new DiffResultBO(DiffResultEnum.Unknown);
        }

        protected override List<ChangedInfoBO> GetCoreChanges()
        {
            var returnList = new List<ChangedInfoBO>();
            var elementType = GetElementType();
            const TypeEnum changeType = TypeEnum.Changed;

            if (_oldValue != _newValue)
                returnList.Add(new ChangedInfoBO(elementType, changeType, _context.GetDiffContextElementType(), _oldValue.ToString(), _newValue.ToString()));

            return returnList;
        }
    }
}