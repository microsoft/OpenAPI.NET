// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;

namespace Microsoft.OpenApi.Reader
{
    internal class AnyFieldMap<T> : Dictionary<string, AnyFieldMapParameter<T>>
    {
    }
}
