// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.IO;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Writers
{
    public interface IOpenApiStructureWriter
    {
        void Write(Stream stream, OpenApiDocument document);
    }

    public interface IOpenApiDocumentSerializer
    {
        bool Save(OpenApiDocument document);
    }
}
