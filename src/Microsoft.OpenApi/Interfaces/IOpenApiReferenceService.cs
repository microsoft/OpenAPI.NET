// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi
{
    public interface IOpenApiReferenceService
    {
        IOpenApiReference LoadReference(OpenApiReference reference);

        OpenApiReference ParseReference(string pointer);
    }
}
