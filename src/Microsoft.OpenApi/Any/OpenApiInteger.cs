// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.OpenApi.Any
{
    public class OpenApiInteger : OpenApiPrimitive<int>
    {
        public override PrimitiveType PrimitiveType { get; } = PrimitiveType.Integer;

        public OpenApiInteger(int value)
            : base(value)
        { }
    }
}
