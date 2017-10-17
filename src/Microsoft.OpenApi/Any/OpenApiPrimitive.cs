//---------------------------------------------------------------------
// <copyright file="OpenApiPrimitive.cs" company="Microsoft">
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

    public abstract class OpenApiPrimitive<T> : IOpenApiAny
    {
        public AnyTypeKind AnyKind { get; } = AnyTypeKind.Primitive;

        public abstract PrimitiveTypeKind PrimitiveKind { get; }

        public T Value { get; }

        public OpenApiPrimitive(T value)
        {
            Value = value;
        }
    }
}
