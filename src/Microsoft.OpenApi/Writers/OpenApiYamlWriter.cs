// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.IO;

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

        protected override int IndentShift
        {
            get
            {
                return -1;
            }
        }

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
        /// Write the start property.
        /// </summary>
        public override void WritePropertyName(string name)
        {
            VerifyCanWritePropertyName(name);

            var current = CurrentScope();

            if (current.ObjectCount == 0)
            {
                if (!current.IsInArray)
                {
                    // If this object is the outermost scope, there is no need to insert a newline.
                    if (!IsTopLevelScope())
                    {
                        Writer.WriteLine();
                    }

                    WriteIndentation();
                }
            }
            else
            {
                Writer.WriteLine();
                WriteIndentation();
            }

            Writer.Write(name);
            Writer.Write(":");

            ++current.ObjectCount;
        }

        /// <summary>
        /// Write string value.
        /// </summary>
        /// <param name="value">The string value.</param>
        public override void WriteValue(string value)
        {
            WriteValueSeparator();

            value = value.Replace("\n", "\\n");

            // If string is an empty string, wrap it in quote to ensure it is not recognized as null.
            if (value == "")
            {
                value = "''";
            }

            // If string is the word null, wrap it in quote to ensure it is not recognized as empty scalar null.
            if (value == "null")
            {
                value = "'null'";
            }

            // If string includes special character, wrap it in quote to avoid conflicts.
            if (value.StartsWith("#"))
            {
                value = $"'{value}'";
            }

            // If string can be mistaken as a number or a boolean, wrap it in quote to indicate that this is
            // indeed a string, not a number of a boolean.
            if (decimal.TryParse(value, out var _) || bool.TryParse(value, out var _))
            {
                value = $"'{value}'";
            }

            Writer.Write(value);
        }

        /// <summary>
        /// the empty scalar as “null”.
        /// </summary>
        public override void WriteNull()
        {
            // YAML allows null value to be represented by either nothing or the word null.
            // We will write nothing here.
            WriteValueSeparator();
        }

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

        public override void WriteRaw(string value)
        {
            WriteValue(value); //TODO: fake it for the moment.
        }
    }
}