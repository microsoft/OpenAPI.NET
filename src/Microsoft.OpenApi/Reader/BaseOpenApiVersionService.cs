// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;

namespace Microsoft.OpenApi.Reader;

internal abstract class BaseOpenApiVersionService : IOpenApiVersionService
{
    public OpenApiDiagnostic Diagnostic { get; }

    protected BaseOpenApiVersionService(OpenApiDiagnostic diagnostic)
    {
        Diagnostic = diagnostic;
    }

    internal abstract Dictionary<Type, Func<JsonNode, OpenApiDocument, ParsingContext, object?>> Loaders { get; }

    public abstract OpenApiDocument LoadDocument(JsonNode JsonNode, Uri location, ParsingContext context);

    public T? LoadElement<T>(JsonNode node, OpenApiDocument doc, ParsingContext context) where T : IOpenApiElement
    {
        if (Loaders.TryGetValue(typeof(T), out var loader) && loader(node, doc, context) is T result)
        {
            return result;
        }
        return default;
    }
    public virtual string? GetReferenceScalarValues(JsonObject JsonObject, string scalarValue)
    {
        if (JsonObject.Any(static x => !"$ref".Equals(x.Key, StringComparison.OrdinalIgnoreCase)) &&
            JsonObject
            .Where(x => x.Key.Equals(scalarValue))
            .Select(static x => x.Value)
            .OfType<JsonValue>().FirstOrDefault() is {} JsonNode)
        {
            return JsonNode.GetScalarValue();
        }

        return null;
    } 
}
