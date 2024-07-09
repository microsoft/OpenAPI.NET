// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Linq;
using System.Reflection;
using Microsoft.OpenApi.Attributes;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Extensions
{
    /// <summary>
    /// Enumeration type extension methods.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Gets an attribute on an enum field value.
        /// </summary>
        /// <typeparam name="T">The type of the attribute to retrieve.</typeparam>
        /// <param name="enumValue">The enum value.</param>
        /// <returns>
        /// The attribute of the specified type or null.
        /// </returns>
        public static T GetAttributeOfType<T>(this Enum enumValue) where T : Attribute
        {
            var type = enumValue.GetType();
            var memInfo = type.GetMember(enumValue.ToString()).First();
            var attributes = memInfo.GetCustomAttributes<T>(false);
            return attributes.FirstOrDefault();
        }

        /// <summary>
        /// Gets the enum display name.
        /// </summary>
        /// <param name="enumValue">The enum value.</param>
        /// <returns>
        /// Use <see cref="DisplayAttribute"/> if exists.
        /// Otherwise, use the standard string representation.
        /// </returns>
        [Obsolete("Use native AoT-friendly type-specific overloads GetDisplayName methods instead.")]
        public static string GetDisplayName(this Enum enumValue)
        {
            var attribute = enumValue.GetAttributeOfType<DisplayAttribute>();
            return attribute == null ? enumValue.ToString() : attribute.Name;
        }

        /// <summary>
        /// Gets the enum display for name <see cref="ParameterStyle" /> without the use of reflection.
        /// </summary>
        /// <param name="parameterStyle">The enum value.</param>
        /// <returns>The display string to use.</returns>
        internal static string GetDisplayName(this ParameterStyle parameterStyle) => parameterStyle switch
            {
                ParameterStyle.Matrix => "matrix",
                ParameterStyle.Label => "label",
                ParameterStyle.Form => "form",
                ParameterStyle.Simple => "simple",
                ParameterStyle.SpaceDelimited => "spaceDelimited",
                ParameterStyle.PipeDelimited => "pipeDelimited",
                ParameterStyle.DeepObject => "deepObject",
                _ => throw new InvalidOperationException($"Unknown parameter style: {parameterStyle}")
            };

        /// <summary>
        /// Gets the enum display for name <see cref="ParameterLocation" /> without the use of reflection. 
        /// </summary>
        /// <param name="parameterLocation">The enum value.</param>
        /// <returns>The display string to use.</returns>
        public static string GetDisplayName(this ParameterLocation parameterLocation) => parameterLocation switch
            {
                ParameterLocation.Query => "query",
                ParameterLocation.Header => "header",
                ParameterLocation.Path => "path",
                ParameterLocation.Cookie => "cookie",
                _ => throw new InvalidOperationException($"Unknown parameter location: {parameterLocation}")
            };

        /// <summary>
        /// Gets the enum display for name <see cref="ReferenceType" /> without the use of reflection. 
        /// </summary>
        /// <param name="referenceType">The enum value.</param>
        /// <returns>The display string to use.</returns>
        internal static string GetDisplayName(this ReferenceType referenceType) => referenceType switch
            {
                ReferenceType.Schema => "schemas",
                ReferenceType.Response => "responses",
                ReferenceType.Parameter => "parameters",
                ReferenceType.Example => "examples",
                ReferenceType.RequestBody => "requestBodies",
                ReferenceType.Header => "headers",
                ReferenceType.SecurityScheme => "securitySchemes",
                ReferenceType.Link => "links",
                ReferenceType.Callback => "callbacks",
                ReferenceType.Tag => "tags",
                ReferenceType.Path => "path",
                _ => throw new InvalidOperationException($"Unknown reference type: {referenceType}")
            };

        /// <summary>
        /// Gets the enum display for name <see cref="OperationType" /> without the use of reflection. 
        /// </summary>
        /// <param name="operationType">The enum value.</param>
        /// <returns>The display string to use.</returns>
        internal static string GetDisplayName(this OperationType operationType) => operationType switch
            {
                OperationType.Get => "get",
                OperationType.Put => "put",
                OperationType.Post => "post",
                OperationType.Delete => "delete",
                OperationType.Options => "options",
                OperationType.Head => "head",
                OperationType.Patch => "patch",
                OperationType.Trace => "trace",
                _ => throw new InvalidOperationException($"Unknown operation type: {operationType}")
            };

        /// <summary>
        /// Gets the enum display for name <see cref="SecuritySchemeType" /> without the use of reflection. 
        /// </summary>
        /// <param name="securitySchemeType">The enum value.</param>
        /// <returns>The display string to use.</returns>
        internal static string GetDisplayName(this SecuritySchemeType securitySchemeType) => securitySchemeType switch
            {
                SecuritySchemeType.ApiKey => "apiKey",
                SecuritySchemeType.Http => "http",
                SecuritySchemeType.OAuth2 => "oauth2",
                SecuritySchemeType.OpenIdConnect => "openIdConnect",
                _ => throw new InvalidOperationException($"Unknown security scheme type: {securitySchemeType}")
            };
    }
}
