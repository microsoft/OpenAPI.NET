//---------------------------------------------------------------------
// <copyright file="OpenApiPassword.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OpenApi
{
    public class OpenApiPassword : OpenApiPrimitive<string>
    {
        public override PrimitiveTypeKind PrimitiveKind { get; } = PrimitiveTypeKind.Password;

        public OpenApiPassword(string value)
            : base(value)
        { }
    }
}
