// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Services
{
    internal static class OpenApiComponentsRegistryExtensions
    {
        public static void RegisterComponents(this OpenApiWorkspace workspace, OpenApiDocument document, OpenApiSpecVersion version = OpenApiSpecVersion.OpenApi3_0)
        {
            if (document?.Components == null) return;

            string baseUri = document.BaseUri + OpenApiConstants.ComponentsSegment;
            string location;

            // Register Schema
            foreach (var item in document.Components.Schemas)
            {
                if (item.Value.Id != null)
                {
                    location = document.BaseUri + item.Value.Id;
                }
                else
                {
                    location = baseUri + ReferenceType.Schema.GetDisplayName() + "/" + item.Key;
                }

                workspace.RegisterComponent(location, item.Value);
            }

            // Register Parameters
            foreach (var item in document.Components.Parameters)
            {
                location = baseUri + ReferenceType.Parameter.GetDisplayName() + "/" + item.Key;
                workspace.RegisterComponent(location, item.Value);
            }

            // Register Responses
            foreach (var item in document.Components.Responses)
            {
                location = baseUri + ReferenceType.Response.GetDisplayName() + "/" + item.Key;
                workspace.RegisterComponent(location, item.Value);
            }

            // Register RequestBodies
            foreach (var item in document.Components.RequestBodies)
            {
                location = baseUri + ReferenceType.RequestBody.GetDisplayName() + "/" + item.Key;
                workspace.RegisterComponent(location, item.Value);
            }

            // Register Links
            foreach (var item in document.Components.Links)
            {
                location = baseUri + ReferenceType.Link.GetDisplayName() + "/" + item.Key;
                workspace.RegisterComponent(location, item.Value);
            }

            // Register Callbacks
            foreach (var item in document.Components.Callbacks)
            {
                location = baseUri + ReferenceType.Callback.GetDisplayName() + "/" + item.Key;
                workspace.RegisterComponent(location, item.Value);
            }

            // Register PathItems
            foreach (var item in document.Components.PathItems)
            {
                location = baseUri + ReferenceType.PathItem.GetDisplayName() + "/" + item.Key;
                workspace.RegisterComponent(location, item.Value);
            }

            // Register Examples
            foreach (var item in document.Components.Examples)
            {
                location = baseUri + ReferenceType.Example.GetDisplayName() + "/" + item.Key;
                workspace.RegisterComponent(location, item.Value);
            }

            // Register Headers
            foreach (var item in document.Components.Headers)
            {
                location = baseUri + ReferenceType.Header.GetDisplayName() + "/" + item.Key;
                workspace.RegisterComponent(location, item.Value);
            }

            // Register SecuritySchemes
            foreach (var item in document.Components.SecuritySchemes)
            {
                location = baseUri + ReferenceType.SecurityScheme.GetDisplayName() + "/" + item.Key;
                workspace.RegisterComponent(location, item.Value);
            }
        }
    }
}
