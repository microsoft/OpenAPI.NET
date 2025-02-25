// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json.Nodes;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.MicrosoftExtensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Validations;

namespace Microsoft.OpenApi.Reader
{
    /// <summary>
    /// Configuration settings to control how OpenAPI documents are parsed
    /// </summary>
    public class OpenApiReaderSettings
    {
        private static readonly Lazy<HttpClient> httpClient = new(() => new HttpClient());
        private HttpClient _httpClient;
        /// <summary>
        /// HttpClient to use for making requests and retrieve documents
        /// </summary>
        public HttpClient HttpClient
        { 
            internal get
            {
                _httpClient ??= httpClient.Value;
                return _httpClient;
            }
            init
            {
                _httpClient = value;
            }
        }
        /// <summary>
        /// Adds a reader for the specified format
        /// </summary>
        public void AddJsonReader()
        {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP || NET5_0_OR_GREATER
            Readers.TryAdd(OpenApiConstants.Json, new OpenApiJsonReader());
#else
            if (!Readers.ContainsKey(OpenApiConstants.Json))
            {
                Readers.Add(OpenApiConstants.Json, new OpenApiJsonReader());
            }
#endif
        }
        /// <summary>
        /// Readers to use to parse the OpenAPI document
        /// </summary>
        public Dictionary<string, IOpenApiReader> Readers { get; init; } = new Dictionary<string, IOpenApiReader>(StringComparer.OrdinalIgnoreCase)
        {
            { OpenApiConstants.Json, new OpenApiJsonReader() }
        };
        /// <summary>
        /// When external references are found, load them into a shared workspace
        /// </summary>
        public bool LoadExternalRefs { get; set; } = false;

        /// <summary>
        /// Dictionary of parsers for converting extensions into strongly typed classes
        /// </summary>
        public Dictionary<string, Func<JsonNode, OpenApiSpecVersion, IOpenApiExtension>> ExtensionParsers { get; set; } = new();

        /// <summary>
        /// Rules to use for validating OpenAPI specification.  If none are provided a default set of rules are applied.
        /// </summary>
        public ValidationRuleSet RuleSet { get; set; } = ValidationRuleSet.GetDefaultRuleSet();

        /// <summary>
        /// URL where relative references should be resolved from if the description does not contain Server definitions
        /// </summary>
        public Uri BaseUrl { get; set; }

        /// <summary>
        /// Allows clients to define a custom DefaultContentType if produces array is empty
        /// </summary>
        public List<string> DefaultContentType { get; set; }

        /// <summary>
        /// Function used to provide an alternative loader for accessing external references.
        /// </summary>
        /// <remarks>
        /// Default loader will attempt to dereference http(s) urls and file urls.
        /// </remarks>
        public IStreamLoader CustomExternalLoader { get; set; }

        /// <summary>
        /// Whether to leave the <see cref="Stream"/> object open after reading
        /// from an OpenApiStreamReader object.
        /// </summary>
        public bool LeaveStreamOpen { get; set; }

        /// <summary>
        /// Adds parsers for Microsoft OpenAPI extensions:
        /// - <see cref="OpenApiPagingExtension"/>
        /// - <see cref="OpenApiEnumValuesDescriptionExtension"/>
        /// - <see cref="OpenApiPrimaryErrorMessageExtension"/>
        /// - <see cref="OpenApiDeprecationExtension"/>
        /// - <see cref="OpenApiReservedParameterExtension"/>
        /// - <see cref="OpenApiEnumFlagsExtension"/>
        /// NOTE: The list of extensions is subject to change.
        /// </summary>
        public void AddMicrosoftExtensionParsers()
        {
            if (!ExtensionParsers.ContainsKey(OpenApiPagingExtension.Name))
                ExtensionParsers.Add(OpenApiPagingExtension.Name, static (i, _) => OpenApiPagingExtension.Parse(i));
            if (!ExtensionParsers.ContainsKey(OpenApiEnumValuesDescriptionExtension.Name))
                ExtensionParsers.Add(OpenApiEnumValuesDescriptionExtension.Name, static (i, _ ) => OpenApiEnumValuesDescriptionExtension.Parse(i));
            if (!ExtensionParsers.ContainsKey(OpenApiPrimaryErrorMessageExtension.Name))
                ExtensionParsers.Add(OpenApiPrimaryErrorMessageExtension.Name, static (i, _ ) => OpenApiPrimaryErrorMessageExtension.Parse(i));
            if (!ExtensionParsers.ContainsKey(OpenApiDeprecationExtension.Name))
                ExtensionParsers.Add(OpenApiDeprecationExtension.Name, static (i, _ ) => OpenApiDeprecationExtension.Parse(i));
            if (!ExtensionParsers.ContainsKey(OpenApiReservedParameterExtension.Name))
                ExtensionParsers.Add(OpenApiReservedParameterExtension.Name, static (i, _ ) => OpenApiReservedParameterExtension.Parse(i));
            if (!ExtensionParsers.ContainsKey(OpenApiEnumFlagsExtension.Name))
                ExtensionParsers.Add(OpenApiEnumFlagsExtension.Name, static (i, _ ) => OpenApiEnumFlagsExtension.Parse(i));
        }
    }
}
