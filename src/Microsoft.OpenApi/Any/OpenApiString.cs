//---------------------------------------------------------------------
// <copyright file="OpenApiString.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OpenApi
{
    public class OpenApiString : OpenApiPrimitive<string>
    {
        public override PrimitiveTypeKind PrimitiveKind { get; } = PrimitiveTypeKind.String;

        public OpenApiString(string value)
            : base(value)
        { }
    }
}
