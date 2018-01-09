using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Readers.ParseNodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OpenApi.Readers
{
    /// <summary>
    /// Configuration settings to control how OpenAPI documents are parsed
    /// </summary>
    public class OpenApiReaderSettings
    {
        /// <summary>
        /// Dictionary of parsers for converting extensions into strongly typed classes
        /// </summary>
        public Dictionary<string, Func<IOpenApiAny , IOpenApiExtension>> ExtensionParsers { get; set; } = new Dictionary<string, Func<IOpenApiAny, IOpenApiExtension>>();

    }
}
