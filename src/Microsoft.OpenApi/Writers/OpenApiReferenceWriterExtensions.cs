//---------------------------------------------------------------------
// <copyright file="OpenApiReferenceWriterExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OpenApi.Writers
{
    public static class OpenApiReferenceWriterExtensions
    {
        public static bool IsReference(this IOpenApiReference reference)
        {
            return reference.Pointer != null;
        }

        public static void WriteRef(this IOpenApiReference reference, IOpenApiWriter writer)
        {
            writer.WriteStartObject();
            writer.WriteStringProperty("$ref", reference.Pointer.ToString());
            writer.WriteEndObject();
        }
    }
}
