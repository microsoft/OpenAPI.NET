//---------------------------------------------------------------------
// <copyright file="OpenApiBoolean.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OpenApi
{
    public class OpenApiBoolean : OpenApiPrimitive<bool>
    {
        public override PrimitiveTypeKind PrimitiveKind { get; } = PrimitiveTypeKind.Boolean;

        public OpenApiBoolean(bool value)
            : base(value)
        { }
    }
}
