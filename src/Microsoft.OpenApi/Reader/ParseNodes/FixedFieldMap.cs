// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Reader.ParseNodes
{
    internal class FixedFieldMap<T> : Dictionary<string, Action<T, ParseNode, OpenApiDocument>>
    {
    }
}
