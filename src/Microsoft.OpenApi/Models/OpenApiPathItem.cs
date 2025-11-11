// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// Path Item Object: to describe the operations available on a single path.
    /// </summary>
    public class OpenApiPathItem : IOpenApiExtensible, IOpenApiPathItem
    {
        /// <inheritdoc/>
        public string? Summary { get; set; }

        /// <inheritdoc/>
        public string? Description { get; set; }

        /// <inheritdoc/>
        public Dictionary<HttpMethod, OpenApiOperation>? Operations { get; set; }

        /// <inheritdoc/>
        public IList<OpenApiServer>? Servers { get; set; }

        /// <inheritdoc/>
        public IList<IOpenApiParameter>? Parameters { get; set; }

        /// <inheritdoc/>
        public IDictionary<string, IOpenApiExtension>? Extensions { get; set; }

        /// <summary>
        /// Add one operation into this path item.
        /// </summary>
        /// <param name="operationType">The operation type kind.</param>
        /// <param name="operation">The operation item.</param>
        public void AddOperation(HttpMethod operationType, OpenApiOperation operation)
        {
            Operations ??= [];
            Operations[operationType] = operation;
        }

        /// <summary>
        /// Parameterless constructor
        /// </summary>
        public OpenApiPathItem() { }

        /// <summary>
        /// Initializes a clone of an <see cref="OpenApiPathItem"/> object
        /// </summary>
        internal OpenApiPathItem(IOpenApiPathItem pathItem)
        {
            Utils.CheckArgumentNull(pathItem);
            Summary = pathItem.Summary ?? Summary;
            Description = pathItem.Description ?? Description;
            Operations = pathItem.Operations != null ? new Dictionary<HttpMethod, OpenApiOperation>(pathItem.Operations) : null;
            Servers = pathItem.Servers != null ? [.. pathItem.Servers] : null;
            Parameters = pathItem.Parameters != null ? [.. pathItem.Parameters] : null;
            Extensions = pathItem.Extensions != null ? new Dictionary<string, IOpenApiExtension>(pathItem.Extensions) : null;
        }

        /// <summary>
        /// Serialize <see cref="OpenApiPathItem"/> to Open Api v3.2
        /// </summary>
        public virtual void SerializeAsV32(IOpenApiWriter writer)
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_2, (writer, element) => element.SerializeAsV32(writer));
        }

        /// <summary>
        /// Serialize <see cref="OpenApiPathItem"/> to Open Api v3.1
        /// </summary>
        public virtual void SerializeAsV31(IOpenApiWriter writer)
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_1, (writer, element) => element.SerializeAsV31(writer));
        }

        /// <summary>
        /// Serialize <see cref="OpenApiPathItem"/> to Open Api v3.0
        /// </summary>
        public virtual void SerializeAsV3(IOpenApiWriter writer)
        {
            SerializeInternal(writer, OpenApiSpecVersion.OpenApi3_0, (writer, element) => element.SerializeAsV3(writer));
        }

        /// <summary>
        /// Serialize inline PathItem in OpenAPI V2
        /// </summary>
        /// <param name="writer"></param>
        public virtual void SerializeAsV2(IOpenApiWriter writer)
        {
            Utils.CheckArgumentNull(writer);

            writer.WriteStartObject();

            // operations except "trace"
            if (Operations != null)
            {

                foreach (var operation in Operations.Where(o => _standardHttp2MethodsNames.Contains(o.Key.Method, StringComparer.OrdinalIgnoreCase)))
                {
                    writer.WriteOptionalObject(
                    operation.Key.Method.ToLowerInvariant(),
                    operation.Value,
                    (w, o) => o.SerializeAsV2(w));
                }
                var nonStandardOperations = Operations.Where(o => !_standardHttp2MethodsNames.Contains(o.Key.Method, StringComparer.OrdinalIgnoreCase)).ToDictionary(static o => o.Key.Method, static o => o.Value);
                if (nonStandardOperations.Count > 0)
                {
                    writer.WriteRequiredMap($"x-oai-{OpenApiConstants.AdditionalOperations}", nonStandardOperations, (w, o) => o.SerializeAsV2(w));
                }
            }

            // parameters
            writer.WriteOptionalCollection(OpenApiConstants.Parameters, Parameters, (w, p) => p.SerializeAsV2(w));

            // write "summary" as extensions
            writer.WriteProperty(OpenApiConstants.ExtensionFieldNamePrefix + OpenApiConstants.Summary, Summary);

            // write "description" as extensions
            writer.WriteProperty(
                OpenApiConstants.ExtensionFieldNamePrefix + OpenApiConstants.Description,
                Description);

            // specification extensions
            writer.WriteExtensions(Extensions, OpenApiSpecVersion.OpenApi2_0);

            writer.WriteEndObject();
        }

        internal static readonly HashSet<string> _standardHttp2MethodsNames = new(StringComparer.OrdinalIgnoreCase)
        {
            "get",
            "put",
            "post",
            "delete",
            "options",
            "head",
            "patch",
        };

        internal static readonly HashSet<string> _standardHttp30MethodsNames = new(_standardHttp2MethodsNames, StringComparer.OrdinalIgnoreCase)
        {
            "trace",
        };

        internal static readonly HashSet<string> _standardHttp31MethodsNames = new(_standardHttp30MethodsNames, StringComparer.OrdinalIgnoreCase)
        {
        };

        internal static readonly HashSet<string> _standardHttp32MethodsNames = new(_standardHttp31MethodsNames, StringComparer.OrdinalIgnoreCase)
        {
            "query",
        };

        internal virtual void SerializeInternal(IOpenApiWriter writer, OpenApiSpecVersion version,
            Action<IOpenApiWriter, IOpenApiSerializable> callback)
        {
            Utils.CheckArgumentNull(writer);

            writer.WriteStartObject();

            // summary
            writer.WriteProperty(OpenApiConstants.Summary, Summary);

            // description
            writer.WriteProperty(OpenApiConstants.Description, Description);

            var standardMethodsNames = version switch
            {
                OpenApiSpecVersion.OpenApi3_2 => _standardHttp32MethodsNames,
                OpenApiSpecVersion.OpenApi3_1 => _standardHttp31MethodsNames,
                OpenApiSpecVersion.OpenApi3_0 => _standardHttp30MethodsNames,
                // OpenAPI 2.0 supports the fewest methods, so it's the safest default in case of an unknown version.
                // This way the library will emit additional methods as extensions instead of producing a potentially invalid spec.
                _ => _standardHttp2MethodsNames,
            };

            // operations
            if (Operations != null)
            {
                foreach (var operation in Operations.Where(o => standardMethodsNames.Contains(o.Key.Method, StringComparer.OrdinalIgnoreCase)))
                {
                    writer.WriteOptionalObject(
                    operation.Key.Method.ToLowerInvariant(),
                    operation.Value,
                    callback);
                }
                var nonStandardOperations = Operations.Where(o => !standardMethodsNames.Contains(o.Key.Method, StringComparer.OrdinalIgnoreCase)).ToDictionary(static o => o.Key.Method, static o => o.Value);
                if (nonStandardOperations.Count > 0)
                {
                    var additionalOperationsPropertyName = version switch
                    {
                        OpenApiSpecVersion.OpenApi2_0 or OpenApiSpecVersion.OpenApi3_0 or OpenApiSpecVersion.OpenApi3_1 =>
                            $"x-oai-{OpenApiConstants.AdditionalOperations}",
                        _ => OpenApiConstants.AdditionalOperations,
                    };
                    writer.WriteRequiredMap(additionalOperationsPropertyName, nonStandardOperations, (w, o) => o.SerializeAsV32(w));
                }
            }

            // servers
            writer.WriteOptionalCollection(OpenApiConstants.Servers, Servers, callback);

            // parameters
            writer.WriteOptionalCollection(OpenApiConstants.Parameters, Parameters, callback);

            // specification extensions
            writer.WriteExtensions(Extensions, version);

            writer.WriteEndObject();
        }

        /// <inheritdoc/>
        public IOpenApiPathItem CreateShallowCopy()
        {
            return new OpenApiPathItem(this);
        }
    }
}
