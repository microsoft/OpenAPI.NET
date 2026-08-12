// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;

namespace Microsoft.OpenApi.Readers
{
    /// <summary>
    /// Resource limits applied while reading an OpenAPI description, protecting the reader from
    /// hostile documents that would otherwise exhaust memory or the stack.
    /// </summary>
    public static class OpenApiReaderLimits
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

        private static uint _maxDepth = DefaultMaxDepth;
        private static uint _maxNodeCount = DefaultMaxNodeCount;

        /// <summary>
        /// Gets or sets the maximum nesting depth allowed when materializing values from a node graph.
        /// Defaults to <see cref="DefaultMaxDepth"/>. Raise this if legitimate deeply nested documents are
        /// being rejected, or lower it to fail faster when only shallow documents are expected.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when set to zero.</exception>
        public static uint MaxDepth
        {
            get => _maxDepth;
            set
            {
                if (value == 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "MaxDepth must be greater than zero.");
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
        /// <exception cref="ArgumentOutOfRangeException">Thrown when set to zero.</exception>
        public static uint MaxNodeCount
        {
            get => _maxNodeCount;
            set
            {
                if (value == 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "MaxNodeCount must be greater than zero.");
                }

                _maxNodeCount = value;
            }
        }
    }
}
