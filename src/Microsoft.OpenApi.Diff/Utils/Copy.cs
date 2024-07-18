using System.Collections.Generic;

namespace Microsoft.OpenApi.Diff.Utils
{
    public static class Copy
    {
        public static Dictionary<T1, T2> CopyDictionary<T1, T2>(this Dictionary<T1, T2> dict)
        {
            return dict == null ? new Dictionary<T1, T2>() : new Dictionary<T1, T2>(dict);
        }
    }
}
