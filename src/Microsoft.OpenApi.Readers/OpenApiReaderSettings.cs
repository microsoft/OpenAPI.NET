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
    /// Indicates if and when the reader should convert unresolved references into resolved objects
    /// </summary>
    public enum ReferenceResolutionSetting
    {
        /// <summary>
        /// Create placeholder objects with an OpenApiReference instance and UnresolvedReference set to true.
        /// </summary>
        DoNotResolveReferences,
        /// <summary>
        /// Convert local references to references of valid domain objects.
        /// </summary>
        ResolveLocalReferences,
        /// <summary>
        /// Convert all references to references of valid domain objects.
        /// </summary>
        ResolveAllReferences
    }

    /// <summary>
    /// Configuration settings to control how OpenAPI documents are parsed
    /// </summary>
    public class OpenApiReaderSettings
    {
        /// <summary>
        /// Indicates how references in the source document should be handled.
        /// </summary>
        public ReferenceResolutionSetting ReferenceResolution { get; set; } = ReferenceResolutionSetting.ResolveLocalReferences;

        /// <summary>
        /// Dictionary of parsers for converting extensions into strongly typed classes
        /// </summary>
        public Dictionary<string, Func<IOpenApiAny , OpenApiSpecVersion, IOpenApiExtension>> ExtensionParsers { get; set; } = new Dictionary<string, Func<IOpenApiAny, OpenApiSpecVersion, IOpenApiExtension>>();

        /// <summary>
        /// Rules to use for validating OpenAPI specification.  If none are provided a default set of rules are applied.
        /// </summary>
        public ValidationRuleSet RuleSet { get; set; } = ValidationRuleSet.GetDefaultRuleSet();

        /// <summary>
        /// URL where relative references should be resolved from if the description does not contain Server definitions
        /// </summary>
        public Uri BaseUrl { get; set; } 
    }
}
