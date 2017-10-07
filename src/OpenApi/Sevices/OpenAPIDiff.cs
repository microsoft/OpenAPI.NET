
namespace Microsoft.OpenApi.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public static class OpenAPIDiff

    {
        public static List<OpenApiDifference> Compare(OpenApiDocument source, OpenApiDocument target)
        {
            var diffs = new List<OpenApiDifference>();
            return diffs;
        }


    }

    public class OpenApiDifference { }

}
