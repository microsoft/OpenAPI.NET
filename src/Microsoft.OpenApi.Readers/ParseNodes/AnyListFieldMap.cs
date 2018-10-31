// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Readers.ParseNodes
{
    internal class AnyListFieldMap<T> : Dictionary<string, AnyListFieldMapParameter<T>>
    {

    }

    internal class AnyListFieldMapParameter<T>
    {
        public AnyListFieldMapParameter(
            Func<T, IList<IOpenApiAny>> propertyGetter,
            Func<T, IList<IOpenApiAny>, IList<IOpenApiAny>> propertySetter,
            Func<T, OpenApiSchema> schemaGetter)
        {
            this.PropertyGetter = propertyGetter;
            this.PropertySetter = propertySetter;
            this.SchemaGetter = schemaGetter;
        }

        public Func<T, IList<IOpenApiAny>> PropertyGetter { get; }

        public Func<T, IList<IOpenApiAny>, IList<IOpenApiAny>> PropertySetter { get; }

        public Func<T, OpenApiSchema> SchemaGetter { get; }
    }
}