//---------------------------------------------------------------------
// <copyright file="IReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OpenApi
{
    public interface IReference
    {
        OpenApiReference Pointer { get; set; }
    }
}
