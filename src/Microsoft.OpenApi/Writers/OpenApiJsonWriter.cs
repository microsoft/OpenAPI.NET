﻿//---------------------------------------------------------------------
// <copyright file="OpenApiJsonWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------


namespace Microsoft.OpenApi.Writers
{
    using System.IO;

    /// <summary>
    /// JSON Writer.
    /// </summary>
    public class OpenApiJsonWriter : OpenApiWriterBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenApiJsonWriter"/> class.
        /// </summary>
        /// <param name="textWriter">The text writer.</param>
        public OpenApiJsonWriter(TextWriter textWriter)
            : this(textWriter, new OpenApiSerializerSettings())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenApiJsonWriter"/> class.
        /// </summary>
        /// <param name="textWriter">The text writer.</param>
        /// <param name="settings">The writer settings.</param>
        public OpenApiJsonWriter(TextWriter textWriter, OpenApiSerializerSettings settings)
            : base(textWriter, settings)
        {
        }

        /// <summary>
        /// Write JSON start object.
        /// </summary>
        public override void WriteStartObject()
        {
            Scope preScope = CurrentScope();

            StartScope(ScopeType.Object);

            if (preScope != null && preScope.Type == ScopeType.Array)
            {
                Writer.WriteLine();
                WriteIndentation();
            }

            Writer.Write(WriterConstants.StartObjectScope);
            IncreaseIndentation();
        }

        /// <summary>
        /// Write JSOn end object.
        /// </summary>
        public override void WriteEndObject()
        {
            Scope current = EndScope(ScopeType.Object);
            if (current.ObjectCount != 0)
            {
                Writer.WriteLine();
                DecreaseIndentation();
                WriteIndentation();
            }
            else
            {
                Writer.Write(WriterConstants.WhiteSpaceForEmptyObjectArray);
                DecreaseIndentation();
            }

            Writer.Write(WriterConstants.EndObjectScope);
        }

        /// <summary>
        /// Write JSON start array.
        /// </summary>
        public override void WriteStartArray()
        {
            StartScope(ScopeType.Array);
            this.Writer.Write(WriterConstants.StartArrayScope);
            IncreaseIndentation();
        }

        /// <summary>
        /// Write JSON end array.
        /// </summary>
        public override void WriteEndArray()
        {
            Scope current = EndScope(ScopeType.Array);
            if (current.ObjectCount != 0)
            {
                Writer.WriteLine();
                DecreaseIndentation();
                WriteIndentation();
            }
            else
            {
                Writer.Write(WriterConstants.WhiteSpaceForEmptyObjectArray);
                DecreaseIndentation();
            }

            Writer.Write(WriterConstants.EndArrayScope);
        }

        /// <summary>
        /// Write property name.
        /// </summary>
        /// <param name="name">The property name.</param>
        /// public override void WritePropertyName(string name)
        public override void WritePropertyName(string name)
        {
            ValifyCanWritePropertyName(name);

            Scope currentScope = CurrentScope();
            if (currentScope.ObjectCount != 0)
            {
                Writer.Write(WriterConstants.ObjectMemberSeparator);
            }
            Writer.WriteLine();

            currentScope.ObjectCount++;

            // JsonValueUtils.WriteEscapedJsonString(this.writer, name);
            WriteIndentation();

            Writer.Write(WriterConstants.QuoteCharacter);
            Writer.Write(name);
            Writer.Write(WriterConstants.QuoteCharacter);
            Writer.Write(WriterConstants.NameValueSeparator);
        }

        /// <summary>
        /// Write string value.
        /// </summary>
        /// <param name="value">The string value.</param>
        public override void WriteValue(string value)
        {
            WriteValueSeparator();

            value = value.Replace("\n", "\\n");

            Writer.Write(WriterConstants.QuoteCharacter);
            Writer.Write(value);
            Writer.Write(WriterConstants.QuoteCharacter);
        }

        public override void WriteNull()
        {
            Writer.Write("null");
        }

        /// <summary>
        /// Writes a separator of a value if it's needed for the next value to be written.
        /// </summary>
        protected override void WriteValueSeparator()
        {
            if (scopes.Count == 0)
            {
                return;
            }

            Scope currentScope = this.scopes.Peek();
            if (currentScope.Type == ScopeType.Array)
            {
                if (currentScope.ObjectCount != 0)
                {
                    Writer.Write(WriterConstants.ArrayElementSeparator);
                }

                Writer.WriteLine();
                WriteIndentation();
                currentScope.ObjectCount++;
            }
        }

        public override void WriteRaw(string value)
        {
            WriteValue(value); //TODO: fake it for the moment.
        }
    }
}
