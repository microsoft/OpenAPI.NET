// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Services
{
    internal static class OpenApiComponentsRegistryExtensions
    {
        public static void RegisterComponents(this OpenApiWorkspace workspace, OpenApiDocument document)
        {
            if (document?.Components == null) return;

            var baseUri = document.BaseUri + "/components/";

            // Register Schema
            foreach (var item in document.Components.Schemas)
            {
                var location = baseUri + ReferenceType.Schema.GetDisplayName() + "/" + item.Key;
                workspace.RegisterComponent(location, item.Value);
            }

            // Register Parameters
            foreach (var item in document.Components.Parameters)
            {
                var location = baseUri + ReferenceType.Parameter.GetDisplayName() + "/" + item.Key;
                workspace.RegisterComponent(location, item.Value);
            }

            // Register Responses
            foreach (var item in document.Components.Responses)
            {
                var location = baseUri + ReferenceType.Response.GetDisplayName() + "/" + item.Key;
                workspace.RegisterComponent(location, item.Value);
            }

            // Register RequestBodies
            foreach (var item in document.Components.RequestBodies)
            {
                var location = baseUri + ReferenceType.RequestBody.GetDisplayName() + "/" + item.Key;
                workspace.RegisterComponent(location, item.Value);
            }

            // Register Links
            foreach (var item in document.Components.Links)
            {
                var location = baseUri + ReferenceType.Link.GetDisplayName() + "/" + item.Key;
                workspace.RegisterComponent(location, item.Value);
            }

            // Register Callbacks
            foreach (var item in document.Components.Callbacks)
            {
                var location = baseUri + ReferenceType.Callback.GetDisplayName() + "/" + item.Key;
                workspace.RegisterComponent(location, item.Value);
            }

            // Register PathItems
            foreach (var item in document.Components.PathItems)
            {
                var location = baseUri + ReferenceType.PathItem.GetDisplayName() + "/" + item.Key;
                workspace.RegisterComponent(location, item.Value);
            }

            // Register Examples
            foreach (var item in document.Components.Examples)
            {
                var location = baseUri + ReferenceType.Example.GetDisplayName() + "/" + item.Key;
                workspace.RegisterComponent(location, item.Value);
            }

            // Register Headers
            foreach (var item in document.Components.Headers)
            {
                var location = baseUri + ReferenceType.Header.GetDisplayName() + "/" + item.Key;
                workspace.RegisterComponent(location, item.Value);
            }

            // Register SecuritySchemes
            foreach (var item in document.Components.SecuritySchemes)
            {
                var location = baseUri + ReferenceType.SecurityScheme.GetDisplayName() + "/" + item.Key;
                workspace.RegisterComponent(location, item.Value);
            }
        }
    }
}
