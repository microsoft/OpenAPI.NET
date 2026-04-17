// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// TOML writer for Open API documents.
    /// </summary>
    /// <remarks>
    /// Produces TOML v1.0 output. Scalar values and inline arrays are written as
    /// key = value pairs; sub-tables are written as [section] headers; arrays of
    /// objects are written as [[array]] headers.
    /// TOML does not have a native null type, so null values are omitted.
    /// </remarks>
    public class OpenApiTomlWriter : OpenApiWriterBase, IOpenApiWriter
    {
        private static readonly Regex BareKeyPattern = new(@"^[A-Za-z0-9_-]+$", RegexOptions.Compiled);

        // Root table of the buffered document
        private TomlTable? _root;

        // Stack of currently open nodes (TomlTable or TomlArray)
        private readonly Stack<object> _nodeStack = new();

        // Property name pending to be used as the key for the next value
        private string? _pendingKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenApiTomlWriter"/> class.
        /// </summary>
        /// <param name="textWriter">The output text writer.</param>
        public OpenApiTomlWriter(TextWriter textWriter) : this(textWriter, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenApiTomlWriter"/> class.
        /// </summary>
        /// <param name="textWriter">The output text writer.</param>
        /// <param name="settings">Settings for controlling how the document is written.</param>
        public OpenApiTomlWriter(TextWriter textWriter, OpenApiWriterSettings? settings)
            : base(textWriter, settings)
        {
        }

        /// <inheritdoc/>
        protected override int BaseIndentation => 0;

        /// <inheritdoc/>
        public override void WriteStartObject()
        {
            StartScope(ScopeType.Object);
            var table = new TomlTable();
            if (_root is null && _nodeStack.Count == 0)
            {
                _root = table;
            }
            else
            {
                AddToParent(table);
            }
            _nodeStack.Push(table);
        }

        /// <inheritdoc/>
        public override void WriteEndObject()
        {
            EndScope(ScopeType.Object);
            _nodeStack.Pop();
        }

        /// <inheritdoc/>
        public override void WriteStartArray()
        {
            StartScope(ScopeType.Array);
            var array = new TomlArray();
            AddToParent(array);
            _nodeStack.Push(array);
        }

        /// <inheritdoc/>
        public override void WriteEndArray()
        {
            EndScope(ScopeType.Array);
            _nodeStack.Pop();
        }

        /// <inheritdoc/>
        public override void WritePropertyName(string name)
        {
            VerifyCanWritePropertyName(name);
            CurrentScope()!.ObjectCount++;
            _pendingKey = name;
        }

        /// <inheritdoc/>
        public override void WriteValue(string value) =>
            AddToParent(new TomlScalar(TomlScalarKind.String, value));

        /// <inheritdoc/>
        public override void WriteValue(int value) =>
            AddToParent(new TomlScalar(TomlScalarKind.Integer, value));

        /// <inheritdoc/>
        public override void WriteValue(bool value) =>
            AddToParent(new TomlScalar(TomlScalarKind.Boolean, value));

        /// <inheritdoc/>
        public override void WriteValue(decimal value) =>
            AddToParent(new TomlScalar(TomlScalarKind.Float, value));

        /// <inheritdoc/>
        public override void WriteValue(float value) =>
            AddToParent(new TomlScalar(TomlScalarKind.Float, (double)value));

        /// <inheritdoc/>
        public override void WriteValue(double value) =>
            AddToParent(new TomlScalar(TomlScalarKind.Float, value));

        /// <inheritdoc/>
        public override void WriteValue(long value) =>
            AddToParent(new TomlScalar(TomlScalarKind.Integer, value));

        /// <inheritdoc/>
        public override void WriteValue(DateTime value) =>
            AddToParent(new TomlScalar(TomlScalarKind.String, value.ToString("o", CultureInfo.InvariantCulture)));

        /// <inheritdoc/>
        public override void WriteValue(DateTimeOffset value) =>
            AddToParent(new TomlScalar(TomlScalarKind.String, value.ToString("o", CultureInfo.InvariantCulture)));

        /// <inheritdoc/>
        public override void WriteValue(object? value)
        {
            if (value is null) { WriteNull(); return; }
            if (value is string s) WriteValue(s);
            else if (value is int i) WriteValue(i);
            else if (value is uint u) WriteValue((long)u);
            else if (value is long l) WriteValue(l);
            else if (value is bool b) WriteValue(b);
            else if (value is float f) WriteValue(f);
            else if (value is double d) WriteValue(d);
            else if (value is decimal dc) WriteValue(dc);
            else if (value is DateTime dt) WriteValue(dt);
            else if (value is DateTimeOffset dto) WriteValue(dto);
            else if (value is System.Collections.Generic.IEnumerable<object> en) WriteEnumerable(en);
            else throw new OpenApiWriterException(string.Format(SRResource.OpenApiUnsupportedValueType, value.GetType().FullName));
        }

        /// <inheritdoc/>
        public override void WriteNull() => AddToParent(TomlNull.Instance);

        /// <inheritdoc/>
        public override void WriteRaw(string value) =>
            AddToParent(new TomlScalar(TomlScalarKind.Raw, value));

        /// <inheritdoc/>
        protected override void WriteValueSeparator()
        {
            // No-op: the tree structure manages value ordering.
        }

        /// <inheritdoc/>
        async Task IOpenApiWriter.FlushAsync(CancellationToken cancellationToken)
        {
            if (_root is not null)
            {
                var sb = new StringBuilder();
                SerializeTable(sb, _root, []);
                await Writer.WriteAsync(sb.ToString()).ConfigureAwait(false);
            }
#if NET8_OR_GREATER
            await Writer.FlushAsync(cancellationToken).ConfigureAwait(false);
#else
            await Writer.FlushAsync().ConfigureAwait(false);
#endif
        }

        private void AddToParent(object node)
        {
            if (_nodeStack.Count == 0)
            {
                return;
            }

            var parent = _nodeStack.Peek();
            if (parent is TomlTable table)
            {
                if (_pendingKey is null)
                {
                    throw new OpenApiWriterException("A property name must be written before writing a value.");
                }
                table.Add(_pendingKey, node);
                _pendingKey = null;
            }
            else if (parent is TomlArray array)
            {
                array.Items.Add(node);
            }
        }

        // ── TOML serialization ───────────────────────────────────────────────

        /// <summary>
        /// Recursively serialises a TOML table.
        /// Scalars are emitted first, then arrays of tables, then sub-tables.
        /// This ordering satisfies TOML's constraint that scalars of a section
        /// must precede any child table/array-of-tables sections.
        /// </summary>
        private static void SerializeTable(StringBuilder sb, TomlTable table, List<string> path)
        {
            // 1. Scalar key = value pairs (strings, numbers, booleans, inline arrays).
            foreach (var key in table.Keys)
            {
                var value = table.Entries[key];
                if (!IsInlineValue(value))
                {
                    continue;
                }
                sb.Append(EscapeKey(key));
                sb.Append(" = ");
                AppendInlineValue(sb, value);
                sb.AppendLine();
            }

            // 2. Arrays of tables – [[path.key]] header per element.
            foreach (var key in table.Keys)
            {
                var value = table.Entries[key];
                if (value is not TomlArray arr || !IsTableArray(arr))
                {
                    continue;
                }
                var newPath = new List<string>(path) { key };
                var pathStr = string.Join(".", newPath.Select(EscapeKey));
                foreach (TomlTable item in arr.Items.Cast<TomlTable>())
                {
                    sb.AppendLine();
                    sb.Append("[[");
                    sb.Append(pathStr);
                    sb.AppendLine("]]");
                    SerializeTable(sb, item, newPath);
                }
            }

            // 3. Sub-tables – [path.key] header (only when the sub-table has
            //    direct scalar content; otherwise the header is implied by deeper paths).
            foreach (var key in table.Keys)
            {
                var value = table.Entries[key];
                if (value is not TomlTable subTable)
                {
                    continue;
                }
                var newPath = new List<string>(path) { key };
                SerializeSubTable(sb, subTable, newPath);
            }
        }

        private static void SerializeSubTable(StringBuilder sb, TomlTable table, List<string> path)
        {
            bool hasDirectContent = table.Entries.Values.Any(IsInlineValue);
            if (hasDirectContent)
            {
                sb.AppendLine();
                sb.Append('[');
                sb.Append(string.Join(".", path.Select(EscapeKey)));
                sb.AppendLine("]");
            }
            SerializeTable(sb, table, path);
        }

        // ── Predicate helpers ────────────────────────────────────────────────

        private static bool IsInlineValue(object value) =>
            value switch
            {
                TomlScalar => true,
                TomlArray arr => !IsTableArray(arr),   // empty or scalar arrays → inline
                _ => false,                             // TomlNull, TomlTable → not inline
            };

        private static bool IsTableArray(TomlArray array) =>
            array.Items.Count > 0 && array.Items.All(i => i is TomlTable);

        // ── Value formatters ─────────────────────────────────────────────────

        private static void AppendInlineValue(StringBuilder sb, object value)
        {
            switch (value)
            {
                case TomlScalar scalar:
                    AppendScalar(sb, scalar);
                    break;
                case TomlArray array:
                    AppendInlineArray(sb, array);
                    break;
                case TomlTable table:
                    AppendInlineTable(sb, table);
                    break;
                case TomlNull:
                    // Null is not representable in TOML; callers skip null values.
                    break;
            }
        }

        private static void AppendScalar(StringBuilder sb, TomlScalar scalar)
        {
            switch (scalar.Kind)
            {
                case TomlScalarKind.String:
                    AppendTomlString(sb, (string)scalar.Value);
                    break;
                case TomlScalarKind.Integer:
                    sb.Append(Convert.ToInt64(scalar.Value, CultureInfo.InvariantCulture));
                    break;
                case TomlScalarKind.Float:
                    AppendTomlFloat(sb, Convert.ToDouble(scalar.Value, CultureInfo.InvariantCulture));
                    break;
                case TomlScalarKind.Boolean:
                    sb.Append((bool)scalar.Value ? "true" : "false");
                    break;
                case TomlScalarKind.Raw:
                    sb.Append((string)scalar.Value);
                    break;
            }
        }

        private static void AppendTomlFloat(StringBuilder sb, double value)
        {
            if (double.IsPositiveInfinity(value)) { sb.Append("inf"); return; }
            if (double.IsNegativeInfinity(value)) { sb.Append("-inf"); return; }
            if (double.IsNaN(value)) { sb.Append("nan"); return; }

            // Ensure the number is rendered in a way TOML parsers recognise as a float.
            var str = value.ToString("G17", CultureInfo.InvariantCulture);
            sb.Append(str);
            if (!str.Contains('.') && !str.Contains('E') && !str.Contains('e')
                && !str.Equals("inf", StringComparison.Ordinal)
                && !str.Equals("-inf", StringComparison.Ordinal)
                && !str.Equals("nan", StringComparison.Ordinal))
            {
                sb.Append(".0");
            }
        }

        private static void AppendInlineArray(StringBuilder sb, TomlArray array)
        {
            sb.Append('[');
            bool first = true;
            foreach (var item in array.Items)
            {
                if (item is TomlNull) { continue; }
                if (!first) { sb.Append(", "); }
                AppendInlineValue(sb, item);
                first = false;
            }
            sb.Append(']');
        }

        private static void AppendInlineTable(StringBuilder sb, TomlTable table)
        {
            sb.Append('{');
            bool first = true;
            foreach (var key in table.Keys)
            {
                var val = table.Entries[key];
                if (val is TomlNull) { continue; }
                if (!first) { sb.Append(", "); }
                sb.Append(EscapeKey(key));
                sb.Append(" = ");
                AppendInlineValue(sb, val);
                first = false;
            }
            sb.Append('}');
        }

        // ── Key / string escaping ────────────────────────────────────────────

        private static string EscapeKey(string key) =>
            BareKeyPattern.IsMatch(key) ? key : BuildTomlString(key);

        private static void AppendTomlString(StringBuilder sb, string value)
        {
            sb.Append(BuildTomlString(value));
        }

        private static string BuildTomlString(string value)
        {
            var sb = new StringBuilder("\"");
            foreach (var c in value)
            {
                switch (c)
                {
                    case '"':  sb.Append("\\\""); break;
                    case '\\': sb.Append("\\\\"); break;
                    case '\n': sb.Append("\\n");  break;
                    case '\r': sb.Append("\\r");  break;
                    case '\t': sb.Append("\\t");  break;
                    case '\b': sb.Append("\\b");  break;
                    case '\f': sb.Append("\\f");  break;
                    default:
                        if (c < 0x20 || c == 0x7F)
                        {
                            sb.Append($"\\u{(int)c:X4}");
                        }
                        else
                        {
                            sb.Append(c);
                        }
                        break;
                }
            }
            sb.Append('"');
            return sb.ToString();
        }

        // ── Private TOML node model ──────────────────────────────────────────

        private enum TomlScalarKind
        {
            String,
            Integer,
            Float,
            Boolean,
            Raw,
        }

        private sealed class TomlScalar
        {
            public TomlScalar(TomlScalarKind kind, object value)
            {
                Kind = kind;
                Value = value;
            }

            public TomlScalarKind Kind { get; }
            public object Value { get; }
        }

        private sealed class TomlNull
        {
            private TomlNull() { }
            public static readonly TomlNull Instance = new();
        }

        private sealed class TomlArray
        {
            public List<object> Items { get; } = new();
        }

        private sealed class TomlTable
        {
            private readonly Dictionary<string, object> _entries = new(StringComparer.Ordinal);

            public IReadOnlyDictionary<string, object> Entries => _entries;

            /// <summary>Insertion-ordered list of keys (preserves document order).</summary>
            public List<string> Keys { get; } = new();

            public void Add(string key, object value)
            {
                if (!_entries.ContainsKey(key))
                {
                    Keys.Add(key);
                }
                _entries[key] = value;
            }
        }
    }
}
