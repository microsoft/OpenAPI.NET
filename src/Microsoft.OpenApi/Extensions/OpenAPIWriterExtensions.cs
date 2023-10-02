using Microsoft.OpenApi.Writers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return new OpenApiWriterSettings();
        }
    }
}
