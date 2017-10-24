//---------------------------------------------------------------------
// <copyright file="IOpenApiStructureWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OpenApi.Writers
{
    using System.IO;
    using Microsoft.OpenApi;
    public interface IOpenApiStructureWriter
    {
        void Write(Stream stream, OpenApiDocument document);
    }

    public interface IOpenApiDocumentSerializer
    {
        bool Save(OpenApiDocument document);
    }
}
