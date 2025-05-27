// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json.Nodes;
using Microsoft.OpenApi.MicrosoftExtensions;

namespace Microsoft.OpenApi.Reader
{
    /// <summary>
    /// Configuration settings to control how OpenAPI documents are parsed
    /// </summary>
    public class OpenApiReaderSettings
    {
        private static readonly Lazy<HttpClient> httpClient = new(() => new HttpClient());
        private HttpClient? _httpClient;
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
            TryAddReader(OpenApiConstants.Json, new OpenApiJsonReader());
        }
        /// <summary>
        /// Gets the reader for the specified format
        /// </summary>
        /// <param name="format">Format to fetch the reader for</param>
        /// <returns>The retrieved reader</returns>
        /// <exception cref="NotSupportedException">When no reader is registered for that format</exception>
        internal IOpenApiReader GetReader(string format)
        {
            Utils.CheckArgumentNullOrEmpty(format);
            if (Readers.TryGetValue(format, out var reader))
            {
                return reader;
            }

            throw new NotSupportedException($"Format '{format}' is not supported.");
        }
        /// <summary>
        /// Adds a reader for the specified format.
        /// This method is a no-op if the reader already exists.
        /// This method is equivalent to TryAdd, is provided for compatibility reasons and TryAdd should be used instead when available.
        /// </summary>
        /// <param name="format">Format to add a reader for</param>
        /// <param name="reader">Reader to add</param>
        /// <returns>True if the reader was added, false if it already existed</returns>
        public bool TryAddReader(string format, IOpenApiReader reader)
        {
            Utils.CheckArgumentNullOrEmpty(format);
            Utils.CheckArgumentNull(reader);
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP || NET5_0_OR_GREATER
            return Readers.TryAdd(format, reader);
#else
            if (!Readers.ContainsKey(format))
            {
                Readers.Add(format, reader);
                return true;
            }
            return false;
#endif
        }
        private Dictionary<string, IOpenApiReader> _readers = new(StringComparer.OrdinalIgnoreCase)
        {
            { OpenApiConstants.Json, new OpenApiJsonReader() }
        };
        /// <summary>
        /// Readers to use to parse the OpenAPI document
        /// </summary>
        public Dictionary<string, IOpenApiReader> Readers
        { 
            get => _readers;
            init
            {
                Utils.CheckArgumentNull(value);
                _readers = value.Comparer is StringComparer stringComparer && stringComparer == StringComparer.OrdinalIgnoreCase ?
                    value :
                    new Dictionary<string, IOpenApiReader>(value, StringComparer.OrdinalIgnoreCase);
            }
        }
        /// <summary>
        /// When external references are found, load them into a shared workspace
        /// </summary>
        public bool LoadExternalRefs { get; set; } = false;

        /// <summary>
        /// Dictionary of parsers for converting extensions into strongly typed classes
        /// </summary>
        public Dictionary<string, Func<JsonNode, OpenApiSpecVersion, IOpenApiExtension>>? ExtensionParsers { get; set; } = new();

        /// <summary>
        /// Rules to use for validating OpenAPI specification.  If none are provided a default set of rules are applied.
        /// </summary>
        public ValidationRuleSet RuleSet { get; set; } = ValidationRuleSet.GetDefaultRuleSet();

        /// <summary>
        /// URL where relative references should be resolved from if the description does not contain Server definitions
        /// </summary>
        public Uri? BaseUrl { get; set; }

        /// <summary>
        /// Allows clients to define a custom DefaultContentType if produces array is empty
        /// </summary>
        public List<string>? DefaultContentType { get; set; }

        /// <summary>
        /// Function used to provide an alternative loader for accessing external references.
        /// </summary>
        /// <remarks>
        /// Default loader will attempt to dereference http(s) urls and file urls.
        /// </remarks>
        public IStreamLoader? CustomExternalLoader { get; set; }

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
            TryAddExtensionParser(OpenApiPagingExtension.Name, static (i, _) => OpenApiPagingExtension.Parse(i));
            TryAddExtensionParser(OpenApiEnumValuesDescriptionExtension.Name, static (i, _ ) => OpenApiEnumValuesDescriptionExtension.Parse(i));
            TryAddExtensionParser(OpenApiPrimaryErrorMessageExtension.Name, static (i, _ ) => OpenApiPrimaryErrorMessageExtension.Parse(i));
            TryAddExtensionParser(OpenApiDeprecationExtension.Name, static (i, _ ) => OpenApiDeprecationExtension.Parse(i));
            TryAddExtensionParser(OpenApiReservedParameterExtension.Name, static (i, _ ) => OpenApiReservedParameterExtension.Parse(i));
            TryAddExtensionParser(OpenApiEnumFlagsExtension.Name, static (i, _ ) => OpenApiEnumFlagsExtension.Parse(i));
        }
      
        private void TryAddExtensionParser(string name, Func<JsonNode, OpenApiSpecVersion, IOpenApiExtension> parser)
        {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP || NET5_0_OR_GREATER
            ExtensionParsers?.TryAdd(name, parser);
#else
            if (ExtensionParsers is not null && !ExtensionParsers.ContainsKey(name))
                ExtensionParsers.Add(name, parser);
#endif
        }
    }
}
