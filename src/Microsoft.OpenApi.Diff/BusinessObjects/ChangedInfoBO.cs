using Microsoft.OpenApi.Diff.Enums;

namespace Microsoft.OpenApi.Diff.BusinessObjects
{
    public class ChangedInfoBO
    {
        public ChangedElementTypeEnum ElementType { get; }
        public TypeEnum ChangeType { get; }
        public string FieldName { get; }
        public string OldValue { get; }
        public string NewValue { get; }

        public ChangedInfoBO(ChangedElementTypeEnum elementType, TypeEnum changeType, string fieldName, string oldValue, string newValue)
        {
            ElementType = elementType;
            ChangeType = changeType;
            FieldName = fieldName;
            OldValue = oldValue;
            NewValue = newValue;
        }

        public static ChangedInfoBO ForAdded(ChangedElementTypeEnum elementType, string fieldName)
        {
            return new ChangedInfoBO(elementType, TypeEnum.Added, fieldName, null, null);
        }

        public static ChangedInfoBO ForRemoved(ChangedElementTypeEnum elementType, string fieldName)
        {
            return new ChangedInfoBO(elementType, TypeEnum.Removed, fieldName, null, null);
        }
    }
}
