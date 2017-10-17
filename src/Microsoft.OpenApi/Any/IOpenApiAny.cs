//---------------------------------------------------------------------
// <copyright file="IOpenApiAny.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OpenApi
{
    public enum AnyTypeKind
    {
        None,

        Primitive,

        Null,

        Array,

        Object
    }

    public interface IOpenApiAny
    {
        AnyTypeKind AnyKind { get; }
    }
}
