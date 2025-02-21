using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi.Hidi.Extensions
{
    internal static class OpenApiExtensibleExtensions
    {
        /// <summary>
        /// Gets an extension value from the extensions dictionary.
        /// </summary>
        /// <param name="extensions">A dictionary of <see cref="IOpenApiExtension"/>.</param>
        /// <param name="extensionKey">The key corresponding to the <see cref="IOpenApiExtension"/>.</param>
        /// <returns>A <see cref="string"/> value matching the provided extensionKey. Return null when extensionKey is not found. </returns>
        internal static string GetExtension(this IDictionary<string, IOpenApiExtension> extensions, string extensionKey)
        {
            if (extensions.TryGetValue(extensionKey, out var value) && value is OpenApiAny { Node: JsonValue castValue } && castValue.TryGetValue<string>(out var stringValue))
            {
                return stringValue;
            }
            return string.Empty;
        }
    }
}
