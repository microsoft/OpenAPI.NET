// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.ComponentModel;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Attributes;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Operation type.
    /// </summary>
    [JsonConverter(typeof(Json.More.EnumStringConverter<OperationType>))]
    public enum OperationType
    {
        /// <summary>
        /// A definition of a GET operation on this path.
        /// </summary>
        [Description("get")] Get,

        /// <summary>
        /// A definition of a PUT operation on this path.
        /// </summary>
        [Description("put")] Put,

        /// <summary>
        /// A definition of a POST operation on this path.
        /// </summary>
        [Description("post")] Post,

        /// <summary>
        /// A definition of a DELETE operation on this path.
        /// </summary>
        [Description("delete")] Delete,

        /// <summary>
        /// A definition of a OPTIONS operation on this path.
        /// </summary>
        [Description("options")] Options,

        /// <summary>
        /// A definition of a HEAD operation on this path.
        /// </summary>
        [Description("head")] Head,

        /// <summary>
        /// A definition of a PATCH operation on this path.
        /// </summary>
        [Description("patch")] Patch,

        /// <summary>
        /// A definition of a TRACE operation on this path.
        /// </summary>
        [Description("trace")] Trace
    }
}
