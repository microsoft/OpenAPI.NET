using System;
using System.Text.Json.Serialization;
using System.Text.Json;
using Json.Schema;

namespace Microsoft.OpenApi.Draft4Support
{
    [SchemaKeyword(Name)]
    [SchemaSpecVersion(Draft4SupportData.Draft4Version)]
    [JsonConverter(typeof(NullableKeywordJsonConverter))]
    internal class NullableKeyword : IJsonSchemaKeyword, IEquatable<NullableKeyword>
    {
        public const string Name = "nullable";

        /// <summary>
        /// The ID.
        /// </summary>
        public bool Value { get; }

        /// <summary>
        /// Creates a new <see cref="IdKeyword"/>.
        /// </summary>
        /// <param name="value">Whether the `minimum` value should be considered exclusive.</param>
        public NullableKeyword(bool value)
        {
            Value = value;
        }

        public void Evaluate(EvaluationContext context)
        {
            context.EnterKeyword(Name);
            var schemaValueType = context.LocalInstance.GetSchemaValueType();
            if (schemaValueType == SchemaValueType.Null && !Value)
            {
                context.LocalResult.Fail(Name, "nulls are not allowed"); // TODO: localize error message
            }
            context.ExitKeyword(Name, context.LocalResult.IsValid);
        }

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other">other</paramref> parameter; otherwise, false.</returns>
        public bool Equals(NullableKeyword other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Value, other.Value);
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as NullableKeyword);
        }

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }

    internal class NullableKeywordJsonConverter : JsonConverter<NullableKeyword>
    {
        public override NullableKeyword Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType is not (JsonTokenType.True or JsonTokenType.False))
            {
                throw new JsonException("Expected boolean");
            }

            return new NullableKeyword(reader.GetBoolean());
        }

        public override void Write(Utf8JsonWriter writer, NullableKeyword value, JsonSerializerOptions options)
        {
            writer.WriteBoolean(NullableKeyword.Name, value.Value);
        }
    }
}
