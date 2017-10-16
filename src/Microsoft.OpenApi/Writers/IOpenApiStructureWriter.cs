//---------------------------------------------------------------------
// <copyright file="IOpenApiWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OpenApi.Writers
{
    using System.IO;
    using Microsoft.OpenApi;
    public interface IOpenApiStructureWriter {
        void Write(Stream stream, OpenApiDocument document);
    }
}
