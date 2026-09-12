using System;

namespace Microsoft.OpenApi.YamlReader;

/// <summary>
/// Configures resource limits for an <see cref="OpenApiYamlReader"/>.
/// </summary>
public sealed class OpenApiYamlReaderSettings
{
    /// <summary>
    /// Default maximum number of input bytes read from a single YAML document (128 MiB).
    /// Bounds the buffered copy of a non-seekable stream, so an endless or oversized response body
    /// cannot exhaust memory before parsing begins.
    /// </summary>
    public const uint DefaultMaxInputByteCount = 128 * 1024 * 1024;

    /// <summary>
    /// Default maximum length of a single YAML scalar value (65,536 UTF-16 code units).
    /// Bounds the cost of any one key, string, number, date or block literal. For reference, the
    /// longest scalar in the Microsoft Graph beta description is 1,833 code units, so this leaves
    /// substantial headroom for legitimate documents.
    /// </summary>
    public const uint DefaultMaxScalarLength = 64 * 1024;

    /// <summary>
    /// Gets or sets the maximum YAML nesting depth.
    /// Defaults to <see cref="YamlConverter.DefaultMaxDepth"/> and cannot exceed
    /// <see cref="YamlConverter.MaximumAllowedDepth"/>.
    /// </summary>
    public uint MaxDepth { get; set; } = YamlConverter.DefaultMaxDepth;

    /// <summary>
    /// Gets or sets the maximum number of JSON nodes materialized from one YAML document.
    /// Defaults to <see cref="YamlConverter.DefaultMaxNodeCount"/> and cannot exceed
    /// <see cref="YamlConverter.MaximumAllowedNodeCount"/>.
    /// </summary>
    public uint MaxNodeCount { get; set; } = YamlConverter.DefaultMaxNodeCount;

    /// <summary>
    /// Gets or sets the maximum number of JSON nodes materialized specifically from aliases.
    /// Defaults to <see cref="YamlConverter.DefaultMaxAliasExpansionNodeCount"/>.
    /// </summary>
    public uint MaxAliasExpansionNodeCount { get; set; } = YamlConverter.DefaultMaxAliasExpansionNodeCount;

    /// <summary>
    /// Gets or sets the maximum number of input bytes read from one YAML document.
    /// Defaults to <see cref="DefaultMaxInputByteCount"/>.
    /// </summary>
    public uint MaxInputByteCount { get; set; } = DefaultMaxInputByteCount;

    /// <summary>
    /// Gets or sets the maximum length of one YAML scalar value.
    /// Defaults to <see cref="DefaultMaxScalarLength"/>.
    /// </summary>
    public uint MaxScalarLength { get; set; } = DefaultMaxScalarLength;

    internal void Validate()
    {
        YamlConverter.ValidateMaxDepth(MaxDepth, nameof(MaxDepth));
        YamlConverter.ValidateMaxNodeCount(MaxNodeCount, nameof(MaxNodeCount));
        ValidatePositive(MaxAliasExpansionNodeCount, nameof(MaxAliasExpansionNodeCount));
        ValidatePositive(MaxInputByteCount, nameof(MaxInputByteCount));
        ValidatePositive(MaxScalarLength, nameof(MaxScalarLength));
    }

    private static void ValidatePositive(uint value, string parameterName)
    {
        if (value == 0)
        {
            throw new ArgumentOutOfRangeException(parameterName, $"{parameterName} must be greater than zero.");
        }
    }
}
