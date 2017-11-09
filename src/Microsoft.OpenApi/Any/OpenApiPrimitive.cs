// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.OpenApi.Any
{
    public abstract class OpenApiPrimitive<T> : IOpenApiPrimitive
    {
        public AnyTypeKind AnyKind { get; } = AnyTypeKind.Primitive;

        public abstract PrimitiveType PrimitiveType { get; }

        public T Value { get; }

        public OpenApiPrimitive(T value)
        {
            Value = value;
        }
    }
}
