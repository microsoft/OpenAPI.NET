using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Properties;

namespace Microsoft.OpenApi.Extensions;

/// <summary>
/// Extension methods for <see cref="OpenApiServer"/> serialization.
/// </summary>
public static class OpenApiServerExtensions
{
    /// <summary>
    /// Replaces URL variables in a server's URL
    /// </summary>
    /// <param name="server">The OpenAPI server object</param>
    /// <param name="values">The server variable values that will be used to replace the default values.</param>
    /// <returns>A URL with the provided variables substituted.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when:
    ///   1. A substitution has no valid value in both the supplied dictionary and the default
    ///   2. A substitution's value is not available in the enum provided
    /// </exception>
    public static string ReplaceServerUrlVariables(this OpenApiServer server, IDictionary<string, string> values)
    {
        var parsedUrl = server.Url;
        foreach (var variable in server.Variables)
        {
            // Try to get the value from the passed in values
            values.TryGetValue(variable.Key, out var actualValue);
            // Fall back to the default value
            if (string.IsNullOrEmpty(actualValue)) { actualValue = variable.Value.Default; }

            if (string.IsNullOrEmpty(actualValue))
            {
                // According to the spec, the variable's default value is required.
                // This code path should be hit when a value isn't provided & a default value isn't available
                throw new ArgumentException(
                    string.Format(SRResource.ParseServerUrlDefaultValueNotAvailable, variable.Key), nameof(server));
            }

            if (variable.Value.Enum is { Count: > 0 } e && !e.Contains(actualValue))
            {
                throw new ArgumentException(
                    string.Format(SRResource.ParseServerUrlValueNotValid, actualValue, variable.Key), nameof(values));
            }

            parsedUrl = parsedUrl.Replace($"{{{variable.Key}}}", actualValue);
        }

        return parsedUrl;
    }
}
