// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.OpenApi.Readers.Exceptions;

namespace Microsoft.OpenApi.Readers
{
    /// <summary>
    /// Tracks resources consumed while composing a single YAML document.
    /// </summary>
    internal sealed class YamlResourceBudget
    {
        private readonly uint _maxDepth;
        private readonly uint _maxNodeCount;
        private readonly uint _maxAliasExpansionNodeCount;
        private uint _nodeCount;
        private uint _aliasExpansionNodeCount;

        public YamlResourceBudget(uint maxDepth, uint maxNodeCount, uint maxAliasExpansionNodeCount)
        {
            _maxDepth = maxDepth;
            _maxNodeCount = maxNodeCount;
            _maxAliasExpansionNodeCount = maxAliasExpansionNodeCount;
        }

        public void EnterNode(uint depth)
        {
            ValidateDepth(depth);
            AddNodes(1);
        }

        public void EnterAlias(uint depth, uint expandedNodeCount, uint expandedHeight)
        {
            ValidateDepth(depth);

            if (expandedHeight > _maxDepth - depth)
            {
                throw new OpenApiReaderException(
                    $"The YAML document expands an alias to more than the maximum supported nesting depth of {_maxDepth}.");
            }

            if (expandedNodeCount > _maxAliasExpansionNodeCount - _aliasExpansionNodeCount)
            {
                throw new OpenApiReaderException(
                    $"The YAML document expands aliases to more than the maximum supported number of nodes ({_maxAliasExpansionNodeCount}).");
            }

            _aliasExpansionNodeCount += expandedNodeCount;
            AddNodes(expandedNodeCount);
        }

        private void ValidateDepth(uint depth)
        {
            if (depth >= _maxDepth)
            {
                throw new OpenApiReaderException(
                    $"The YAML document exceeds the maximum supported nesting depth of {_maxDepth}.");
            }
        }

        private void AddNodes(uint count)
        {
            if (count > _maxNodeCount - _nodeCount)
            {
                throw new OpenApiReaderException(
                    $"The YAML document expands to more than the maximum supported number of nodes ({_maxNodeCount}). This may indicate a YAML anchor/alias expansion attack.");
            }

            _nodeCount += count;
        }
    }
}
