using Microsoft.OpenApi.Diff.BusinessObjects;

namespace Microsoft.OpenApi.Diff.Utils
{
    public static class ChangedUtils
    {
        public static bool IsUnchanged(ChangedBO changed)
        {
            return changed == null || changed.IsUnchanged();
        }

        public static bool IsCompatible(ChangedBO changed)
        {
            return changed == null || changed.IsCompatible();
        }

        public static T IsChanged<T>(T changed)
        where T : ChangedBO
        {
            return IsUnchanged(changed) ? null : changed;
        }
    }
}
