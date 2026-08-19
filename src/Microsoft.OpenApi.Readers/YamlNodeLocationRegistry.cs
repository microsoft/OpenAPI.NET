// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;
using SharpYaml.Serialization;

namespace Microsoft.OpenApi.Readers
{
    /// <summary>
    /// Preserves parser locations for nodes created through SharpYaml's public constructors.
    /// </summary>
    internal static class YamlNodeLocationRegistry
    {
        private static readonly ConditionalWeakTable<YamlNode, NodeLocation> Locations = new();

        public static void Register(YamlNode node, int line)
        {
            Locations.Add(node, new(line));
        }

        public static int GetLine(YamlNode node)
        {
            if (node != null && Locations.TryGetValue(node, out var location))
            {
                return location.Line;
            }

            return node?.Start.Line ?? 0;
        }

        private sealed class NodeLocation
        {
            public NodeLocation(int line)
            {
                Line = line;
            }

            public int Line { get; }
        }
    }
}
