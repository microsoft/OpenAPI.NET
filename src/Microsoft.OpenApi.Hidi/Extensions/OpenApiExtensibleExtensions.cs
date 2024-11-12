using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using System.Collections.Generic;

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
            if (extensions.TryGetValue(extensionKey, out var value) && value is OpenApiAny castValue)
            {
                return castValue.Node.GetValue<string>();
            }
            return string.Empty;
        }
    }
}
