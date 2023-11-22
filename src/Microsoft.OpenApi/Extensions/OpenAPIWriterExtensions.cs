using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi
{
    internal static class OpenAPIWriterExtensions
    {
        /// <summary>
        /// Temporary extension method until we add Settings property to IOpenApiWriter in next major version
        /// </summary>
        /// <param name="openApiWriter"></param>
        /// <returns></returns>
        internal static OpenApiWriterSettings GetSettings(this IOpenApiWriter openApiWriter)
        {
            if (openApiWriter is OpenApiWriterBase @base)
            {
                return @base.Settings;
            }
            return new();
        }
    }
}
