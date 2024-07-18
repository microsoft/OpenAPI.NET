using System.Collections.Generic;
using Microsoft.OpenApi.Diff.Enums;
using Microsoft.OpenApi.Diff.Extensions;
using Microsoft.OpenApi.Extensions;

namespace Microsoft.OpenApi.Diff.BusinessObjects
{
    public abstract class ChangedBO
    {
        protected ChangedBO()
        {
        }

        protected abstract ChangedElementTypeEnum GetElementType();
        protected string GetIdentifier(string identifier) => identifier ?? GetElementType().GetDisplayName();

        public abstract DiffResultBO IsChanged();

        public virtual DiffResultBO IsCoreChanged() => IsChanged();

        protected abstract List<ChangedInfoBO> GetCoreChanges();

        public ChangedInfosBO GetCoreChangeInfo(string identifier, List<string> parentPath = null)
        {
            var isChanged = IsCoreChanged();
            var newPath = new List<string>();

            if (!parentPath.IsNullOrEmpty())
                newPath = new List<string>(parentPath);

            newPath.Add(GetIdentifier(identifier));

            var result = new ChangedInfosBO
            {
                Path = newPath,
                ChangeType = isChanged
            };

            if (isChanged.IsUnchanged())
                return result;

            result.Changes = GetCoreChanges();
            return result;
        }

        public virtual List<ChangedInfosBO> GetAllChangeInfoFlat(string identifier, List<string> parentPath = null)
        {
            return new List<ChangedInfosBO>
            {
                GetCoreChangeInfo(identifier, parentPath)
            };
        }

        public static DiffResultBO Result(ChangedBO changed)
        {
            return changed?.IsChanged() ?? new DiffResultBO(DiffResultEnum.NoChanges);
        }
        public bool IsCompatible()
        {
            return IsChanged().IsCompatible();
        }

        public bool IsIncompatible()
        {
            return IsChanged().IsIncompatible();
        }

        public bool IsUnchanged()
        {
            return IsChanged().IsUnchanged();
        }

        public bool IsDifferent()
        {
            return IsChanged().IsDifferent();
        }
    }
}
