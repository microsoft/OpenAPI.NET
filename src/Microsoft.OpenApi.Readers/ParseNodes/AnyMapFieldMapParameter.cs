// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using Json.Schema;
using Microsoft.OpenApi.Any;

namespace Microsoft.OpenApi.Readers.ParseNodes
{
    internal class AnyMapFieldMapParameter<T, U>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public AnyMapFieldMapParameter(
            Func<T, IDictionary<string, U>> propertyMapGetter,
            Func<U, OpenApiAny> propertyGetter,
            Action<U, OpenApiAny> propertySetter,
            Func<T, JsonSchema> schemaGetter)
        {
            this.PropertyMapGetter = propertyMapGetter;
            this.PropertyGetter = propertyGetter;
            this.PropertySetter = propertySetter;
            this.SchemaGetter = schemaGetter;
        }

        /// <summary>
        /// Function to retrieve the property that is a map from string to an inner element containing IOpenApiAny.
        /// </summary>
        public Func<T, IDictionary<string, U>> PropertyMapGetter { get; }

        /// <summary>
        /// Function to retrieve the value of the property from an inner element.
        /// </summary>
        public Func<U, OpenApiAny> PropertyGetter { get; }

        /// <summary>
        /// Function to set the value of the property.
        /// </summary>
        public Action<U, OpenApiAny> PropertySetter { get; }

        /// <summary>
        /// Function to get the schema to apply to the property.
        /// </summary>
        public Func<T, JsonSchema> SchemaGetter { get; }
    }
}
