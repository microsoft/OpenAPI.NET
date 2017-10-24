//---------------------------------------------------------------------
// <copyright file="IOpenApiPrimitive.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OpenApi
{
    public enum PrimitiveTypeKind
    {
        Integer,
        Long,
        Float,
        Double,
        String,
        Byte,
        Binary,
        Boolean,
        Date,
        DateTime,
        Password
    }

    public interface IOpenApiPrimitive : IOpenApiAny
    {
        PrimitiveTypeKind PrimitiveKind { get; }
    }
}
