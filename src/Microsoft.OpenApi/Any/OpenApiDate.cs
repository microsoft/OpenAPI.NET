// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;

namespace Microsoft.OpenApi.Any
{
    public class OpenApiDate : OpenApiPrimitive<DateTime>
    {
        public override PrimitiveType PrimitiveType { get; } = PrimitiveType.Date;

        public OpenApiDate(DateTime value)
            : base(value)
        { }
    }
}
