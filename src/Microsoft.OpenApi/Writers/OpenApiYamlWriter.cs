// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.IO;
using System.Linq;

namespace Microsoft.OpenApi.Writers
{
    /// <summary>
    /// YAML writer.
    /// </summary>
    public class OpenApiYamlWriter : OpenApiWriterBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenApiYamlWriter"/> class.
        /// </summary>
        /// <param name="textWriter">The text writer.</param>
        public OpenApiYamlWriter(TextWriter textWriter)
            : this(textWriter, new OpenApiSerializerSettings())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenApiYamlWriter"/> class.
        /// </summary>
        /// <param name="textWriter">The text writer.</param>
        /// <param name="settings">The writer settings.</param>
        public OpenApiYamlWriter(TextWriter textWriter, OpenApiSerializerSettings settings)
            : base(textWriter, settings)
        {
        }

        /// <summary>
        /// Base Indentation Level.
        /// This denotes how many indentations are needed for the property in the base object.
        /// </summary>
        protected override int BaseIndentation => 0;

        /// <summary>
        /// Write YAML start object.
        /// </summary>
        public override void WriteStartObject()
        {
            var previousScope = CurrentScope();

            var currentScope = StartScope(ScopeType.Object);

            if (previousScope != null && previousScope.Type == ScopeType.Array)
            {
                currentScope.IsInArray = true;

                Writer.WriteLine();

                WriteIndentation();

                Writer.Write(WriterConstants.PrefixOfArrayItem);
            }

            IncreaseIndentation();
        }

        /// <summary>
        /// Write YAML end object.
        /// </summary>
        public override void WriteEndObject()
        {
            var previousScope = EndScope(ScopeType.Object);
            DecreaseIndentation();

            var currentScope = CurrentScope();

            // If the object is empty, indicate it by writing { }
            if (previousScope.ObjectCount == 0)
            {
                // If we are in an object, write a white space preceding the braces.
                if (currentScope != null && currentScope.Type == ScopeType.Object)
                {
                    Writer.Write(" ");
                }

                Writer.Write(WriterConstants.EmptyObject);
            }
        }

        /// <summary>
        /// Write YAML start array.
        /// </summary>
        public override void WriteStartArray()
        {
            var previousScope = CurrentScope();

            var currentScope = StartScope(ScopeType.Array);

            if (previousScope != null && previousScope.Type == ScopeType.Array)
            {
                currentScope.IsInArray = true;

                Writer.WriteLine();

                WriteIndentation();

                Writer.Write(WriterConstants.PrefixOfArrayItem);
            }

            IncreaseIndentation();
        }

        /// <summary>
        /// Write YAML end array.
        /// </summary>
        public override void WriteEndArray()
        {
            var previousScope = EndScope(ScopeType.Array);
            DecreaseIndentation();

            var currentScope = CurrentScope();

            // If the array is empty, indicate it by writing [ ]
            if (previousScope.ObjectCount == 0)
            {
                // If we are in an object, write a white space preceding the braces.
                if (currentScope != null && currentScope.Type == ScopeType.Object)
                {
                    Writer.Write(" ");
                }

                Writer.Write(WriterConstants.EmptyArray);
            }
        }

        /// <summary>
        /// Write the property name and the delimiter.
        /// </summary>
        public override void WritePropertyName(string name)
        {
            VerifyCanWritePropertyName(name);

            var currentScope = CurrentScope();

            // If this is NOT the first property in the object, always start a new line and add indentation.
            if (currentScope.ObjectCount != 0)
            {
                Writer.WriteLine();
                WriteIndentation();
            }
            // Only add newline and indentation when this object is not in the top level scope and not in an array.
            // The top level scope should have no indentation and it is already in its own line.
            // The first property of an object inside array can go after the array prefix (-) directly.
            else if (!IsTopLevelScope() && !currentScope.IsInArray)
            {
                Writer.WriteLine();
                WriteIndentation();
            }

            Writer.Write(name);
            Writer.Write(":");

            currentScope.ObjectCount++;
        }

        /// <summary>
        /// Write string value.
        /// </summary>
        /// <param name="value">The string value.</param>
        public override void WriteValue(string value)
        {
            WriteValueSeparator();

            // If string is an empty string, wrap it in quote to ensure it is not recognized as null.
            if (value == "")
            {
                Writer.Write("''");
                return;
            }

            // If string is the word null, wrap it in quote to ensure it is not recognized as empty scalar null.
            if (value == "null")
            {
                Writer.Write("'null'");
                return;
            }

            // If string is the letter ~, wrap it in quote to ensure it is not recognized as empty scalar null.
            if (value == "~")
            {
                Writer.Write("'~'");
                return;
            }

            var specialCharacters = new[]
            {
                ':',
                '{',
                '}',
                '[',
                ']',
                ',',
                '&',
                '*',
                '#',
                '?',
                '|',
                '-',
                '<',
                '>',
                '=',
                '!',
                '%',
                '@',
                '`',
                '\'',
                '"',
                '\\'
            };

            var controlCharacters = new[]
            {
                '\0',
                '\x01',
                '\x02',
                '\x03',
                '\x04',
                '\x05',
                '\x06',
                '\a',
                '\b',
                '\t',
                '\n',
                '\v',
                '\f',
                '\r',
                '\x0e',
                '\x0f',
                '\x10',
                '\x11',
                '\x12',
                '\x13',
                '\x14',
                '\x15',
                '\x16',
                '\x17',
                '\x18',
                '\x19',
                '\x1a',
                '\x1b',
                '\x1c',
                '\x1d',
                '\x1e',
                '\x1f'
            };

            // If string includes a control character, wrapping in double quote is required.
            if (value.Any(c => controlCharacters.Contains(c)))
            {
                // Replace the backslash first, so that the new backslashes created by other Replaces are not duplicated.
                value = value.Replace("\\", "\\\\");

                // Escape the double quotes.
                value = value.Replace("\"", "\\\"");

                // Escape all the control characters.
                value = value.Replace("\0", "\\0");
                value = value.Replace("\x01", "\\x01");
                value = value.Replace("\x02", "\\x02");
                value = value.Replace("\x03", "\\x03");
                value = value.Replace("\x04", "\\x04");
                value = value.Replace("\x05", "\\x05");
                value = value.Replace("\x06", "\\x06");
                value = value.Replace("\a", "\\a");
                value = value.Replace("\b", "\\b");
                value = value.Replace("\t", "\\t");
                value = value.Replace("\n", "\\n");
                value = value.Replace("\v", "\\v");
                value = value.Replace("\f", "\\f");
                value = value.Replace("\r", "\\r");
                value = value.Replace("\x0e", "\\x0e");
                value = value.Replace("\x0f", "\\x0f");
                value = value.Replace("\x10", "\\x10");
                value = value.Replace("\x11", "\\x11");
                value = value.Replace("\x12", "\\x12");
                value = value.Replace("\x13", "\\x13");
                value = value.Replace("\x14", "\\x14");
                value = value.Replace("\x15", "\\x15");
                value = value.Replace("\x16", "\\x16");
                value = value.Replace("\x17", "\\x17");
                value = value.Replace("\x18", "\\x18");
                value = value.Replace("\x19", "\\x19");
                value = value.Replace("\x1a", "\\x1a");
                value = value.Replace("\x1b", "\\x1b");
                value = value.Replace("\x1c", "\\x1c");
                value = value.Replace("\x1d", "\\x1d");
                value = value.Replace("\x1e", "\\x1e");
                value = value.Replace("\x1f", "\\x1f");

                Writer.Write($"\"{value}\"");
                return;
            }

            // If string includes a special character, wrap it in single quote.
            if (value.Any(c => specialCharacters.Contains(c)))
            {
                // Escape single quotes with two single quotes.
                value = value.Replace("'", "''");

                Writer.Write($"'{value}'");
                return;
            }

            // If string can be mistaken as a number or a boolean, wrap it in quote to indicate that this is
            // indeed a string, not a number of a boolean.
            if (decimal.TryParse(value, out var _) || bool.TryParse(value, out var _))
            {
                Writer.Write($"'{value}'");
                return;
            }

            Writer.Write(value);
        }

        /// <summary>
        /// Write null value.
        /// </summary>
        public override void WriteNull()
        {
            // YAML allows null value to be represented by either nothing or the word null.
            // We will write nothing here.
            WriteValueSeparator();
        }

        /// <summary>
        /// Write value separator.
        /// </summary>
        protected override void WriteValueSeparator()
        {
            if (IsArrayScope())
            {
                // If array is the outermost scope and this is the first item, there is no need to insert a newline.
                if (!IsTopLevelScope() || CurrentScope().ObjectCount != 0)
                {
                    Writer.WriteLine();
                }

                WriteIndentation();
                Writer.Write(WriterConstants.PrefixOfArrayItem);

                CurrentScope().ObjectCount++;
            }
            else
            {
                Writer.Write(" ");
            }
        }

        /// <summary>
        /// Writes the content raw value.
        /// </summary>
        public override void WriteRaw(string value)
        {
            WriteValueSeparator();
            Writer.Write(value);
        }
    }
}