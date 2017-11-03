﻿// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;

namespace Microsoft.OpenApi.Readers.V2
{
    /// <summary>
    /// Class containing logic to deserialize Open API V2 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV2Deserializer
    {
        #region SecurityRequirement
        public static OpenApiSecurityRequirement LoadSecurityRequirement(ParseNode node)
        {
            var mapNode = node.CheckMapNode("security");

            var obj = new OpenApiSecurityRequirement();

            foreach (var property in mapNode)
            {
                var scheme = OpenApiReferenceService.LoadSecuritySchemeByReference(mapNode.Context, mapNode.Diagnostic, property.Name);
                if (scheme != null)
                {
                    obj.Schemes.Add(scheme, property.Value.CreateSimpleList<string>(n2 => n2.GetScalarValue()));
                }
                else
                {
                    node.Diagnostic.Errors.Add(new OpenApiError(node.Context.GetLocation(), $"Scheme {property.Name} is not found"));
                }
            }
            return obj;
        }

        #endregion
    }
}
