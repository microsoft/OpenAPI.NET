// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OpenApi.Any
{
    public class OpenApiArray : List<IOpenApiAny>, IOpenApiAny
    {
        public AnyTypeKind AnyKind { get; } = AnyTypeKind.Array;
    }
}
