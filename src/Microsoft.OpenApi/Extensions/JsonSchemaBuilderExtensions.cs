// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Json.More;
using Json.Schema;
using Json.Schema.OpenApi;
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

            var extensions = discriminator.Extensions.ToDictionary(kvp => kvp.Key, kvp => ((OpenApiAny)kvp.Value).Node);
            if (extensions.Count == 0)
            {
                extensions = null;
            }
            var mapping = (IReadOnlyDictionary<string, string>)discriminator.Mapping;
            if (mapping.Count == 0)
            {
                mapping = null;
            }
            var discriminatorKeyword = new DiscriminatorKeyword(
                            discriminator.PropertyName,
                            mapping, 
                            extensions);
            builder.Add(discriminatorKeyword);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="schemaConstraint"></param>
        /// <param name="localConstraints"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public KeywordConstraint GetConstraint(SchemaConstraint schemaConstraint, IReadOnlyList<KeywordConstraint> localConstraints, EvaluationContext context)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="schemaConstraint"></param>
        /// <param name="localConstraints"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public KeywordConstraint GetConstraint(SchemaConstraint schemaConstraint, IReadOnlyList<KeywordConstraint> localConstraints, EvaluationContext context)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="schemaConstraint"></param>
        /// <param name="localConstraints"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public KeywordConstraint GetConstraint(SchemaConstraint schemaConstraint, IReadOnlyList<KeywordConstraint> localConstraints, EvaluationContext context)
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
        /// 
        /// </summary>
        /// <param name="schemaConstraint"></param>
        /// <param name="localConstraints"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public KeywordConstraint GetConstraint(SchemaConstraint schemaConstraint, IReadOnlyList<KeywordConstraint> localConstraints, EvaluationContext context)
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
            var dict = new Dictionary<string, IOpenApiExtension>();
            // iterate over the reader and create extensions values
            //
            return new ExtensionsKeyword(dict);
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

}
