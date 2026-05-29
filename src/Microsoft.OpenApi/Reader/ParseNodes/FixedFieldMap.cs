// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi.Reader
{
    internal class FixedFieldMap<T> : Dictionary<string, Action<T, JsonNode, OpenApiDocument, ParsingContext>>
    {
    }
}
