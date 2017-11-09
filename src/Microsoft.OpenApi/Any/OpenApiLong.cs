// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.OpenApi.Any
{
    public class OpenApiLong : OpenApiPrimitive<long>
    {
        public override PrimitiveType PrimitiveType { get; } = PrimitiveType.Long;

        public OpenApiLong(long value)
            : base(value)
        {
        }
    }
}
