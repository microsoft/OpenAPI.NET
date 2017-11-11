// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OpenApi.Any
{
    /// <summary>
    /// Open API array.
    /// </summary>
    public class OpenApiArray : List<IOpenApiAny>, IOpenApiAny
    {
        /// <summary>
        /// The type of <see cref="IOpenApiAny"/>
        /// </summary>
        public AnyType AnyType { get; } = AnyType.Array;
    }
}
