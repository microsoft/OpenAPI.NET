using System;
using System.Text.Json.Serialization;
using System.Text.Json;
using Json.Schema;

namespace Microsoft.OpenApi.Draft4Support
{
    [SchemaKeyword(Name)]
    [SchemaPriority(int.MinValue + 1)]
    [SchemaSpecVersion(Draft4SupportData.Draft4Version)]
    [JsonConverter(typeof(Draft4IdKeywordJsonConverter))]
    internal class Draft4IdKeyword : IJsonSchemaKeyword, IEquatable<Draft4IdKeyword>
    {
        public const string Name = "id";

        /// <summary>
        /// The ID.
        /// </summary>
        public Uri Id { get; }

        /// <summary>
        /// Creates a new <see cref="IdKeyword"/>.
        /// </summary>
        /// <param name="id">The ID.</param>
        public Draft4IdKeyword(Uri id)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
        }

        public void Evaluate(EvaluationContext context)
        {
            context.EnterKeyword(Name);
            context.Log(() => "Nothing to do");
            context.ExitKeyword(Name, true);
        }

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other">other</paramref> parameter; otherwise, false.</returns>
        public bool Equals(Draft4IdKeyword other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Id, other.Id);
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as Draft4IdKeyword);
        }

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    internal class Draft4IdKeywordJsonConverter : JsonConverter<Draft4IdKeyword>
    {
        public override Draft4IdKeyword Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
                throw new JsonException("Expected string");

            var uriString = reader.GetString();
            if (!Uri.TryCreate(uriString, UriKind.RelativeOrAbsolute, out var uri))
                throw new JsonException("Expected URI");

            return new Draft4IdKeyword(uri);
        }

        public override void Write(Utf8JsonWriter writer, Draft4IdKeyword value, JsonSerializerOptions options)
        {
            writer.WriteString(Draft4IdKeyword.Name, value.Id.OriginalString);
        }
    }
}
