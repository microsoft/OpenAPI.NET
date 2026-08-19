// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.MicrosoftExtensions;
using Microsoft.OpenApi.Readers.Interface;
using Microsoft.OpenApi.Validations;
using System;
using System.Collections.Generic;
using System.IO;

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
        /// ResolveAllReferences effectively means load external references. Will be removed in v2. External references are never "resolved".
        /// </summary>
        ResolveAllReferences
    }

    /// <summary>
    /// Configuration settings to control how OpenAPI documents are parsed
    /// </summary>
    public class OpenApiReaderSettings
    {
        /// <summary>
        /// Default maximum nesting depth allowed when materializing values from a YAML/JSON node graph.
        /// Mirrors the default System.Text.Json depth limit (64), protecting the recursive readers
        /// from stack exhaustion on deeply nested documents.
        /// </summary>
        public const uint DefaultMaxDepth = 64;

        /// <summary>
        /// Default maximum number of nodes that may be materialized from a single document.
        /// Guards against YAML anchor/alias expansion ("billion laughs") attacks, where a tiny document
        /// expands exponentially when its shared node graph is materialized into an independent tree.
        /// </summary>
        public const uint DefaultMaxNodeCount = 5_000_000;

        /// <summary>
        /// Default maximum number of nodes that may be materialized specifically from YAML aliases.
        /// </summary>
        public const uint DefaultMaxAliasExpansionNodeCount = 5_000;

        /// <summary>
        /// Default maximum number of input bytes read from a single document (128 MiB).
        /// </summary>
        public const uint DefaultMaxInputByteCount = 128 * 1024 * 1024;

        /// <summary>
        /// Default maximum length of a single YAML scalar value in UTF-16 code units.
        /// </summary>
        public const uint DefaultMaxScalarLength = 64 * 1024;

        /// <summary>
        /// Maximum configurable document nesting depth.
        /// </summary>
        public const uint MaximumAllowedDepth = 256;

        /// <summary>
        /// Maximum configurable number of nodes materialized from a single document.
        /// </summary>
        public const uint MaximumAllowedNodeCount = 10_000_000;

        private uint _maxDepth = DefaultMaxDepth;
        private uint _maxNodeCount = DefaultMaxNodeCount;
        private uint _maxAliasExpansionNodeCount = DefaultMaxAliasExpansionNodeCount;
        private uint _maxInputByteCount = DefaultMaxInputByteCount;
        private uint _maxScalarLength = DefaultMaxScalarLength;

        /// <summary>
        /// Gets or sets the maximum nesting depth allowed when materializing values from a node graph.
        /// Defaults to <see cref="DefaultMaxDepth"/>. Raise this if legitimate deeply nested documents are
        /// being rejected, or lower it to fail faster when only shallow documents are expected.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when set outside the supported range.</exception>
        public uint MaxDepth
        {
            get => _maxDepth;
            set
            {
                if (value == 0 || value > MaximumAllowedDepth)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), $"MaxDepth must be between 1 and {MaximumAllowedDepth}.");
                }

                _maxDepth = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum number of nodes that may be materialized from a single document.
        /// Defaults to <see cref="DefaultMaxNodeCount"/>, guarding against YAML anchor/alias expansion
        /// ("billion laughs") attacks. Raise this if legitimate large documents are being rejected, or lower
        /// it to fail faster when only small documents are expected.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when set outside the supported range.</exception>
        public uint MaxNodeCount
        {
            get => _maxNodeCount;
            set
            {
                if (value == 0 || value > MaximumAllowedNodeCount)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), $"MaxNodeCount must be between 1 and {MaximumAllowedNodeCount}.");
                }

                _maxNodeCount = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum number of nodes materialized specifically from YAML aliases.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when set to zero.</exception>
        public uint MaxAliasExpansionNodeCount
        {
            get => _maxAliasExpansionNodeCount;
            set
            {
                ValidatePositive(value, nameof(value), nameof(MaxAliasExpansionNodeCount));
                _maxAliasExpansionNodeCount = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum number of input bytes read from one document. Stream readers
        /// count raw bytes; string and text-reader inputs count their UTF-8 encoded size.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when set to zero.</exception>
        public uint MaxInputByteCount
        {
            get => _maxInputByteCount;
            set
            {
                ValidatePositive(value, nameof(value), nameof(MaxInputByteCount));
                _maxInputByteCount = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum length of one YAML scalar in UTF-16 code units.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when set to zero.</exception>
        public uint MaxScalarLength
        {
            get => _maxScalarLength;
            set
            {
                ValidatePositive(value, nameof(value), nameof(MaxScalarLength));
                _maxScalarLength = value;
            }
        }

        /// <summary>
        /// Indicates how references in the source document should be handled.
        /// </summary>
        /// <remarks>This setting will be going away in the next major version of this library.  Use GetEffective on model objects to get resolved references.</remarks>
        public ReferenceResolutionSetting ReferenceResolution { get; set; } = ReferenceResolutionSetting.ResolveLocalReferences;

        /// <summary>
        /// When external references are found, load them into a shared workspace
        /// </summary>
        public bool LoadExternalRefs { get; set; } = false;

        /// <summary>
        /// Dictionary of parsers for converting extensions into strongly typed classes
        /// </summary>
        public Dictionary<string, Func<IOpenApiAny, OpenApiSpecVersion, IOpenApiExtension>> ExtensionParsers { get; set; } = new();

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
        /// from an <see cref="OpenApiStreamReader"/> object.
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

        private static void ValidatePositive(uint value, string parameterName, string propertyName)
        {
            if (value == 0)
            {
                throw new ArgumentOutOfRangeException(parameterName, $"{propertyName} must be greater than zero.");
            }
        }
    }
}
