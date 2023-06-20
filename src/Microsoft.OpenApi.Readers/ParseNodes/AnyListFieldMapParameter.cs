// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using Json.Schema;

namespace Microsoft.OpenApi.Readers.ParseNodes
{
    internal class AnyListFieldMapParameter<T>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public AnyListFieldMapParameter(
            Func<T, IList<JsonNode>> propertyGetter,
            Action<T, IList<JsonNode>> propertySetter,
            Func<T, JsonSchema> schema31Getter = null)
        {
            this.PropertyGetter = propertyGetter;
            this.PropertySetter = propertySetter;
            this.Schema31Getter = schema31Getter;
        }

        /// <summary>
        /// Function to retrieve the value of the property.
        /// </summary>
        public Func<T, IList<JsonNode>> PropertyGetter { get; }

        /// <summary>
        /// Function to set the value of the property.
        /// </summary>
        public Action<T, IList<JsonNode>> PropertySetter { get; }

        /// <summary>
        /// Function to get the schema to apply to the property.
        /// </summary>
        public Func<T, JsonSchema> Schema31Getter { get; }
    }
}
