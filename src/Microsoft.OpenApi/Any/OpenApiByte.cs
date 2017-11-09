// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.OpenApi.Any
{
    public class OpenApiByte : OpenApiPrimitive<byte>
    {
        public override PrimitiveType PrimitiveType { get; } = PrimitiveType.Byte;

        public OpenApiByte(byte value)
            : base(value)
        { }
    }
}
