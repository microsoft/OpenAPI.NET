using System.Collections.Generic;

namespace Microsoft.OpenApi.Diff.BusinessObjects
{
    public class ChangedInfosBO
    {
        public List<string> Path { get; set; }
        public DiffResultBO ChangeType { get; set; }
        public List<ChangedInfoBO> Changes { get; set; }
    }
}
