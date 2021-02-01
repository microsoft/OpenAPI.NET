using Microsoft.OpenApi.Diff.Enums;

namespace Microsoft.OpenApi.Diff.BusinessObjects
{
    public class DiffResultBO
    {
        public readonly DiffResultEnum DiffResult;

        public DiffResultBO(DiffResultEnum diffResult)
        {
            DiffResult = diffResult;
        }

        public bool IsUnchanged()
        {
            return DiffResult == 0;
        }

        public bool IsDifferent()
        {
            return DiffResult > 0;
        }

        public bool IsIncompatible()
        {
            return (int)DiffResult > 2;
        }

        public bool IsCompatible()
        {
            return (int)DiffResult <= 2;
        }

        public bool IsMetaChanged()
        {
            return (int)DiffResult == 1;
        }
    }
}
