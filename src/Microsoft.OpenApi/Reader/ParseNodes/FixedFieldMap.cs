﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;

namespace Microsoft.OpenApi.Reader
{
    internal class FixedFieldMap<T> : Dictionary<string, Action<T, ParseNode, OpenApiDocument>>
    {
    }
}
