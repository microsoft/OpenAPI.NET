// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;

namespace Microsoft.OpenApi.Readers.V3
{
    /// <summary>
    /// Class containing logic to deserialize Open API V3 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV3Deserializer
    {
        public static OpenApiTag LoadTag(ParseNode n)
        {
            var mapNode = n.CheckMapNode("tag");

            var obj = new OpenApiTag();

            foreach (var node in mapNode)
            {
                var key = node.Name;
                switch (key)
                {
                    case "description":
                        obj.Description = node.Value.GetScalarValue();
                        break;
                    case "name":
                        obj.Name = node.Value.GetScalarValue();
                        break;
                }
            }

            return obj;
        }
    }
}