//---------------------------------------------------------------------
// <copyright file="OpenApiInteger.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OpenApi
{
    public class OpenApiInteger : OpenApiPrimitive<int>
    {
        public override PrimitiveTypeKind PrimitiveKind { get; } = PrimitiveTypeKind.Integer;

        public OpenApiInteger(int value)
            : base(value)
        { }
    }
}
