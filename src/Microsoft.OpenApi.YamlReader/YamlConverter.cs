using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using SharpYaml;
using SharpYaml.Serialization;

namespace Microsoft.OpenApi.YamlReader
{
    /// <summary>
    /// Provides extensions to convert YAML models to JSON models.
    /// </summary>
    public static class YamlConverter
    {
        /// <summary>
        /// Default maximum nesting depth allowed when converting a YAML node graph into JSON nodes.
        /// Mirrors the default System.Text.Json depth limit (64) that already bounds the JSON reader path,
        /// protecting the recursive conversion from stack exhaustion on deeply nested documents.
        /// </summary>
        internal const int DefaultMaxDepth = 64;

        /// <summary>
        /// Default maximum number of JSON nodes that may be materialized from a single YAML document.
        /// Guards against YAML anchor/alias expansion ("billion laughs") attacks, where a tiny document
        /// expands exponentially when its shared node graph is materialized into an independent JSON tree.
        /// Increase this only if legitimate large documents are being rejected.
        /// </summary>
        internal const int DefaultMaxNodeCount = 5_000_000;

        /// <summary>
        /// Tracks and enforces resource limits while converting a YAML node graph into JSON nodes,
        /// failing fast when a hostile document would otherwise exhaust memory or the stack.
        /// </summary>
        private sealed class YamlConversionBudget
        {
            private readonly int _maxDepth;
            private readonly int _maxNodeCount;
            private int _nodeCount;

            public YamlConversionBudget(int maxDepth = DefaultMaxDepth, int maxNodeCount = DefaultMaxNodeCount)
            {
                _maxDepth = maxDepth;
                _maxNodeCount = maxNodeCount;
            }

            public void EnterNode(int depth)
            {
                if (depth > _maxDepth)
                {
                    throw new OpenApiReaderException($"The YAML document exceeds the maximum supported nesting depth of {_maxDepth}.");
                }

                if (++_nodeCount > _maxNodeCount)
                {
                    throw new OpenApiReaderException($"The YAML document expands to more than the maximum supported number of nodes ({_maxNodeCount}). This may indicate a YAML anchor/alias expansion (billion laughs) attack.");
                }
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
            return yaml.ToJsonNode(new YamlConversionBudget(), 0);
        }

        private static JsonNode ToJsonNode(this YamlNode yaml, YamlConversionBudget budget, int depth)
        {
            budget.EnterNode(depth);
            return yaml switch
            {
                YamlMappingNode map => map.ToJsonObject(budget, depth),
                YamlSequenceNode seq => seq.ToJsonArray(budget, depth),
                YamlScalarNode scalar => scalar.ToJsonValue(),
                _ => throw new NotSupportedException("This yaml isn't convertible to JSON")
            };
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
            return yaml.ToJsonObject(new YamlConversionBudget(), 0);
        }

        private static JsonObject ToJsonObject(this YamlMappingNode yaml, YamlConversionBudget budget, int depth)
        {
            var node = new JsonObject();
            foreach (var keyValuePair in yaml)
            {
                var key = ((YamlScalarNode)keyValuePair.Key).Value!;
                node[key] = keyValuePair.Value.ToJsonNode(budget, depth + 1);
            }

            return node;
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
            return yaml.ToJsonArray(new YamlConversionBudget(), 0);
        }

        private static JsonArray ToJsonArray(this YamlSequenceNode yaml, YamlConversionBudget budget, int depth)
        {
            var node = new JsonArray();
            foreach (var value in yaml)
            {
                node.Add(value.ToJsonNode(budget, depth + 1));
            }

            return node;
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

        private static JsonValue ToJsonValue(this YamlScalarNode yaml)
        {
            return yaml.Style switch
            {
                ScalarStyle.Plain when decimal.TryParse(yaml.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var d) => JsonValue.Create(d),
                ScalarStyle.Plain when bool.TryParse(yaml.Value, out var b) => JsonValue.Create(b),
                ScalarStyle.Plain when YamlNullRepresentations.Contains(yaml.Value) => (JsonValue)JsonNullSentinel.JsonNull.DeepClone(),
                ScalarStyle.Plain => JsonValue.Create(yaml.Value),
                ScalarStyle.SingleQuoted or ScalarStyle.DoubleQuoted or ScalarStyle.Literal or ScalarStyle.Folded or ScalarStyle.Any => JsonValue.Create(yaml.Value),
                _ => throw new ArgumentOutOfRangeException(nameof(yaml)),
            };
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
