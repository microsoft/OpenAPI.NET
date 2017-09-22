
namespace Tavis.OpenApi
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using OpenApi.Model;

    public static class OpenAPIDiff

    {
        public static List<OpenApiDifference> Compare(OpenApiDocument source, OpenApiDocument target)
        {
            var diffs = new List<OpenApiDifference>();
            target.Diff(diffs, source);
            return diffs;
        }
    }

    public class OpenApiDifference { }

}
