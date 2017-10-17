//---------------------------------------------------------------------
// <copyright file="OpenApiBinary.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OpenApi
{
    public class OpenApiBinary : OpenApiPrimitive<byte[]>
    {
        public override PrimitiveTypeKind PrimitiveKind { get; } = PrimitiveTypeKind.Binary;

        public OpenApiBinary(byte[] value)
            : base(value)
        { }
    }
}
