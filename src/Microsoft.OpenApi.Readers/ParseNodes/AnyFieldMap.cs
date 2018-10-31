// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Readers.ParseNodes
{
    internal class AnyFieldMap<T> : Dictionary<string, AnyFieldMapParameter<T>>
    {

    }

    internal class AnyFieldMapParameter<T>
    {
        public AnyFieldMapParameter(
            Func<T, IOpenApiAny> propertyGetter,
            Func<T, IOpenApiAny, IOpenApiAny> propertySetter,
            Func<T, OpenApiSchema> schemaGetter)
        {
            this.PropertyGetter = propertyGetter;
            this.PropertySetter = propertySetter;
            this.SchemaGetter = schemaGetter;
        }

        public Func<T, IOpenApiAny> PropertyGetter { get; }

        public Func<T, IOpenApiAny, IOpenApiAny> PropertySetter { get; }

        public Func<T, OpenApiSchema> SchemaGetter { get; }
    }
}