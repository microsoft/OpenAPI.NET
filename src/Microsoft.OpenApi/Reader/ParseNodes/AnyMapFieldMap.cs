// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;

namespace Microsoft.OpenApi.Reader.ParseNodes
{
    internal class AnyMapFieldMap<T, U> : OrderedDictionary<string, AnyMapFieldMapParameter<T, U>>
    {
    }
}
