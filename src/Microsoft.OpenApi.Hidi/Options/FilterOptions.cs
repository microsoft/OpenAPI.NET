// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

namespace Microsoft.OpenApi.Hidi.Options
{
    internal class FilterOptions
    {
        public string? FilterByOperationIds { get; internal set; }
        public string? FilterByTags { get; internal set; }
        public string? FilterByCollection { get; internal set; }
        public string? FilterByApiManifest { get; internal set; }
    }
}
