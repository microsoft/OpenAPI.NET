using System;
using System.Text.Json.Serialization;
using System.Text.Json;
using Json.Schema;
using Json.More;

namespace Microsoft.OpenApi.Draft4Support
{
    [SchemaKeyword(Name)]
    [SchemaSpecVersion(Draft4SupportData.Draft4Version)]
    [JsonConverter(typeof(Draft4ExclusiveMaximumKeywordJsonConverter))]
    internal class Draft4ExclusiveMaximumKeyword : IJsonSchemaKeyword, IEquatable<Draft4ExclusiveMaximumKeyword>
    {
        public const string Name = "exclusiveMaximum";

        /// <summary>
        /// The ID.
        /// </summary>
        public bool Value { get; }

        /// <summary>
        /// Creates a new <see cref="IdKeyword"/>.
        /// </summary>
        /// <param name="value">Whether the `minimum` value should be considered exclusive.</param>
        public Draft4ExclusiveMaximumKeyword(bool value)
        {
            Value = value;
        }

        public void Evaluate(EvaluationContext context)
        {
            context.EnterKeyword(Name);
            if (!Value)
            {
                context.NotApplicable(() => "exclusiveMinimum is false; minimum validation is sufficient");
                return;
            }

            var limit = context.LocalSchema.GetMinimum();
            if (!limit.HasValue)
            {
                context.NotApplicable(() => "minimum not present");
                return;
            }

            var schemaValueType = context.LocalInstance.GetSchemaValueType();
            if (schemaValueType is not (SchemaValueType.Number or SchemaValueType.Integer))
            {
                context.WrongValueKind(schemaValueType);
                return;
            }

            var number = context.LocalInstance!.AsValue().GetNumber();

            if (limit == number)
                context.LocalResult.Fail(Name, ErrorMessages.ExclusiveMaximum, ("received", number), ("limit", Value));
            context.ExitKeyword(Name, context.LocalResult.IsValid);
        }

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other">other</paramref> parameter; otherwise, false.</returns>
        public bool Equals(Draft4ExclusiveMaximumKeyword other)
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
            return Equals(obj as Draft4ExclusiveMaximumKeyword);
        }

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }

    internal class Draft4ExclusiveMaximumKeywordJsonConverter : JsonConverter<Draft4ExclusiveMaximumKeyword>
    {
        public override Draft4ExclusiveMaximumKeyword Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType is not JsonTokenType.True or JsonTokenType.False)
                throw new JsonException("Expected boolean");

            var value = reader.GetBoolean();

            return new Draft4ExclusiveMaximumKeyword(value);
        }

        public override void Write(Utf8JsonWriter writer, Draft4ExclusiveMaximumKeyword value, JsonSerializerOptions options)
        {
            writer.WriteBoolean(Draft4ExclusiveMaximumKeyword.Name, value.Value);
        }
    }
}
