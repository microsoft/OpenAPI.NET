using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Readers.ParseNodes;
using Microsoft.OpenApi.Validations;
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

        /// <summary>
        /// Rules to use for validating OpenAPI specification.  If none are provided a default set of rules are applied.
        /// </summary>
        public ValidationRuleSet RuleSet { get; set; }

    }
}
