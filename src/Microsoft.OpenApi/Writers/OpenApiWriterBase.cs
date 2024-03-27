// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using Json.Schema;
using Json.Schema.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Properties;
using Microsoft.OpenApi.Services;

namespace Microsoft.OpenApi.Writers
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
        public OpenApiWriterBase(TextWriter textWriter) : this(textWriter, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenApiWriterBase"/> class.
        /// </summary>
        /// <param name="textWriter"></param>
        /// <param name="settings"></param>
        public OpenApiWriterBase(TextWriter textWriter, OpenApiWriterSettings settings)
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

        /// <summary>
        /// Flush the writer.
        /// </summary>
        public void Flush()
        {
            Writer.Flush();
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
            this.WriteValue(value.ToString("o"));
        }

        /// <summary>
        /// Write DateTimeOffset value.
        /// </summary>
        /// <param name="value">The DateTimeOffset value.</param>
        public virtual void WriteValue(DateTimeOffset value)
        {
            this.WriteValue(value.ToString("o"));
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

            var type = value.GetType();

            if (type == typeof(string))
            {
                WriteValue((string)(value));
            }
            else if (type == typeof(int) || type == typeof(int?))
            {
                WriteValue((int)value);
            }
            else if (type == typeof(uint) || type == typeof(uint?))
            {
                WriteValue((uint)value);
            }
            else if (type == typeof(long) || type == typeof(long?))
            {
                WriteValue((long)value);
            }
            else if (type == typeof(bool) || type == typeof(bool?))
            {
                WriteValue((bool)value);
            }
            else if (type == typeof(float) || type == typeof(float?))
            {
                WriteValue((float)value);
            }
            else if (type == typeof(double) || type == typeof(double?))
            {
                WriteValue((double)value);
            }
            else if (type == typeof(decimal) || type == typeof(decimal?))
            {
                WriteValue((decimal)value);
            }
            else if (type == typeof(DateTime) || type == typeof(DateTime?))
            {
                WriteValue((DateTime)value);
            }
            else if (type == typeof(DateTimeOffset) || type == typeof(DateTimeOffset?))
            {
                WriteValue((DateTimeOffset)value);
            }
            else
            {
                throw new OpenApiWriterException(string.Format(SRResource.OpenApiUnsupportedValueType, type.FullName));
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
        protected Scope CurrentScope()
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

        /// <summary>
        /// Writes out a JsonSchema object
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="version"></param>
        public void WriteJsonSchema(JsonSchema schema, OpenApiSpecVersion version)
        {
            if (schema == null)
            {
                return;
            }

            var reference = schema.GetRef();
            if (reference != null)
            {
                if (!Settings.ShouldInlineReference())
                {
                    WriteJsonSchemaReference(this, reference, version);
                    return;
                }
                else
                {
                    if (Settings.InlineExternalReferences)
                    {
                        FindJsonSchemaRefs.ResolveJsonSchema(schema);
                    }
                    else if (Settings.InlineLocalReferences)
                    {
                        schema = FindJsonSchemaRefs.FetchSchemaFromRegistry(schema, reference);
                    }
                    if (!Settings.LoopDetector.PushLoop(schema))
                    {
                        Settings.LoopDetector.SaveLoop(schema);
                        WriteJsonSchemaReference(this, reference, version);
                        return;
                    }
                }
            }

            if (schema != null)
            {
                WriteJsonSchemaWithoutReference(this, schema, version);
            }

            if (reference != null)
            {
                Settings.LoopDetector.PopLoop<JsonSchema>();
            }
        }

        /// <inheritdoc />
        public void WriteJsonSchemaWithoutReference(IOpenApiWriter writer, JsonSchema schema, OpenApiSpecVersion version)
        {
            writer.WriteStartObject();

            // title
            writer.WriteProperty(OpenApiConstants.Title, schema.GetTitle());

            // multipleOf
            writer.WriteProperty(OpenApiConstants.MultipleOf, schema.GetMultipleOf());

            // maximum
            writer.WriteProperty(OpenApiConstants.Maximum, schema.GetMaximum());

            // exclusiveMaximum
            writer.WriteProperty(OpenApiConstants.ExclusiveMaximum, schema.GetOpenApiExclusiveMaximum());

            // minimum
            writer.WriteProperty(OpenApiConstants.Minimum, schema.GetMinimum());

            // exclusiveMinimum
            writer.WriteProperty(OpenApiConstants.ExclusiveMinimum, schema.GetOpenApiExclusiveMinimum());

            // maxLength
            writer.WriteProperty(OpenApiConstants.MaxLength, schema.GetMaxLength());

            // minLength
            writer.WriteProperty(OpenApiConstants.MinLength, schema.GetMinLength());

            // pattern
            writer.WriteProperty(OpenApiConstants.Pattern, schema.GetPattern()?.ToString());

            // maxItems
            writer.WriteProperty(OpenApiConstants.MaxItems, schema.GetMaxItems());

            // minItems
            writer.WriteProperty(OpenApiConstants.MinItems, schema.GetMinItems());

            // uniqueItems
            writer.WriteProperty(OpenApiConstants.UniqueItems, schema.GetUniqueItems());

            // maxProperties
            writer.WriteProperty(OpenApiConstants.MaxProperties, schema.GetMaxProperties());

            // minProperties
            writer.WriteProperty(OpenApiConstants.MinProperties, schema.GetMinProperties());

            // required
            writer.WriteOptionalCollection(OpenApiConstants.Required, schema.GetRequired(), (w, s) => w.WriteValue(s));

            // enum
            writer.WriteOptionalCollection(OpenApiConstants.Enum, schema.GetEnum(), (nodeWriter, s) => nodeWriter.WriteAny(new OpenApiAny(s)));

            // type
            writer.WriteProperty(OpenApiConstants.Type, schema.GetJsonType()?.ToString().ToLowerInvariant());

            // allOf
            writer.WriteOptionalCollection(OpenApiConstants.AllOf, schema.GetAllOf(), (w, s) => w.WriteJsonSchema(s, version));

            // anyOf
            writer.WriteOptionalCollection(OpenApiConstants.AnyOf, schema.GetAnyOf(), (w, s) => w.WriteJsonSchema(s, version));

            // oneOf
            writer.WriteOptionalCollection(OpenApiConstants.OneOf, schema.GetOneOf(), (w, s) => w.WriteJsonSchema(s, version));

            // not
            writer.WriteOptionalObject(OpenApiConstants.Not, schema.GetNot(), (w, s) => w.WriteJsonSchema(s, version));

            // items
            writer.WriteOptionalObject(OpenApiConstants.Items, schema.GetItems(), (w, s) => w.WriteJsonSchema(s, version));

            // properties
            writer.WriteOptionalMap(OpenApiConstants.Properties, (IDictionary<string, JsonSchema>)schema.GetProperties(),
                (w, key, s) => w.WriteJsonSchema(s, version));

            // pattern properties
            var patternProperties = schema?.GetPatternProperties();
            var stringPatternProperties = patternProperties?.ToDictionary(
                kvp => kvp.Key.ToString(),  // Convert Regex key to string
                kvp => kvp.Value
            );

            writer.WriteOptionalMap(OpenApiConstants.PatternProperties, stringPatternProperties,
                (w, key, s) => w.WriteJsonSchema(s, version));

            // additionalProperties
            if (schema.GetAdditionalPropertiesAllowed() ?? false)
            {
                writer.WriteOptionalObject(
                    OpenApiConstants.AdditionalProperties,
                    schema.GetAdditionalProperties(),
                    (w, s) => w.WriteJsonSchema(s, version));
            }
            else
            {
                writer.WriteProperty(OpenApiConstants.AdditionalProperties, schema.GetAdditionalPropertiesAllowed());
            }

            // description
            writer.WriteProperty(OpenApiConstants.Description, schema.GetDescription());

            // format
            writer.WriteProperty(OpenApiConstants.Format, schema.GetFormat()?.Key);

            // default
            writer.WriteOptionalObject(OpenApiConstants.Default, schema.GetDefault(), (w, d) => w.WriteAny(new OpenApiAny(d)));

            // nullable
            writer.WriteProperty(OpenApiConstants.Nullable, schema.GetNullable(), false);

            // discriminator
            writer.WriteOptionalObject(OpenApiConstants.Discriminator, schema.GetOpenApiDiscriminator(), (w, d) => d.SerializeAsV3(w));

            // readOnly
            writer.WriteProperty(OpenApiConstants.ReadOnly, schema.GetReadOnly(), false);

            // writeOnly
            writer.WriteProperty(OpenApiConstants.WriteOnly, schema.GetWriteOnly(), false);

            // xml
            writer.WriteOptionalObject(OpenApiConstants.Xml, schema.GetXml(), (w, s) => JsonSerializer.Serialize(s));

            // externalDocs
            writer.WriteOptionalObject(OpenApiConstants.ExternalDocs, schema.GetExternalDocs(), (w, s) => JsonSerializer.Serialize(s));

            // example
            writer.WriteOptionalObject(OpenApiConstants.Example, schema.GetExample(), (w, s) => w.WriteAny(new OpenApiAny(s)));

            // examples
            writer.WriteOptionalCollection(OpenApiConstants.Examples, schema.GetExamples(), (n, e) => n.WriteAny(new OpenApiAny(e)));

            // deprecated
            writer.WriteProperty(OpenApiConstants.Deprecated, schema.GetDeprecated(), false);

            // extensions
            writer.WriteExtensions(schema.GetExtensions(), OpenApiSpecVersion.OpenApi3_0);

            writer.WriteEndObject();
        }

        /// <inheritdoc />
        public void WriteJsonSchemaReference(IOpenApiWriter writer, Uri reference, OpenApiSpecVersion version)
        {
            var referenceItem = version.Equals(OpenApiSpecVersion.OpenApi2_0)
                ? reference.OriginalString.Replace("components/schemas", "definitions")
                : reference.OriginalString;

            WriteStartObject();
            this.WriteProperty(OpenApiConstants.DollarRef, referenceItem);
            WriteEndObject();
        }

        /// <inheritdoc/>
        public void WriteV2Examples(IOpenApiWriter writer, OpenApiExample example, OpenApiSpecVersion version)
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

    internal class FindJsonSchemaRefs : OpenApiVisitorBase
    {
        public static void ResolveJsonSchema(JsonSchema schema)
        {
            var visitor = new FindJsonSchemaRefs();
            var walker = new OpenApiWalker(visitor);
            walker.Walk(schema);
        }

        public static JsonSchema FetchSchemaFromRegistry(JsonSchema schema, Uri reference)
        {
            var referencePath = string.Concat("https://registry", reference.OriginalString.Split('#').Last());
            var resolvedSchema = (JsonSchema)SchemaRegistry.Global.Get(new Uri(referencePath));
            return resolvedSchema ?? schema;
        }
    }
}
