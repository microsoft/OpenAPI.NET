// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Services;

namespace Microsoft.OpenApi.Readers
{

    internal class Resolver : OpenApiVisitorBase
    {
        private OpenApiDocument _currentDocument;

        public Resolver(OpenApiDocument currentDocument)  // In future pass in Workbench for remote references
        {
            _currentDocument = currentDocument;
        }

        public override void Visit(OpenApiMediaType mediaType)
        {
            if (IsReference(mediaType.Schema))
            {
                mediaType.Schema = ResolveReference<OpenApiSchema>(mediaType.Schema.Reference);
            }
        }

        private T ResolveReference<T>(OpenApiReference reference) where T : class
        {
            if (string.IsNullOrEmpty(reference.ExternalResource))
            {
                return _currentDocument.ResolveReference(reference) as T;
            }
            else
            {
                // TODO
                return default(T);
            }
        }

        private bool IsReference(IOpenApiReferenceable possibleReference)
        {
            return (possibleReference != null && possibleReference.Reference != null);
        }
    }
}
