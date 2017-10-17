//---------------------------------------------------------------------
// <copyright file="OpenApiArray.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OpenApi
{
    public class OpenApiArray : List<IOpenApiAny>, IOpenApiAny
    {
        public AnyTypeKind AnyKind { get; } = AnyTypeKind.Array;
    }
}
