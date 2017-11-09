// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.OpenApi.Any
{
    public class OpenApiDouble : OpenApiPrimitive<double>
    {
        public override PrimitiveType PrimitiveType { get; } = PrimitiveType.Double;

        public OpenApiDouble(float value)
            : base(value)
        { }
    }
}
