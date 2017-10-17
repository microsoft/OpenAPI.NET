//---------------------------------------------------------------------
// <copyright file="OpenApiDate.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;

namespace Microsoft.OpenApi
{
    public class OpenApiDate : OpenApiPrimitive<DateTime>
    {
        public override PrimitiveTypeKind PrimitiveKind { get; } = PrimitiveTypeKind.Date;

        public OpenApiDate(DateTime value)
            : base(value)
        { }
    }
}
