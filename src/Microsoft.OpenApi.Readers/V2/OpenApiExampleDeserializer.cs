// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------


using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;
using System.Collections.Generic;

namespace Microsoft.OpenApi.Readers.V2
{
    /// <summary>
    /// Class containing logic to deserialize Open API V2 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV2Deserializer
    {
        public static void LoadExamples(OpenApiResponse response, ParseNode node)
        {
            var mapNode = node.CheckMapNode("examples");
            foreach (var mediaTypeNode in mapNode)
            {
                LoadExample(response, mediaTypeNode.Name, mediaTypeNode.Value);
            }
        }

            public static void LoadExample(OpenApiResponse response, string mediaType, ParseNode node)
        {
            var exampleNode = node.CreateAny();

            if (response.Content == null)
            {
                response.Content = new Dictionary<string, OpenApiMediaType>();
            }
            OpenApiMediaType mediaTypeObject;
            if (response.Content.ContainsKey(mediaType))
            {
                mediaTypeObject = response.Content[mediaType];
            }
            else
            {
                mediaTypeObject = new OpenApiMediaType();
                response.Content.Add(mediaType, mediaTypeObject);
            }
            mediaTypeObject.Example = exampleNode;

        }
    }
}