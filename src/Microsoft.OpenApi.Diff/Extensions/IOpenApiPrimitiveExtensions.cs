using System.IO;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Diff.Extensions
{
    public static class IOpenApiPrimitiveExtensions
    {
        public static string GetValueString(this IOpenApiPrimitive primitive)
        {
            using var sb = new StringWriter();
            var writer = new OpenApiYamlWriter(sb);
            primitive.Write(writer, OpenApiSpecVersion.OpenApi3_0);
            return sb.ToString();
        }
    }
}
