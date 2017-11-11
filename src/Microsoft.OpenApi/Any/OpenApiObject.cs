// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OpenApi.Any
{
    /// <summary>
    /// Open API object.
    /// </summary>
    public class OpenApiObject : Dictionary<string, IOpenApiAny>, IOpenApiAny
    {
        /// <summary>
        /// Type of <see cref="IOpenApiAny"/>.
        /// </summary>
        public AnyType AnyType { get; } = AnyType.Object;
    }
}
