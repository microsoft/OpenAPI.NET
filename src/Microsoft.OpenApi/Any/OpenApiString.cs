// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.OpenApi.Any
{
    public class OpenApiString : OpenApiPrimitive<string>
    {
        public override PrimitiveTypeKind PrimitiveKind { get; } = PrimitiveTypeKind.String;

        public OpenApiString(string value)
            : base(value)
        { }
    }
}
