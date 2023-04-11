// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Text.Json;
using System.Text.Json.Nodes;
using SharpYaml.Serialization;

namespace Microsoft.OpenApi.Readers.ParseNodes
{
    /// <summary>
    /// Wrapper class around JsonDocument to isolate semantic parsing from details of Json DOM.
    /// </summary>
    internal class RootNode : ParseNode
    {
        private readonly JsonNode _jsonNode;

        public RootNode(
            ParsingContext context,
            JsonNode jsonNode) : base(context)
        {
            _jsonNode = jsonNode;
        }

        public ParseNode Find(JsonPointer referencePointer)
        {
            var jsonNode = referencePointer.Find(_jsonNode);
            if (jsonNode == null)
            {
                return null;
            }

            return Create(Context, jsonNode);
        }

        public MapNode GetMap()
        {
            var jsonNode = _jsonNode;
            return new MapNode(Context, jsonNode);
        }
    }
}
