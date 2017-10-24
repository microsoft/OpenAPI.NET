// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.OpenApi.Any
{
    public class OpenApiNull : IOpenApiAny
    {
        public AnyTypeKind AnyKind { get; } = AnyTypeKind.Null;
    }
}
