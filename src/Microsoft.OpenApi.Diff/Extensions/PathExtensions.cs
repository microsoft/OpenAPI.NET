using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Microsoft.OpenApi.Diff.Extensions
{
    public static class PathExtensions
    {
        private const string RegexPath = "\\{([^/]+)\\}";

        public static string NormalizePath(this string path)
        {
            return Regex.Replace(path, RegexPath, "{}");
        }

        public static List<string> ExtractParametersFromPath(this string path)
        {
            var paramsList = new List<string>();
            var reg = new Regex(RegexPath);
            var matches = reg.Matches(path);
            if (!matches.IsNullOrEmpty())
            {
                foreach (Match m in matches)
                    paramsList.Add(m.Groups[1].Value);
            }
            return paramsList;
        }
    }
}
