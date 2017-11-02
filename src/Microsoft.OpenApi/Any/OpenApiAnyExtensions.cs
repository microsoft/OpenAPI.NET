//---------------------------------------------------------------------
// <copyright file="OpenApiAnyExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OpenApi
{
    /// <summary>
    /// Extension methods covert <see cref="System.String"/> to <see cref="IOpenApiAny"/>
    /// </summary>
    internal static class OpenApiAnyExtensions
    {
        public static IOpenApiAny CreateOpenApiAny(this string input)
        {
            if (input == null)
            {
                return new OpenApiNull();
            }

            // TODO: add the logics


            // If we can't distiguish the type, just return it as string.
            return new OpenApiString(input);
        }
    }
}
