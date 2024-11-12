// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

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
        public OpenApiJsonWriter(TextWriter textWriter) : base(textWriter, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenApiJsonWriter"/> class.
        /// </summary>
        /// <param name="textWriter">The text writer.</param>
        /// <param name="settings">Settings for controlling how the OpenAPI document will be written out.</param>
        public OpenApiJsonWriter(TextWriter textWriter, OpenApiJsonWriterSettings settings) : base(textWriter, settings)
        {
            _produceTerseOutput = settings.Terse;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenApiJsonWriter"/> class.
        /// </summary>
        /// <param name="textWriter">The text writer.</param>
        /// <param name="settings">Settings for controlling how the OpenAPI document will be written out.</param>
        /// <param name="terseOutput"> Setting for allowing the JSON emitted to be in terse format.</param>
        public OpenApiJsonWriter(TextWriter textWriter, OpenApiWriterSettings settings, bool terseOutput = false) : base(textWriter, settings)
        {
            _produceTerseOutput = terseOutput;
        }

        /// <summary>
        /// Indicates whether or not the produced document will be written in a compact or pretty fashion.
        /// </summary>
        private readonly bool _produceTerseOutput = false;

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

            if (previousScope is {Type: ScopeType.Array})
            {
                currentScope.IsInArray = true;

                if (previousScope.ObjectCount != 1)
                {
                    Writer.Write(WriterConstants.ArrayElementSeparator);
                }

                WriteLine();
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
                WriteLine();
                DecreaseIndentation();
                WriteIndentation();
            }
            else
            {
                if (!_produceTerseOutput)
                {
                    Writer.Write(WriterConstants.WhiteSpaceForEmptyObject);
                }
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

            if (previousScope is {Type: ScopeType.Array})
            {
                currentScope.IsInArray = true;

                if (previousScope.ObjectCount != 1)
                {
                    Writer.Write(WriterConstants.ArrayElementSeparator);
                }

                WriteLine();
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
                WriteLine();
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

            WriteLine();

            currentScope.ObjectCount++;

            WriteIndentation();

            name = name.GetJsonCompatibleString();

            Writer.Write(name);

            Writer.Write(WriterConstants.NameValueSeparator);

            if (!_produceTerseOutput)
            {
                Writer.Write(WriterConstants.NameValueSeparatorWhiteSpaceSuffix);
            }
        }

        /// <summary>
        /// Write string value.
        /// </summary>
        /// <param name="value">The string value.</param>
        public override void WriteValue(string value)
        {
            WriteValueSeparator();

            value = value.GetJsonCompatibleString();

            Writer.Write(value);
        }

        /// <summary>
        /// Write null value.
        /// </summary>
        public override void WriteNull()
        {
            WriteValueSeparator();

            Writer.Write("null");
        }

        /// <summary>
        /// Writes a separator of a value if it's needed for the next value to be written.
        /// </summary>
        protected override void WriteValueSeparator()
        {
            if (Scopes.Count == 0)
            {
                return;
            }

            var currentScope = Scopes.Peek();

            if (currentScope.Type == ScopeType.Array)
            {
                if (currentScope.ObjectCount != 0)
                {
                    Writer.Write(WriterConstants.ArrayElementSeparator);
                }

                WriteLine();
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

        /// <summary>
        /// Write the indentation.
        /// </summary>
        public override void WriteIndentation()
        {
            if (_produceTerseOutput)
            {
                return;
            }

            base.WriteIndentation();
        }


        /// <summary>
        /// Writes a line terminator to the text string or stream.
        /// </summary>
        private void WriteLine()
        {
            if (_produceTerseOutput)
            {
                return;
            }

            Writer.WriteLine();
        }
    }
}
