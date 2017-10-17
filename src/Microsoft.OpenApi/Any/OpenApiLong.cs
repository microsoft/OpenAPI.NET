//---------------------------------------------------------------------
// <copyright file="OpenApiLong.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OpenApi
{
    public class OpenApiLong : OpenApiPrimitive<long>
    {
        public override PrimitiveTypeKind PrimitiveKind { get; } = PrimitiveTypeKind.Long;

        public OpenApiLong(long value)
            : base(value)
        {
        }
    }
}
