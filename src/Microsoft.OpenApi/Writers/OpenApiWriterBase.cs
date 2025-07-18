﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// Base class for Open API writer.
    /// </summary>
    public abstract class OpenApiWriterBase : IOpenApiWriter
    {

        /// <summary>
        /// Settings for controlling how the OpenAPI document will be written out.
        /// </summary>
        public OpenApiWriterSettings Settings { get; set; }

        /// <summary>
        /// The indentation string to prepend to each line for each indentation level.
        /// </summary>
        protected const string IndentationString = "  ";

        /// <summary>
        /// Scope of the Open API element - object, array, property.
        /// </summary>
        protected readonly Stack<Scope> Scopes;

        /// <summary>
        /// Number which specifies the level of indentation.
        /// </summary>
        private int _indentLevel;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenApiWriterBase"/> class.
        /// </summary>
        /// <param name="textWriter">The text writer.</param>
        protected OpenApiWriterBase(TextWriter textWriter) : this(textWriter, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenApiWriterBase"/> class.
        /// </summary>
        /// <param name="textWriter"></param>
        /// <param name="settings"></param>
        protected OpenApiWriterBase(TextWriter textWriter, OpenApiWriterSettings? settings)
        {
            Writer = textWriter;
            Writer.NewLine = "\n";

            Scopes = new();
            if (settings == null)
            {
                settings = new();
            }
            Settings = settings;
        }

        /// <summary>
        /// Base Indentation Level.
        /// This denotes how many indentations are needed for the property in the base object.
        /// </summary>
        protected abstract int BaseIndentation { get; }

        /// <summary>
        /// The text writer.
        /// </summary>
        protected TextWriter Writer { get; }

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
        /// Write content raw value.
        /// </summary>
        public abstract void WriteRaw(string value);

        /// <inheritdoc/>
        public Task FlushAsync(CancellationToken cancellationToken = default)
        {
#if NET8_OR_GREATER
            return Writer.FlushAsync(cancellationToken);
#else
            return Writer.FlushAsync();
#endif
        }

        /// <summary>
        /// Write string value.
        /// </summary>
        /// <param name="value">The string value.</param>
        public abstract void WriteValue(string value);

        /// <summary>
        /// Write float value.
        /// </summary>
        /// <param name="value">The float value.</param>
        public virtual void WriteValue(float value)
        {
            WriteValueSeparator();
            Writer.Write(value);
        }

        /// <summary>
        /// Write double value.
        /// </summary>
        /// <param name="value">The double value.</param>
        public virtual void WriteValue(double value)
        {
            WriteValueSeparator();
            Writer.Write(value);
        }

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
        /// Write long value.
        /// </summary>
        /// <param name="value">The long value.</param>
        public virtual void WriteValue(long value)
        {
            WriteValueSeparator();
            Writer.Write(value);
        }

        /// <summary>
        /// Write DateTime value.
        /// </summary>
        /// <param name="value">The DateTime value.</param>
        public virtual void WriteValue(DateTime value)
        {
            this.WriteValue(value.ToString("o", CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Write DateTimeOffset value.
        /// </summary>
        /// <param name="value">The DateTimeOffset value.</param>
        public virtual void WriteValue(DateTimeOffset value)
        {
            this.WriteValue(value.ToString("o", CultureInfo.InvariantCulture));
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
        /// Writes an enumerable collection as an array
        /// </summary>
        /// <param name="collection">The enumerable collection to write.</param>
        /// <typeparam name="T">The type of elements in the collection.</typeparam>
        public virtual void WriteEnumerable<T>(IEnumerable<T> collection)
        {
            WriteStartArray();
            foreach (var item in collection)
            {
                WriteValue(item);
            }
            WriteEndArray();
        }

        /// <summary>
        /// Write object value.
        /// </summary>
        /// <param name="value">The object value.</param>
        public virtual void WriteValue(object? value)
        {
            if (value == null)
            {
                WriteNull();
                return;
            }

            if (value is string strValue)
            {
                WriteValue(strValue);
            }
            else if (value is int intValue)
            {
                WriteValue(intValue);
            }
            else if (value is uint uintValue)
            {
                WriteValue(uintValue);
            }
            else if (value is long longValue)
            {
                WriteValue(longValue);
            }
            else if (value is bool boolValue)
            {
                WriteValue(boolValue);
            }
            else if (value is float floatValue)
            {
                WriteValue(floatValue);
            }
            else if (value is double doubleValue)
            {
                WriteValue(doubleValue);
            }
            else if (value is decimal decimalValue)
            {
                WriteValue(decimalValue);
            }
            else if (value is DateTime DateTimeValue)
            {
                WriteValue(DateTimeValue);
            }
            else if (value is DateTimeOffset DateTimeOffsetValue)
            {
                WriteValue(DateTimeOffsetValue);
            }
            else if (value is IEnumerable<object> enumerable)
            {
                WriteEnumerable(enumerable);
            }
            else
            {
                throw new OpenApiWriterException(string.Format(SRResource.OpenApiUnsupportedValueType, value.GetType().FullName));
            }
        }

        /// <summary>
        /// Increases the level of indentation applied to the output.
        /// </summary>
        public virtual void IncreaseIndentation()
        {
            _indentLevel++;
        }

        /// <summary>
        /// Decreases the level of indentation applied to the output.
        /// </summary>
        public virtual void DecreaseIndentation()
        {
            if (_indentLevel == 0)
            {
                throw new OpenApiWriterException(SRResource.IndentationLevelInvalid);
            }

            if (_indentLevel < 1)
            {
                _indentLevel = 0;
            }
            else
            {
                _indentLevel--;
            }
        }

        /// <summary>
        /// Write the indentation.
        /// </summary>
        public virtual void WriteIndentation()
        {
            for (var i = 0; i < (BaseIndentation + _indentLevel - 1); i++)
            {
                Writer.Write(IndentationString);
            }
        }
        
        /// <summary>
        /// Get current scope.
        /// </summary>
        /// <returns></returns>
        protected Scope? CurrentScope()
        {
            return Scopes.Count == 0 ? null : Scopes.Peek();
        }

        /// <summary>
        /// Start the scope given the scope type.
        /// </summary>
        /// <param name="type">The scope type to start.</param>
        protected Scope StartScope(ScopeType type)
        {
            if (Scopes.Count != 0)
            {
                var currentScope = Scopes.Peek();

                currentScope.ObjectCount++;
            }

            var scope = new Scope(type);
            Scopes.Push(scope);
            return scope;
        }

        /// <summary>
        /// End the scope of the given scope type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected Scope EndScope(ScopeType type)
        {
            if (Scopes.Count == 0)
            {
                throw new OpenApiWriterException(SRResource.ScopeMustBePresentToEnd);
            }

            if (Scopes.Peek().Type != type)
            {
                throw new OpenApiWriterException(
                    string.Format(
                        SRResource.ScopeToEndHasIncorrectType,
                        type,
                        Scopes.Peek().Type));
            }

            return Scopes.Pop();
        }

        /// <summary>
        /// Whether the current scope is the top level (outermost) scope.
        /// </summary>
        protected bool IsTopLevelScope()
        {
            return Scopes.Count == 1;
        }

        /// <summary>
        /// Whether the current scope is an object scope.
        /// </summary>
        protected bool IsObjectScope()
        {
            return IsScopeType(ScopeType.Object);
        }

        /// <summary>
        /// Whether the current scope is an array scope.
        /// </summary>
        /// <returns></returns>
        protected bool IsArrayScope()
        {
            return IsScopeType(ScopeType.Array);
        }

        private bool IsScopeType(ScopeType type)
        {
            if (Scopes.Count == 0)
            {
                return false;
            }

            return Scopes.Peek().Type == type;
        }

        /// <summary>
        /// Verifies whether a property name can be written based on whether
        /// the property name is a valid string and whether the current scope is an object scope.
        /// </summary>
        /// <param name="name">property name</param>
        protected void VerifyCanWritePropertyName(string name)
        {
            Utils.CheckArgumentNull(name);

            if (Scopes.Count == 0)
            {
                throw new OpenApiWriterException(
                    string.Format(SRResource.ActiveScopeNeededForPropertyNameWriting, name));
            }

            if (Scopes.Peek().Type != ScopeType.Object)
            {
                throw new OpenApiWriterException(
                    string.Format(SRResource.ObjectScopeNeededForPropertyNameWriting, name));
            }
        }

        /// <inheritdoc/>
        public static void WriteV2Examples(IOpenApiWriter writer, OpenApiExample example, OpenApiSpecVersion version)
        {
            writer.WriteStartObject();

            // summary
            writer.WriteProperty(OpenApiConstants.Summary, example.Summary);

            // description
            writer.WriteProperty(OpenApiConstants.Description, example.Description);

            // value
            writer.WriteOptionalObject(OpenApiConstants.Value, example.Value, (w, v) => w.WriteAny(v));

            // externalValue
            writer.WriteProperty(OpenApiConstants.ExternalValue, example.ExternalValue);

            // extensions
            writer.WriteExtensions(example.Extensions, version);

            writer.WriteEndObject();
        }
    }
}
