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

            IncreaseIndentation();

            if (previousScope != null && previousScope.Type == ScopeType.Array)
            {
                currentScope.IsInArray = true;
            }
        }

        /// <summary>
        /// Write YAML end object.
        /// </summary>
        public override void WriteEndObject()
        {
            var currentScope = EndScope(ScopeType.Object);
            DecreaseIndentation();
        }

        /// <summary>
        /// Write YAML start array.
        /// </summary>
        public override void WriteStartArray()
        {
            StartScope(ScopeType.Array);
            IncreaseIndentation();
        }

        /// <summary>
        /// Write YAML end array.
        /// </summary>
        public override void WriteEndArray()
        {
            var current = EndScope(ScopeType.Array);
            DecreaseIndentation();
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
                if (current.IsInArray)
                {
                    Writer.WriteLine();

                    WritePrefixIndentation();

                    Writer.Write(WriterConstants.PrefixOfArrayItem);
                }
                else
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
            // writer.Write(WriterConstants.NameValueSeparator);
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

            if (value.StartsWith("#"))
            {
                value = "'" + value + "'";
            }

            Writer.Write(value);
        }

        /// <summary>
        /// the empty scalar as “null”.
        /// </summary>
        public override void WriteNull()
        {
            WriteValueSeparator();
            // nothing here
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