// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi.Reader;

/// <summary>
/// Selects the version service used to parse JSON Schema dialects.
/// </summary>
internal sealed class JsonSchemaDialectService
{
    internal static readonly Uri OpenApiBaseDialect = new("https://spec.openapis.org/oas/3.1/dialect/base");
    internal static readonly Uri JsonSchemaDraft202012 = new("https://json-schema.org/draft/2020-12/schema");

    private readonly Uri? _documentDialect;
    private readonly IOpenApiVersionService _fallbackVersionService;
    private readonly OpenApiDiagnostic _diagnostic;
    private readonly Dictionary<string, IOpenApiVersionService> _versionServices = new(StringComparer.Ordinal);
    private readonly Dictionary<string, Uri> _unsupportedDialects = new(StringComparer.Ordinal);
    private readonly List<(Uri Dialect, OpenApiError Error)> _unsupportedDialectErrors = new();
    private readonly HashSet<string> _reportedDocumentDialects = new(StringComparer.Ordinal);
    private readonly Stack<Uri> _dialectScopes = new();

    internal JsonSchemaDialectService(
        IOpenApiVersionService versionService,
        Uri? documentDialect,
        OpenApiDiagnostic diagnostic)
    {
        _fallbackVersionService = versionService;
        _documentDialect = documentDialect;
        _diagnostic = diagnostic;

        RegisterDialect(OpenApiBaseDialect, versionService);
        RegisterDialect(JsonSchemaDraft202012, versionService);
    }

    internal IReadOnlyCollection<Uri> UnsupportedDialects => _unsupportedDialects.Values;
    internal IReadOnlyCollection<(Uri Dialect, OpenApiError Error)> UnsupportedDialectErrors => _unsupportedDialectErrors;
    internal bool IsUsingFallback => _dialectScopes.Count > 0 &&
        !_versionServices.ContainsKey(GetDialectKey(_dialectScopes.Peek()));

    internal IOpenApiSchema LoadSchema(JsonNode node, OpenApiDocument hostDocument, ParsingContext context)
    {
        var declaredDialect = GetDeclaredDialect(node);
        var inheritedDialect = _dialectScopes.Count > 0 ? _dialectScopes.Peek() : null;
        var effectiveDialect = declaredDialect ?? inheritedDialect ?? _documentDialect ?? OpenApiBaseDialect;

        if (!_versionServices.TryGetValue(GetDialectKey(effectiveDialect), out var versionService))
        {
            versionService = _fallbackVersionService;
            _unsupportedDialects[GetDialectKey(effectiveDialect)] = effectiveDialect;

            // Report the point where an unsupported dialect enters the schema tree. Descendants
            // inherit that result and do not need to repeat the same diagnostic.
            if (declaredDialect is not null)
            {
                AddUnsupportedDialectError(effectiveDialect, context.GetLocation());
            }
            else if (inheritedDialect is null && _documentDialect is not null &&
                     _reportedDocumentDialects.Add(GetDialectKey(effectiveDialect)))
            {
                AddUnsupportedDialectError(effectiveDialect, $"#/{OpenApiConstants.JsonSchemaDialect}");
            }
        }

        _dialectScopes.Push(effectiveDialect);
        try
        {
            return versionService.LoadSchema(node, hostDocument, context);
        }
        finally
        {
            _dialectScopes.Pop();
        }
    }

    private void AddUnsupportedDialectError(Uri dialect, string pointer)
    {
        var error = new OpenApiError(pointer, $"The JSON Schema dialect '{dialect}' is not supported.");
        _diagnostic.Errors.Add(error);
        _unsupportedDialectErrors.Add((dialect, error));
    }

    internal static bool IsKnownDialect(Uri dialect)
    {
        var key = GetDialectKey(dialect);
        return string.Equals(key, GetDialectKey(OpenApiBaseDialect), StringComparison.Ordinal) ||
               string.Equals(key, GetDialectKey(JsonSchemaDraft202012), StringComparison.Ordinal);
    }

    private void RegisterDialect(Uri dialect, IOpenApiVersionService versionService)
    {
        _versionServices[GetDialectKey(dialect)] = versionService;
    }

    private static Uri? GetDeclaredDialect(JsonNode node)
    {
        if (node is not JsonObject jsonObject ||
            !jsonObject.TryGetPropertyValue(OpenApiConstants.DollarSchema, out var dialectNode) ||
            dialectNode is not JsonValue)
        {
            return null;
        }

        var value = dialectNode.GetScalarValue();
        return Uri.TryCreate(value, UriKind.Absolute, out var dialect) ? dialect : null;
    }

    private static string GetDialectKey(Uri dialect)
    {
        var value = dialect.AbsoluteUri;
        return value.EndsWith("#", StringComparison.Ordinal) ? value.Substring(0, value.Length - 1) : value;
    }
}
