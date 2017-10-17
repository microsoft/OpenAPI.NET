//---------------------------------------------------------------------
// <copyright file="OpenApiDouble.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OpenApi
{
    public class OpenApiDouble : OpenApiPrimitive<double>
    {
        public override PrimitiveTypeKind PrimitiveKind { get; } = PrimitiveTypeKind.Double;

        public OpenApiDouble(float value)
            : base(value)
        { }
    }
}
