using System;
using System.Text.Json.Serialization;
using System.Text.Json;
using Json.Schema;
using Json.More;

namespace Microsoft.OpenApi.Draft4Support
{
    [SchemaKeyword(Name)]
    [SchemaSpecVersion(Draft4SupportData.Draft4Version)]
    [SchemaSpecVersion(SpecVersion.Draft202012)]
    [JsonConverter(typeof(Draft4ExclusiveMinimumKeywordJsonConverter))]
    internal class Draft4ExclusiveMinimumKeyword : IJsonSchemaKeyword, IEquatable<Draft4ExclusiveMinimumKeyword>
    {
        public const string Name = "exclusiveMinimum";

        private readonly ExclusiveMinimumKeyword _numberSupport;

        /// <summary>
        /// The ID.
        /// </summary>
        public bool? BoolValue { get; }

        public decimal? NumberValue => _numberSupport?.Value;

        /// <summary>
        /// Creates a new <see cref="IdKeyword"/>.
        /// </summary>
        /// <param name="value">Whether the `minimum` value should be considered exclusive.</param>
        public Draft4ExclusiveMinimumKeyword(bool value)
        {
            BoolValue = value;
        }

        public Draft4ExclusiveMinimumKeyword(decimal value)
        {
            _numberSupport = new ExclusiveMinimumKeyword(value);
        }

        public void Evaluate(EvaluationContext context)
        {
            // TODO: do we need to validate that the right version of the keyword is being used?
            if (BoolValue.HasValue)
            {
                context.EnterKeyword(Name);
                if (!BoolValue.Value)
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
                    context.LocalResult.Fail(Name, ErrorMessages.ExclusiveMaximum, ("received", number), ("limit", BoolValue));
                context.ExitKeyword(Name, context.LocalResult.IsValid);
            }
            else
            {
                _numberSupport.Evaluate(context);
            }
        }

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other">other</paramref> parameter; otherwise, false.</returns>
        public bool Equals(Draft4ExclusiveMinimumKeyword other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(BoolValue, other.BoolValue);
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as Draft4ExclusiveMinimumKeyword);
        }

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return BoolValue.GetHashCode();
        }
    }

    internal class Draft4ExclusiveMinimumKeywordJsonConverter : JsonConverter<Draft4ExclusiveMinimumKeyword>
    {
        public override Draft4ExclusiveMinimumKeyword Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.TokenType switch
            {
                JsonTokenType.True or JsonTokenType.False => new Draft4ExclusiveMinimumKeyword(reader.GetBoolean()),
                JsonTokenType.Number => new Draft4ExclusiveMinimumKeyword(reader.GetDecimal()),
                _ => throw new JsonException("Expected boolean or number")
            };
        }

        public override void Write(Utf8JsonWriter writer, Draft4ExclusiveMinimumKeyword value, JsonSerializerOptions options)
        {
            if (value.BoolValue.HasValue)
                writer.WriteBoolean(Draft4ExclusiveMinimumKeyword.Name, value.BoolValue.Value);
            else
                writer.WriteNumber(Draft4ExclusiveMinimumKeyword.Name, value.NumberValue.Value);
        }
    }
}
