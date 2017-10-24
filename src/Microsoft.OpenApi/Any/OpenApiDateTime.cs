// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;

namespace Microsoft.OpenApi.Any
{
    public class OpenApiDateTime : OpenApiPrimitive<DateTimeOffset>
    {
        public override PrimitiveTypeKind PrimitiveKind { get; } = PrimitiveTypeKind.DateTime;

        public OpenApiDateTime(DateTimeOffset value)
            : base(value)
        { }
    }
}
