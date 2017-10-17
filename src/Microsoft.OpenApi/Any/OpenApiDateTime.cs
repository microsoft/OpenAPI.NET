//---------------------------------------------------------------------
// <copyright file="OpenApiDateTime.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;

namespace Microsoft.OpenApi
{
    public class OpenApiDateTime : OpenApiPrimitive<DateTimeOffset>
    {
        public override PrimitiveTypeKind PrimitiveKind { get; } = PrimitiveTypeKind.DateTime;

        public OpenApiDateTime(DateTimeOffset value)
            : base(value)
        { }
    }
}
