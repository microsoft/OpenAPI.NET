// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OpenApi.Readers.Interface;

namespace Microsoft.OpenApi.Readers.YamlReaders
{
    public class OpenApiDiagnostic : IDiagnostic
    {
        public IList<OpenApiError> Errors { get; set; } = new List<OpenApiError>();
    }
}