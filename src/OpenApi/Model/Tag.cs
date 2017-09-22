using System;
using SharpYaml.Serialization;

namespace Tavis.OpenApi.Model
{
    public class Tag : IReference
    {
        public string Name { get; set; }
        public string Description { get; set; }

        OpenApiReference IReference.Pointer
        {
            get; set;
        }

        internal static Tag LoadByReference(ParseNode node)
        {
            var tagName = node.GetScalarValue();
            var context = node.Context;
            var tagObject = (Tag)context.GetReferencedObject($"#/tags/{tagName}");

            if (tagObject == null)
            {
                tagObject = new Tag() { Name = tagName };
            }
            return tagObject;
        }

        public void WriteRef(IParseNodeWriter writer)
        {
            writer.WriteValue(Name);
        }

        public static void WriteRef(IParseNodeWriter writer, Tag tag)
        {
            tag.WriteRef(writer);
        }
    }
}