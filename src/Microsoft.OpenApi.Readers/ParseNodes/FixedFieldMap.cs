// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;

namespace Microsoft.OpenApi.Readers.ParseNodes
{
    internal class FixedFieldMap<T> : Dictionary<string, Action<T, ParseNode>>
    {
    }
}