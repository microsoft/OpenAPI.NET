// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Text.Json.Nodes;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Readers.ParseNodes
{
    internal class AnyFieldMapParameter<T>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public AnyFieldMapParameter(
            Func<T, JsonNode> propertyGetter,
            Action<T, JsonNode> propertySetter,
            Func<T, OpenApiSchema> schemaGetter)
        {
            this.PropertyGetter = propertyGetter;
            this.PropertySetter = propertySetter;
            this.SchemaGetter = schemaGetter;
        }

        /// <summary>
        /// Function to retrieve the value of the property.
        /// </summary>
        public Func<T, JsonNode> PropertyGetter { get; }

        /// <summary>
        /// Function to set the value of the property.
        /// </summary>
        public Action<T, JsonNode> PropertySetter { get; }

        /// <summary>
        /// Function to get the schema to apply to the property.
        /// </summary>
        public Func<T, OpenApiSchema> SchemaGetter { get; }
    }
}
