using System;
using System.Text.Json.Serialization;
using System.Text.Json;
using Json.Schema;

namespace Microsoft.OpenApi.Draft4Support
{
    [SchemaKeyword(Name)]
    [SchemaSpecVersion(Draft4SupportData.Draft4Version)]
    [SchemaSpecVersion(SpecVersion.Draft202012)]
    [JsonConverter(typeof(Draft4TypeKeywordConverter))]
    internal class Draft4TypeKeyword : IJsonSchemaKeyword, IEquatable<Draft4TypeKeyword>
    {
        public const string Name = "type";

        private readonly TypeKeyword _basicSupport;
        private readonly TypeKeyword _draft4Support;

        /// <summary>
        /// The ID.
        /// </summary>
        public SchemaValueType Type => _basicSupport.Type;

        /// <summary>
        /// Creates a new <see cref="IdKeyword"/>.
        /// </summary>
        /// <param name="type">The instance type that is allowed.</param>
        public Draft4TypeKeyword(SchemaValueType type)
        {
            _basicSupport = new TypeKeyword(type);
            _draft4Support = new TypeKeyword(type | SchemaValueType.Null);
        }

        public void Evaluate(EvaluationContext context)
        {
            if (context.Options.EvaluateAs == Draft4SupportData.Draft4Version)
            {
                _draft4Support.Evaluate(context);
            }
            else
            {
                _basicSupport.Evaluate(context);
            }
        }

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other">other</paramref> parameter; otherwise, false.</returns>
        public bool Equals(Draft4TypeKeyword other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Type, other.Type);
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as Draft4TypeKeyword);
        }

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return Type.GetHashCode();
        }
    }

    internal class Draft4TypeKeywordConverter : JsonConverter<Draft4TypeKeyword>
    {
        public override Draft4TypeKeyword Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var type = JsonSerializer.Deserialize<SchemaValueType>(ref reader, options);

            return new Draft4TypeKeyword(type);
        }
        public override void Write(Utf8JsonWriter writer, Draft4TypeKeyword value, JsonSerializerOptions options)
        {
            writer.WritePropertyName(Draft4TypeKeyword.Name);
            JsonSerializer.Serialize(writer, value.Type, options);
        }
    }
}
