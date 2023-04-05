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
        private readonly JsonDocument _jsonDocument;

        public RootNode(
            ParsingContext context,
            JsonDocument jsonDocument) : base(context)
        {
            _jsonDocument = jsonDocument;
        }

        public ParseNode Find(JsonPointer referencePointer)
        {
            var jsonNode = referencePointer.Find(_jsonDocument.RootElement);
            if (jsonNode == null)
            {
                return null;
            }

            return Create(Context, jsonNode);
        }

        public MapNode GetMap()
        {
            return new MapNode(Context, _jsonDocument.RootElement);
        }
    }
}
