// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Json.More;
using Json.Schema;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Extensions
{
    /// <summary>
    /// Provides extension methods for JSON schema generation
    /// </summary>
    public static class JsonSchemaBuilderExtensions
    {
        /// <summary>
        /// Custom extensions in the schema
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="extensions"></param>
        /// <returns></returns>
        public static JsonSchemaBuilder Extensions(this JsonSchemaBuilder builder, IDictionary<string, IOpenApiExtension> extensions)
        {
            builder.Add(new ExtensionsKeyword(extensions));
            return builder;
        }

        /// <summary>
        /// The Schema summary
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="summary"></param>
        /// <returns></returns>
        public static JsonSchemaBuilder Summary(this JsonSchemaBuilder builder, string summary)
        {
            builder.Add(new SummaryKeyword(summary));
            return builder;
        }

        /// <summary>
        /// Indicates if the schema can contain properties other than those defined by the properties map
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="additionalPropertiesAllowed"></param>
        /// <returns></returns>
        public static JsonSchemaBuilder AdditionalPropertiesAllowed(this JsonSchemaBuilder builder, bool additionalPropertiesAllowed)
        {
            builder.Add(new AdditionalPropertiesAllowedKeyword(additionalPropertiesAllowed));
            return builder;
        }

        /// <summary>
        /// Allows sending a null value for the defined schema. Default value is false.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static JsonSchemaBuilder Nullable(this JsonSchemaBuilder builder, bool value)
        {
            builder.Add(new NullableKeyword(value));
            return builder;
        }

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static JsonSchemaBuilder ExclusiveMaximum(this JsonSchemaBuilder builder, bool value)
        {
            builder.Add(new Draft4ExclusiveMaximumKeyword(value));
            return builder;
        }

        /// <summary>
        /// Follow JSON Schema definition: https://tools.ietf.org/html/draft-fge-json-schema-validation-00
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static JsonSchemaBuilder ExclusiveMinimum(this JsonSchemaBuilder builder, bool value)
        {
            builder.Add(new Draft4ExclusiveMinimumKeyword(value));
            return builder;
        }

        /// <summary>
        /// Adds support for polymorphism. The discriminator is an object name that is used to differentiate
        /// between other schemas which may satisfy the payload description.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="discriminator"></param>
        /// <returns></returns>
        public static JsonSchemaBuilder Discriminator(this JsonSchemaBuilder builder, OpenApiDiscriminator discriminator)
        {
            builder.Add(new DiscriminatorKeyword(discriminator));
            return builder;
        }

        /// <summary>
        /// ExternalDocs object.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="externalDocs"></param>
        /// <returns></returns>
        public static JsonSchemaBuilder OpenApiExternalDocs(this JsonSchemaBuilder builder, OpenApiExternalDocs externalDocs)
        {
            builder.Add(new ExternalDocsKeyword(externalDocs));
            return builder;
        }

        /// <summary>
        /// Removes a keyword
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="keyword"></param>
        public static JsonSchemaBuilder Remove(this JsonSchemaBuilder builder, string keyword)
        {
            var keywords = builder.Build().Keywords;
            keywords = keywords.Where(x => !x.Keyword().Equals(keyword)).ToList();
            var schemaBuilder = new JsonSchemaBuilder();
            if (keywords.Count == 0)
            {
                return schemaBuilder;
            }
            else
            {
                foreach (var item in keywords)
                {
                    schemaBuilder.Add(item);
                }
            }

            return schemaBuilder;
        }
    }

    /// <summary>
    /// The Exclusive minimum keyword as defined in JSON schema Draft4
    /// </summary>
    [SchemaKeyword(Name)]
    public class Draft4ExclusiveMinimumKeyword : IJsonSchemaKeyword
    {
        /// <summary>
        /// The schema keyword name
        /// </summary>
        public const string Name = "exclusiveMinimum";

        /// <summary>
        /// The ID.
        /// </summary>
        public bool MinValue { get; }

        internal Draft4ExclusiveMinimumKeyword(bool value)
        {
            MinValue = value;
        }

        /// <summary>
        /// Implementation of IJsonSchemaKeyword interface
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Evaluate(EvaluationContext context)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// The Exclusive maximum keyword as defined in JSON schema Draft4
    /// </summary>
    [SchemaKeyword(Name)]
    public class Draft4ExclusiveMaximumKeyword : IJsonSchemaKeyword
    {
        /// <summary>
        /// The schema keyword name
        /// </summary>
        public const string Name = "exclusiveMaximum";

        /// <summary>
        /// The ID.
        /// </summary>
        public bool MaxValue { get; }

        internal Draft4ExclusiveMaximumKeyword(bool value)
        {
            MaxValue = value;
        }

        /// <summary>
        /// Implementation of IJsonSchemaKeyword interface
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Evaluate(EvaluationContext context)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// The nullable keyword
    /// </summary>
    [SchemaKeyword(Name)]
    public class NullableKeyword : IJsonSchemaKeyword
    {
        /// <summary>
        /// The schema keyword name
        /// </summary>
        public const string Name = "nullable";

        /// <summary>
        /// The ID.
        /// </summary>
        public bool Value { get; }

        /// <summary>
        /// Creates a new <see cref="NullableKeyword"/>.
        /// </summary>
        /// <param name="value">Whether the `minimum` value should be considered exclusive.</param>
        public NullableKeyword(bool value)
        {
            Value = value;
        }

        /// <summary>
        /// Implementation of IJsonSchemaKeyword interface
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Evaluate(EvaluationContext context)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// The nullable keyword
    /// </summary>
    [SchemaKeyword(Name)]
    public class ExternalDocsKeyword : IJsonSchemaKeyword
    {
        /// <summary>
        /// The schema keyword name
        /// </summary>
        public const string Name = "externalDocs";

        /// <summary>
        /// The ID.
        /// </summary>
        public OpenApiExternalDocs Value { get; }

        /// <summary>
        /// Creates a new <see cref="ExternalDocsKeyword"/>.
        /// </summary>
        /// <param name="value">Whether the `minimum` value should be considered exclusive.</param>
        public ExternalDocsKeyword(OpenApiExternalDocs value)
        {
            Value = value;
        }

        /// <summary>
        /// Implementation of IJsonSchemaKeyword interface
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Evaluate(EvaluationContext context)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// The extensions keyword
    /// </summary>
    [SchemaKeyword(Name)]
    [SchemaSpecVersion(SpecVersion.Draft202012)]
    [JsonConverter(typeof(ExtensionsKeywordJsonConverter))]
    public class ExtensionsKeyword : IJsonSchemaKeyword
    {
        /// <summary>
        /// The schema keyword name
        /// </summary>
        public const string Name = "extensions";

        internal IDictionary<string, IOpenApiExtension> Extensions { get; }

        internal ExtensionsKeyword(IDictionary<string, IOpenApiExtension> extensions)
        {
            Extensions = extensions;
        }

        /// <summary>
        /// Implementation of IJsonSchemaKeyword interface
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Evaluate(EvaluationContext context)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class ExtensionsKeywordJsonConverter : JsonConverter<ExtensionsKeyword>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="typeToConvert"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override ExtensionsKeyword Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        /// <exception cref="NotImplementedException"></exception>
        public override void Write(Utf8JsonWriter writer, ExtensionsKeyword value, JsonSerializerOptions options)
        {
            foreach (var item in value.Extensions)
            {
                var content = item.Value as OpenApiAny;
                if (content != null)
                {
                    writer.WritePropertyName(item.Key);
                    content.Node.WriteTo(writer);
                }
            }
                
        }

    }

    /// <summary>
    /// The summary keyword
    /// </summary>
    [SchemaKeyword(Name)]
    public class SummaryKeyword : IJsonSchemaKeyword
    {
        /// <summary>
        /// The schema keyword name
        /// </summary>
        public const string Name = "summary";

        internal string Summary { get; }

        internal SummaryKeyword(string summary)
        {
            Summary = summary;
        }

        /// <summary>
        /// Implementation of IJsonSchemaKeyword interface
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Evaluate(EvaluationContext context)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// The AdditionalPropertiesAllowed Keyword
    /// </summary>
    [SchemaKeyword(Name)]
    public class AdditionalPropertiesAllowedKeyword : IJsonSchemaKeyword
    {
        /// <summary>
        /// The schema keyword name
        /// </summary>
        public const string Name = "additionalPropertiesAllowed";
        
        internal bool AdditionalPropertiesAllowed { get; }

        internal AdditionalPropertiesAllowedKeyword(bool additionalPropertiesAllowed)
        {
            AdditionalPropertiesAllowed = additionalPropertiesAllowed;
        }

        /// <summary>
        /// Implementation of IJsonSchemaKeyword interface
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Evaluate(EvaluationContext context)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// The Discriminator Keyword
    /// </summary>
    [SchemaKeyword(Name)]
    [SchemaSpecVersion(SpecVersion.Draft202012)]
    public class DiscriminatorKeyword : OpenApiDiscriminator, IJsonSchemaKeyword
    {
        /// <summary>
        /// The schema keyword name
        /// </summary>
        public const string Name = "discriminator";

        /// <summary>
        /// Parameter-less constructor
        /// </summary>
        public DiscriminatorKeyword() : base() { }

        /// <summary>
        /// Initializes a copy of an <see cref="OpenApiDiscriminator"/> instance
        /// </summary>
        internal DiscriminatorKeyword(OpenApiDiscriminator discriminator) : base(discriminator) { }

        /// <summary>
        /// Implementation of IJsonSchemaKeyword interface
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Evaluate(EvaluationContext context)
        {
            throw new NotImplementedException();
        }
    }

}
