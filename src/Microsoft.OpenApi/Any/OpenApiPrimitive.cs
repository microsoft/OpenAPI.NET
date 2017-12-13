// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

namespace Microsoft.OpenApi.Any
{
    /// <summary>
    /// Open API primitive class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class OpenApiPrimitive<T> : IOpenApiPrimitive
    {
        /// <summary>
        /// Initializes the <see cref="IOpenApiPrimitive"/> class with the given value.
        /// </summary>
        /// <param name="value"></param>
        public OpenApiPrimitive(T value)
        {
            Value = value;
        }

        /// <summary>
        /// The kind of <see cref="IOpenApiAny"/>.
        /// </summary>
        public AnyType AnyType { get; } = AnyType.Primitive;

        /// <summary>
        /// The primitive class this object represents.
        /// </summary>
        public abstract PrimitiveType PrimitiveType { get; }

        /// <summary>
        /// Value of this <see cref="IOpenApiPrimitive"/>
        /// </summary>
        public T Value { get; }
    }
}