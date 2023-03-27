// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.ComponentModel;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Attributes;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// The style of the parameter.
    /// </summary>
    [JsonConverter(typeof(Json.More.EnumStringConverter<ParameterStyle>))]
    public enum ParameterStyle
    {
        /// <summary>
        /// Path-style parameters.
        /// </summary>
        [Description("matrix")] Matrix,

        /// <summary>
        /// Label style parameters.
        /// </summary>
        [Description("label")] Label,

        /// <summary>
        /// Form style parameters.
        /// </summary>
        [Description("form")] Form,

        /// <summary>
        /// Simple style parameters.
        /// </summary>
        [Description("simple")] Simple,

        /// <summary>
        /// Space separated array values.
        /// </summary>
        [Description("spaceDelimited")] SpaceDelimited,

        /// <summary>
        /// Pipe separated array values.
        /// </summary>
        [Description("pipeDelimited")] PipeDelimited,

        /// <summary>
        /// Provides a simple way of rendering nested objects using form parameters.
        /// </summary>
        [Description("deepObject")] DeepObject
    }
}
