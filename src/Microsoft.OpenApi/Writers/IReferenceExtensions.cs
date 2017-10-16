//---------------------------------------------------------------------
// <copyright file="IReferenceExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OpenApi.Writers
{
    using Microsoft.OpenApi;

    public static class IReferenceExtensions
    {
        public static bool IsReference(this IReference reference)
        {
            return reference.Pointer != null;
        }

        public static void WriteRef(this IReference reference, IOpenApiWriter writer)
        {
            writer.WriteStartObject();
            writer.WriteStringProperty("$ref", reference.Pointer.ToString());
            writer.WriteEndObject();
        }

    }
}
