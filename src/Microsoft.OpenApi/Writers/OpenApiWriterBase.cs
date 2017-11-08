// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.OpenApi.Properties;

namespace Microsoft.OpenApi.Writers
{
    /// <summary>
    /// Base class for Open API writer.
    /// </summary>
    public abstract class OpenApiWriterBase : IOpenApiWriter
    {
        /// <summary>
        /// The indentation string to prepand to each line for each indentation level.
        /// </summary>
        private const string IndentationString = "  ";

        /// <summary>
        /// Number which specifies the level of indentation. Starts with 0 which means no indentation.
        /// </summary>
        private int indentLevel;

        /// <summary>
        /// Scope of the Open API element - object, array, property.
        /// </summary>
        protected readonly Stack<Scope> scopes;

        /// <summary>
        /// Number which specifies the level of indentation. Starts with 0 which means no indentation.
        /// </summary>
        private OpenApiSerializerSettings settings;

        /// <summary>
        /// Indentent shift value.
        /// </summary>
        protected virtual int IndentShift { get { return 0; } }

        /// <summary>
        /// The text writer.
        /// </summary>
        protected TextWriter Writer { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenApiWriterBase"/> class.
        /// </summary>
        /// <param name="textWriter">The text writer.</param>
        /// <param name="settings">The writer settings.</param>
        public OpenApiWriterBase(TextWriter textWriter, OpenApiSerializerSettings settings)
        {
            Writer = textWriter;
            Writer.NewLine = "\n";

            this.scopes = new Stack<Scope>();
            this.settings = settings;
        }

        /// <summary>
        /// Write start object.
        /// </summary>
        public abstract void WriteStartObject();

        /// <summary>
        /// Write end object.
        /// </summary>
        public abstract void WriteEndObject();

        /// <summary>
        /// Write start array.
        /// </summary>
        public abstract void WriteStartArray();

        /// <summary>
        /// Write end array.
        /// </summary>
        public abstract void WriteEndArray();

        /// <summary>
        /// Write the start property.
        /// </summary>
        public abstract void WritePropertyName(string name);

        /// <summary>
        /// Writes a separator of a value if it's needed for the next value to be written.
        /// </summary>
        protected abstract void WriteValueSeparator();

        /// <summary>
        /// Write null value.
        /// </summary>
        public abstract void WriteNull();

        /// <summary>
        /// Write raw content value for examples
        /// </summary>
        public abstract void WriteRaw(string value);


        /// <summary>
        /// Flush the writer.
        /// </summary>
        public void Flush()
        {
            this.Writer.Flush();
        }

        /// <summary>
        /// Write string value.
        /// </summary>
        /// <param name="value">The string value.</param>
        public abstract void WriteValue(string value);

        /// <summary>
        /// Write decimal value.
        /// </summary>
        /// <param name="value">The decimal value.</param>
        public virtual void WriteValue(decimal value)
        {
            WriteValueSeparator();
            Writer.Write(value);
        }

        /// <summary>
        /// Write integer value.
        /// </summary>
        /// <param name="value">The integer value.</param>
        public virtual void WriteValue(int value)
        {
            WriteValueSeparator();
            Writer.Write(value);
        }

        /// <summary>
        /// Write boolean value.
        /// </summary>
        /// <param name="value">The boolean value.</param>
        public virtual void WriteValue(bool value)
        {
            WriteValueSeparator();
            Writer.Write(value.ToString().ToLower());
        }

        /// <summary>
        /// Write object value.
        /// </summary>
        /// <param name="value">The object value.</param>
        public virtual void WriteValue(object value)
        {
            if (value == null)
            {
                WriteNull();
                return;
            }

            Type type = value.GetType();

            if (type == typeof(string))
            {
                WriteValue((string)(value));
            }
            else if (type == typeof(int) || type == typeof(int?))
            {
                WriteValue((int)value);
            }
            else if (type == typeof(bool) || type == typeof(bool?))
            {
                WriteValue((bool)value);
            }
            else if (type == typeof(decimal) || type == typeof(decimal?))
            {
                WriteValue((decimal)value);
            }
            else
            {
                throw new OpenApiException(String.Format(SRResource.OpenApiUnsupportedValueType, type.FullName));
            }
        }

        /// <summary>
        /// Increases the level of indentation applied to the output.
        /// </summary>
        public virtual void IncreaseIndentation()
        {
            indentLevel++;
        }

        /// <summary>
        /// Decreases the level of indentation applied to the output.
        /// </summary>
        public virtual void DecreaseIndentation()
        {
            Debug.Assert(indentLevel > 0, "Trying to decrease indentation below zero.");
            if (indentLevel < 1)
            {
                indentLevel = 0;
            }
            else
            {
                indentLevel--;
            }
        }

        /// <summary>
        /// Write the indentation.
        /// </summary>
        public virtual void WriteIndentation()
        {
            for (int i = 0; i < (indentLevel + IndentShift); i++)
            {
                Writer.Write(IndentationString);
            }
        }

        public virtual void WritePrefixIndentation()
        {
            for (int i = 0; i < (indentLevel + IndentShift - 1); i++)
            {
                Writer.Write(IndentationString);
            }
        }

        /// <summary>
        /// Get current scope.
        /// </summary>
        /// <returns></returns>
        protected Scope CurrentScope()
        {
            return scopes.Count == 0 ? null : scopes.Peek();
        }

        /// <summary>
        /// Start the scope given the scope type.
        /// </summary>
        /// <param name="type">The scope type to start.</param>
        protected Scope StartScope(ScopeType type)
        {
            if (scopes.Count != 0)
            {
                Scope currentScope = this.scopes.Peek();
                if ((currentScope.Type == ScopeType.Array) &&
                    (currentScope.ObjectCount != 0))
                {
                    Writer.Write(WriterConstants.ArrayElementSeparator);
                }

                currentScope.ObjectCount++;
            }

            Scope scope = new Scope(type);
            this.scopes.Push(scope);
            return scope;
        }

        protected Scope EndScope(ScopeType type)
        {
            Debug.Assert(scopes.Count > 0, "No scope to end.");
            Debug.Assert(scopes.Peek().Type == type, "Ending scope does not match.");
            return scopes.Pop();
        }

        protected bool IsTopLevelScope()
        {
            return scopes.Count == 1;
        }

        protected bool IsObjectScope()
        {
            return IsScopeType(ScopeType.Object);
        }

        protected bool IsArrayScope()
        {
            return IsScopeType(ScopeType.Array);
        }

        private bool IsScopeType(ScopeType type)
        {
            if (scopes.Count == 0)
            {
                return false;
            }

            return scopes.Peek().Type == type;
        }

        protected void VerifyCanWritePropertyName(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                throw Error.ArgumentNullOrWhiteSpace(nameof(name));
            }

            if (this.scopes.Count == 0)
            {
                throw new OpenApiException(String.Format(SRResource.OpenApiWriterMustHaveActiveScope, name));
            }

            if (this.scopes.Peek().Type != ScopeType.Object)
            {
                throw new OpenApiException(String.Format(SRResource.OpenApiWriterMustBeObjectScope, name));
            }
        }
    }
}
