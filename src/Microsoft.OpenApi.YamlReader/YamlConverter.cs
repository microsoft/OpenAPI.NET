using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using SharpYaml;
using SharpYaml.Serialization;

namespace Microsoft.OpenApi.YamlReader
{
    /// <summary>
    /// Provides extensions to convert YAML models to JSON models.
    /// </summary>
    /// <remarks>
    /// These limits apply after a SharpYaml model exists. Use <see cref="OpenApiYamlReader"/>
    /// for untrusted input so limits are enforced before SharpYaml model loading.
    /// </remarks>
    public static class YamlConverter
    {
        /// <summary>
        /// Default maximum nesting depth allowed when converting a YAML node graph into JSON nodes.
        /// Mirrors the default System.Text.Json depth limit (64) that already bounds the JSON reader path,
        /// protecting the recursive conversion from stack exhaustion on deeply nested documents.
        /// </summary>
        public const uint DefaultMaxDepth = 64;

        /// <summary>
        /// Default maximum number of JSON nodes that may be materialized from a single YAML document.
        /// Guards against YAML anchor/alias expansion ("billion laughs") attacks, where a tiny document
        /// expands exponentially when its shared node graph is materialized into an independent JSON tree.
        /// </summary>
        public const uint DefaultMaxNodeCount = 5_000_000;

        /// <summary>
        /// Default maximum number of JSON nodes that may be materialized from YAML aliases.
        /// </summary>
        public const uint DefaultMaxAliasExpansionNodeCount = 5_000;

        /// <summary>
        /// Maximum configurable YAML nesting depth.
        /// </summary>
        public const uint MaximumAllowedDepth = 256;

        /// <summary>
        /// Maximum configurable number of JSON nodes that may be materialized from a single YAML document.
        /// Bounds the running node totals so they cannot overflow while accumulating, which would surface as an
        /// <see cref="OverflowException"/> instead of a reportable diagnostic.
        /// </summary>
        public const uint MaximumAllowedNodeCount = 10_000_000;

        private static uint _maxDepth = DefaultMaxDepth;
        private static uint _maxNodeCount = DefaultMaxNodeCount;
        private static uint _maxAliasExpansionNodeCount = DefaultMaxAliasExpansionNodeCount;

        /// <summary>
        /// Gets or sets the maximum nesting depth allowed when converting a YAML node graph into JSON nodes.
        /// Defaults to <see cref="DefaultMaxDepth"/> and cannot exceed the library's safe depth ceiling.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when set outside the supported range.</exception>
        public static uint MaxDepth
        {
            get => _maxDepth;
            set
            {
                ValidateMaxDepth(value, nameof(value));
                _maxDepth = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum number of JSON nodes that may be materialized from a single YAML document.
        /// Defaults to <see cref="DefaultMaxNodeCount"/>, guarding against YAML anchor/alias expansion
        /// ("billion laughs") attacks. Raise this if legitimate large documents are being rejected, or lower
        /// it to fail faster when only small documents are expected. Cannot exceed
        /// <see cref="MaximumAllowedNodeCount"/>.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when set outside the supported range.</exception>
        public static uint MaxNodeCount
        {
            get => _maxNodeCount;
            set
            {
                ValidateMaxNodeCount(value, nameof(value));
                _maxNodeCount = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum number of JSON nodes that may be materialized from YAML aliases.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when set to zero.</exception>
        public static uint MaxAliasExpansionNodeCount
        {
            get => _maxAliasExpansionNodeCount;
            set
            {
                if (value == 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "MaxAliasExpansionNodeCount must be greater than zero.");
                }

                _maxAliasExpansionNodeCount = value;
            }
        }

        internal static void ValidateMaxDepth(uint value, string parameterName)
        {
            if (value == 0 || value > MaximumAllowedDepth)
            {
                throw new ArgumentOutOfRangeException(parameterName, $"MaxDepth must be between 1 and {MaximumAllowedDepth}.");
            }
        }

        internal static void ValidateMaxNodeCount(uint value, string parameterName)
        {
            if (value == 0 || value > MaximumAllowedNodeCount)
            {
                throw new ArgumentOutOfRangeException(parameterName, $"MaxNodeCount must be between 1 and {MaximumAllowedNodeCount}.");
            }
        }

        /// <summary>
        /// Converts all of the documents in a YAML stream to <see cref="JsonNode"/>s.
        /// </summary>
        /// <param name="yaml">The YAML stream.</param>
        /// <returns>A collection of nodes representing the YAML documents in the stream.</returns>
        public static IEnumerable<JsonNode> ToJsonNode(this YamlStream yaml)
        {
            return yaml.Documents.Select(x => x.ToJsonNode());
        }

        /// <summary>
        /// Converts a single YAML document to a <see cref="JsonNode"/>.
        /// </summary>
        /// <param name="yaml">The YAML document.</param>
        /// <returns>A `JsonNode` representative of the YAML document.</returns>
        public static JsonNode ToJsonNode(this YamlDocument yaml)
        {
            return yaml.RootNode.ToJsonNode();
        }

        /// <summary>
        /// Converts a single YAML node to a <see cref="JsonNode"/>.
        /// </summary>
        /// <param name="yaml">The YAML node.</param>
        /// <returns>A `JsonNode` representative of the YAML node.</returns>
        /// <exception cref="NotSupportedException">Thrown for YAML that is not compatible with JSON.</exception>
        public static JsonNode ToJsonNode(this YamlNode yaml)
        {
            return CreateConversionContext().Convert(yaml, 0).Node;
        }

        /// <summary>
        /// Converts a single JSON node to a <see cref="YamlNode"/>.
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public static YamlNode ToYamlNode(this JsonNode json)
        {
            return json switch
            {
                JsonObject obj => obj.ToYamlMapping(),
                JsonArray arr => arr.ToYamlSequence(),
                JsonValue nullVal when JsonNullSentinel.IsJsonNullSentinel(nullVal) => new YamlScalarNode("null")
                {
                    Style = ScalarStyle.Plain
                },
                JsonValue val => val.ToYamlScalar(),
                _ => throw new NotSupportedException("This isn't a supported JsonNode")
            };
        }

        /// <summary>
        /// Converts a <see cref="YamlMappingNode"/> to a <see cref="JsonObject"/>.
        /// </summary>
        /// <param name="yaml"></param>
        /// <returns></returns>
        public static JsonObject ToJsonObject(this YamlMappingNode yaml)
        {
            return (JsonObject)CreateConversionContext().Convert(yaml, 0).Node;
        }

        private static YamlMappingNode ToYamlMapping(this JsonObject obj)
        {
            return new YamlMappingNode(obj.ToDictionary(x => (YamlNode)new YamlScalarNode(x.Key)
            {
                Style = NeedsQuoting(x.Key) ? ScalarStyle.DoubleQuoted : ScalarStyle.Plain
            }, x => x.Value!.ToYamlNode()));
        }

        /// <summary>
        /// Converts a <see cref="YamlSequenceNode"/> to a <see cref="JsonArray"/>.
        /// </summary>
        /// <param name="yaml"></param>
        /// <returns></returns>
        public static JsonArray ToJsonArray(this YamlSequenceNode yaml)
        {
            return (JsonArray)CreateConversionContext().Convert(yaml, 0).Node;
        }

        private static YamlSequenceNode ToYamlSequence(this JsonArray arr)
        {
            return new YamlSequenceNode(arr.Select(x => x!.ToYamlNode()));
        }

        private static readonly HashSet<string> YamlNullRepresentations = new(StringComparer.Ordinal)
        {
            "~",
            "null",
            "Null",
            "NULL"
        };

        private static YamlConversionContext CreateConversionContext()
        {
            var maxDepth = MaxDepth;
            var maxNodeCount = MaxNodeCount;
            var maxAliasExpansionNodeCount = MaxAliasExpansionNodeCount;
            ValidateMaxDepth(maxDepth, nameof(MaxDepth));
            return new(
                new YamlConversionBudget(maxDepth, maxNodeCount, maxAliasExpansionNodeCount));
        }

        internal static JsonValue ToJsonValue(string? value, ScalarStyle style)
        {
            return style switch
            {
                ScalarStyle.Plain when decimal.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var d) => JsonValue.Create(d),
                ScalarStyle.Plain when bool.TryParse(value, out var b) => JsonValue.Create(b),
                ScalarStyle.Plain when value is not null && YamlNullRepresentations.Contains(value) => (JsonValue)JsonNullSentinel.JsonNull.DeepClone(),
                ScalarStyle.Plain => JsonValue.Create(value ?? string.Empty),
                ScalarStyle.SingleQuoted or ScalarStyle.DoubleQuoted or ScalarStyle.Literal or ScalarStyle.Folded or ScalarStyle.Any => JsonValue.Create(value ?? string.Empty),
                _ => throw new ArgumentOutOfRangeException(nameof(style)),
            };
        }

        private sealed class YamlConversionContext
        {
            private readonly YamlConversionBudget _budget;
            private readonly Dictionary<YamlNode, MaterializedNode> _completed = new(ReferenceEqualityComparer<YamlNode>.Instance);
            private readonly HashSet<YamlNode> _active = new(ReferenceEqualityComparer<YamlNode>.Instance);

            public YamlConversionContext(YamlConversionBudget budget)
            {
                _budget = budget;
            }

            public MaterializedNode Convert(YamlNode yaml, uint depth)
            {
                try
                {
                    RuntimeHelpers.EnsureSufficientExecutionStack();
                }
                catch (InsufficientExecutionStackException ex)
                {
                    throw new OpenApiReaderException("The YAML node graph is too deeply nested to convert safely.", ex);
                }

                if (_active.Contains(yaml))
                {
                    throw new OpenApiReaderException("The YAML node graph contains a cycle.");
                }

                if (_completed.TryGetValue(yaml, out var completed))
                {
                    _budget.EnterAlias(depth, completed.NodeCount, completed.Height);
                    return new(completed.Node.DeepClone(), completed.NodeCount, completed.Height);
                }

                _budget.EnterNode(depth);
                _active.Add(yaml);
                try
                {
                    var materialized = yaml switch
                    {
                        YamlMappingNode map => ConvertMapping(map, depth),
                        YamlSequenceNode sequence => ConvertSequence(sequence, depth),
                        YamlScalarNode scalar => new MaterializedNode(ToJsonValue(scalar.Value, scalar.Style), 1, 1),
                        _ => throw new NotSupportedException("This yaml isn't convertible to JSON")
                    };
                    _completed.Add(yaml, materialized);
                    return materialized;
                }
                finally
                {
                    _active.Remove(yaml);
                }
            }

            private MaterializedNode ConvertMapping(YamlMappingNode yaml, uint depth)
            {
                var node = new JsonObject();
                uint nodeCount = 1;
                uint maxChildHeight = 0;
                foreach (var keyValuePair in yaml)
                {
                    if (keyValuePair.Key is not YamlScalarNode scalarKey || scalarKey.Value is null)
                    {
                        throw new OpenApiReaderException("YAML mapping keys must be scalar values.");
                    }

                    if (node.ContainsKey(scalarKey.Value))
                    {
                        throw new OpenApiReaderException($"The YAML mapping contains the duplicate key '{scalarKey.Value}'.");
                    }

                    var child = Convert(keyValuePair.Value, depth + 1);
                    node.Add(scalarKey.Value, child.Node);
                    nodeCount = checked(nodeCount + child.NodeCount);
                    maxChildHeight = Math.Max(maxChildHeight, child.Height);
                }

                return new(node, nodeCount, maxChildHeight + 1);
            }

            private MaterializedNode ConvertSequence(YamlSequenceNode yaml, uint depth)
            {
                var node = new JsonArray();
                uint nodeCount = 1;
                uint maxChildHeight = 0;
                foreach (var value in yaml)
                {
                    var child = Convert(value, depth + 1);
                    node.Add(child.Node);
                    nodeCount = checked(nodeCount + child.NodeCount);
                    maxChildHeight = Math.Max(maxChildHeight, child.Height);
                }

                return new(node, nodeCount, maxChildHeight + 1);
            }
        }

        private sealed class MaterializedNode
        {
            public MaterializedNode(JsonNode node, uint nodeCount, uint height)
            {
                Node = node;
                NodeCount = nodeCount;
                Height = height;
            }

            public JsonNode Node { get; }
            public uint NodeCount { get; }

            /// <summary>Number of levels in this subtree, where a scalar has height 1.</summary>
            public uint Height { get; }
        }

        private sealed class ReferenceEqualityComparer<T> : IEqualityComparer<T> where T : class
        {
            public static ReferenceEqualityComparer<T> Instance { get; } = new();

            public bool Equals(T? x, T? y) => ReferenceEquals(x, y);

            public int GetHashCode(T obj) => RuntimeHelpers.GetHashCode(obj);
        }

        private static bool NeedsQuoting(string value) =>
        string.IsNullOrEmpty(value) ||
        decimal.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out _) ||
        bool.TryParse(value, out _) ||
        YamlNullRepresentations.Contains(value);

        private static YamlScalarNode ToYamlScalar(this JsonValue val)
        {
            // Try to get the underlying value based on its actual type
            // First try to get it as a string
            if (val.GetValueKind() == JsonValueKind.String &&
                val.TryGetValue(out string? stringValue))
            {
                // For string values, we need to determine if they should be quoted in YAML
                // Strings that look like numbers, booleans, or null need to be quoted
                // to preserve their string type when round-tripping
                var needsQuoting = NeedsQuoting(stringValue);

                var containsNewLine = stringValue.Contains('\n');

                var style = (needsQuoting, containsNewLine) switch
                {
                    (true, _) => ScalarStyle.DoubleQuoted,
                    (false, true) => ScalarStyle.Literal,
                    (false, false) => ScalarStyle.Plain
                };
                
                return new YamlScalarNode(stringValue)
                {
                    Style = style
                };
            }
            
            // For non-string values (numbers, booleans, null), use their string representation
            // These should remain unquoted in YAML
            var valueString = val.ToString();
            return new YamlScalarNode(valueString)
            {
                Style = ScalarStyle.Plain
            };
        }
    }
}
