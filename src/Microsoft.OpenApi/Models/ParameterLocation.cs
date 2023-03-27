// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.ComponentModel;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Attributes;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// The location of the parameter.
    /// </summary>
    [JsonConverter(typeof(Json.More.EnumStringConverter<ParameterLocation>))]
    public enum ParameterLocation
    {
        /// <summary>
        /// Parameters that are appended to the URL.
        /// </summary>
        [Description("query")] Query,

        /// <summary>
        /// Custom headers that are expected as part of the request.
        /// </summary>
        [Description("header")] Header,

        /// <summary>
        /// Used together with Path Templating,
        /// where the parameter value is actually part of the operation's URL
        /// </summary>
        [Description("path")] Path,

        /// <summary>
        /// Used to pass a specific cookie value to the API.
        /// </summary>
        [Description("cookie")] Cookie
    }
}
