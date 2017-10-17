//---------------------------------------------------------------------
// <copyright file="OpenApiFloat.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OpenApi
{
    public class OpenApiFloat : OpenApiPrimitive<float>
    {
        public override PrimitiveTypeKind PrimitiveKind { get; } = PrimitiveTypeKind.Float;

        public OpenApiFloat(float value)
            : base(value)
        { }
    }
}
