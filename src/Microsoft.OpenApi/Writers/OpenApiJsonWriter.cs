// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.IO;

namespace Microsoft.OpenApi.Writers
{
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
        /// Base Indentation Level. 
        /// This denotes how many indentations are needed for the property in the base object.
        /// </summary>
        protected override int BaseIndentation => 1;

        /// <summary>
        /// Write JSON start object.
        /// </summary>
        public override void WriteStartObject()
        {
            var previousScope = CurrentScope();

            var currentScope = StartScope(ScopeType.Object);

            if (previousScope != null && previousScope.Type == ScopeType.Array)
            {
                currentScope.IsInArray = true;

                if (previousScope.ObjectCount != 1)
                {
                    Writer.Write(WriterConstants.ArrayElementSeparator);
                }

                Writer.WriteLine();
                WriteIndentation();
            }

            Writer.Write(WriterConstants.StartObjectScope);

            IncreaseIndentation();
        }

        /// <summary>
        /// Write JSON end object.
        /// </summary>
        public override void WriteEndObject()
        {
            var currentScope = EndScope(ScopeType.Object);
            if (currentScope.ObjectCount != 0)
            {
                Writer.WriteLine();
                DecreaseIndentation();
                WriteIndentation();
            }
            else
            {
                Writer.Write(WriterConstants.WhiteSpaceForEmptyObject);
                DecreaseIndentation();
            }

            Writer.Write(WriterConstants.EndObjectScope);
        }

        /// <summary>
        /// Write JSON start array.
        /// </summary>
        public override void WriteStartArray()
        {
            var previousScope = CurrentScope();

            var currentScope = StartScope(ScopeType.Array);

            if (previousScope != null && previousScope.Type == ScopeType.Array)
            {
                currentScope.IsInArray = true;

                if (previousScope.ObjectCount != 1)
                {
                    Writer.Write(WriterConstants.ArrayElementSeparator);
                }

                Writer.WriteLine();
                WriteIndentation();
            }

            Writer.Write(WriterConstants.StartArrayScope);
            IncreaseIndentation();
        }

        /// <summary>
        /// Write JSON end array.
        /// </summary>
        public override void WriteEndArray()
        {
            var current = EndScope(ScopeType.Array);
            if (current.ObjectCount != 0)
            {
                Writer.WriteLine();
                DecreaseIndentation();
                WriteIndentation();
            }
            else
            {
                Writer.Write(WriterConstants.WhiteSpaceForEmptyArray);
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
            VerifyCanWritePropertyName(name);

            var currentScope = CurrentScope();
            if (currentScope.ObjectCount != 0)
            {
                Writer.Write(WriterConstants.ObjectMemberSeparator);
            }

            Writer.WriteLine();

            currentScope.ObjectCount++;
            
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

        /// <summary>
        /// Write null value.
        /// </summary>
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

            var currentScope = scopes.Peek();

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