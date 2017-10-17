//---------------------------------------------------------------------
// <copyright file="OpenApiByte.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OpenApi
{
    public class OpenApiByte : OpenApiPrimitive<byte>
    {
        public override PrimitiveTypeKind PrimitiveKind { get; } = PrimitiveTypeKind.Byte;

        public OpenApiByte(byte value)
            : base(value)
        { }
    }
}
