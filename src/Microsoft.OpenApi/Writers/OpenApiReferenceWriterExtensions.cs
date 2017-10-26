// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OpenApi.Interfaces;

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
