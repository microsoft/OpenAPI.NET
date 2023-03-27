// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.ComponentModel;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Attributes;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// The reference type.
    /// </summary>
    [JsonConverter(typeof(Json.More.EnumStringConverter<ReferenceType>))]
    public enum ReferenceType
    {
        /// <summary>
        /// Schema item.
        /// </summary>
        [Description("schemas")] Schema,

        /// <summary>
        /// Responses item.
        /// </summary>
        [Description("responses")] Response,

        /// <summary>
        /// Parameters item.
        /// </summary>
        [Description("parameters")] Parameter,

        /// <summary>
        /// Examples item.
        /// </summary>
        [Description("examples")] Example,

        /// <summary>
        /// RequestBodies item.
        /// </summary>
        [Description("requestBodies")] RequestBody,

        /// <summary>
        /// Headers item.
        /// </summary>
        [Description("headers")] Header,

        /// <summary>
        /// SecuritySchemes item.
        /// </summary>
        [Description("securitySchemes")] SecurityScheme,

        /// <summary>
        /// Links item.
        /// </summary>
        [Description("links")] Link,

        /// <summary>
        /// Callbacks item.
        /// </summary>
        [Description("callbacks")] Callback,

        /// <summary>
        /// Tags item.
        /// </summary>
        [Description("tags")] Tag,

        /// <summary>
        /// Path item.
        /// </summary>
        [Description("pathItems")] PathItem
    }
}
