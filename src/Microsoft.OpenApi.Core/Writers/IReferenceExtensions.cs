﻿namespace Microsoft.OpenApi.Writers
{
    using Microsoft.OpenApi;

    public static class IReferenceExtensions
    {
        public static bool IsReference(this IReference reference)
        {
            return reference.Pointer != null;
        }

        public static void WriteRef(this IReference reference, IParseNodeWriter writer)
        {
            writer.WriteStartMap();
            writer.WriteStringProperty("$ref", reference.Pointer.ToString());
            writer.WriteEndMap();
        }

    }
}
