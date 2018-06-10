// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

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
        public OpenApiYamlWriter(TextWriter textWriter) : base(textWriter)
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

            name = name.GetYamlCompatibleString();

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

            value = value.GetYamlCompatibleString();

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