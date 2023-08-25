// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Json.Schema;
using Json.Schema.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;

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

            if (previousScope != null && previousScope.Type == ScopeType.Array)
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

            if (previousScope != null && previousScope.Type == ScopeType.Array)
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
        /// Writes out a JsonSchema object
        /// </summary>
        /// <param name="schema"></param>
        public override void WriteJsonSchema(JsonSchema schema)
        {
            if(schema != null)
            {
                var reference = schema.GetRef();
                if (reference != null)
                {
                    if (Settings.InlineExternalReferences)
                    {
                        FindJsonSchemaRefs.ResolveJsonSchema(schema);
                    }
                    else
                    {
                        this.WriteStartObject();
                        this.WriteProperty(OpenApiConstants.DollarRef, reference.OriginalString);
                        WriteEndObject();
                        return;
                    }
                }

                SerializeAsV3WithoutReference(this, schema);
            }
            //if (_produceTerseOutput)
            //{
            //    WriteRaw(JsonSerializer.Serialize(schema));
            //}
        }               


        public override void WriteJsonSchemaWithoutReference(JsonSchema schema)
        {
            if (_produceTerseOutput)
            {
                WriteRaw(JsonSerializer.Serialize(schema));
            }
            else
            {
                var jsonString = JsonSerializer.Serialize(schema, new JsonSerializerOptions { WriteIndented = true });

                // Split json string into lines
                string[] lines = jsonString.Split(new string[] { "\r\n" }, StringSplitOptions.None);

                for (int i = 0; i < lines.Length; i++)
                {
                    // check for $ref then skip it
                    if (lines[i].Contains("$ref"))
                    {
                        continue;
                    }
                    if (i == 0)
                    {
                        Writer.Write(lines[i]);
                    }
                    else
                    {
                        if (i < lines.Length-1 && lines[i+1].Contains("$ref"))
                        {
                            lines[i] = lines[i].TrimEnd(','); // strip out the leading comma after writing out the preceeding schema property before choosing to ignore the $ref
                        }
                        
                        Writer.WriteLine();
                        WriteIndentation();
                        Writer.Write(lines[i]);
                    }
                }
            }
        }

        /// <summary>
        /// Serialize to OpenAPI V3 document without using reference.
        /// </summary>
        public void SerializeAsV3WithoutReference(IOpenApiWriter writer, JsonSchema schema)
        {
            writer.WriteStartObject();

            // title
            writer.WriteProperty(OpenApiConstants.Title, schema.GetTitle());

            // multipleOf
            writer.WriteProperty(OpenApiConstants.MultipleOf, schema.GetMultipleOf());

            // maximum
            writer.WriteProperty(OpenApiConstants.Maximum, schema.GetMaximum());

            // exclusiveMaximum
            writer.WriteProperty(OpenApiConstants.ExclusiveMaximum, schema.GetExclusiveMaximum());

            // minimum
            writer.WriteProperty(OpenApiConstants.Minimum, schema.GetMinimum());

            // exclusiveMinimum
            writer.WriteProperty(OpenApiConstants.ExclusiveMinimum, schema.GetExclusiveMinimum());

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
            writer.WriteProperty(OpenApiConstants.Type, schema.GetJsonType().ToString().ToLowerInvariant()/*.Value.GetDisplayName()*/);

            // allOf
            writer.WriteOptionalCollection(OpenApiConstants.AllOf, schema.GetAllOf(), (w, s) => w.WriteJsonSchema(s));

            // anyOf
            writer.WriteOptionalCollection(OpenApiConstants.AnyOf, schema.GetAnyOf(), (w, s) => w.WriteJsonSchema(s));

            // oneOf
            writer.WriteOptionalCollection(OpenApiConstants.OneOf, schema.GetOneOf(), (w, s) => w.WriteJsonSchema(s));

            // not
            writer.WriteOptionalObject(OpenApiConstants.Not, schema.GetNot(), (w, s) => w.WriteJsonSchema(s));

            // items
            writer.WriteOptionalObject(OpenApiConstants.Items, schema.GetItems(), (w, s) => w.WriteJsonSchema(s));

            // properties
            writer.WriteOptionalMap(OpenApiConstants.Properties, (IDictionary<string, JsonSchema>)schema.GetProperties(), 
                (w, key, s) =>
                {
                    foreach(var property in schema.GetProperties())
                    {
                       writer.WritePropertyName(property.Key);
                       w.WriteJsonSchema(property.Value);
                    }                   
                });

            // additionalProperties
            //if (schema.GetAdditionalPropertiesAllowed())
            //{
            //    writer.WriteOptionalObject(
            //        OpenApiConstants.AdditionalProperties,
            //        schema.GetAdditionalProperties(),
            //        (w, s) => s.SerializeAsV3(w));
            //}
            //else
            //{
            //    writer.WriteProperty(OpenApiConstants.AdditionalProperties, schema.GetAdditionalPropertiesAllowed());
            //}

            // description
            writer.WriteProperty(OpenApiConstants.Description, schema.GetDescription());

            // format
            writer.WriteProperty(OpenApiConstants.Format, schema.GetFormat()?.Key);

            // default
            writer.WriteOptionalObject(OpenApiConstants.Default, schema.GetDefault(), (w, d) => w.WriteAny(new OpenApiAny(d)));

            // nullable
            //writer.WriteProperty(OpenApiConstants.Nullable, schema.GetNullable(), false);

            // discriminator
            writer.WriteOptionalObject(OpenApiConstants.Discriminator, schema.GetOpenApiDiscriminator(), (w, d) => d.SerializeAsV3(w));

            // readOnly
            writer.WriteProperty(OpenApiConstants.ReadOnly, schema.GetReadOnly(), false);

            // writeOnly
            writer.WriteProperty(OpenApiConstants.WriteOnly, schema.GetWriteOnly(), false);

            // xml
          //  writer.WriteOptionalObject(OpenApiConstants.Xml, schema.GetXml(), (w, s) => s.SerializeAsV2(w));

            // externalDocs
         //   writer.WriteOptionalObject(OpenApiConstants.ExternalDocs, schema.GetExternalDocs(), (w, s) => s.SerializeAsV3(w));

            // example
            writer.WriteOptionalObject(OpenApiConstants.Example, schema.GetExample(), (w, e) => w.WriteAny(new OpenApiAny(e)));

            // deprecated
            writer.WriteProperty(OpenApiConstants.Deprecated, schema.GetDeprecated(), false);

            // extensions
          //  writer.WriteExtensions(schema.GetExtensions(), OpenApiSpecVersion.OpenApi3_0);

            writer.WriteEndObject();
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
