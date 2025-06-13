// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OpenApi.Reader;

internal abstract class BaseOpenApiVersionService : IOpenApiVersionService
{
    public OpenApiDiagnostic Diagnostic { get; }

    protected BaseOpenApiVersionService(OpenApiDiagnostic diagnostic)
    {
        Diagnostic = diagnostic;
    }

    internal abstract Dictionary<Type, Func<ParseNode, OpenApiDocument, object?>> Loaders { get; }

    public abstract OpenApiDocument LoadDocument(RootNode rootNode, Uri location);

    public T? LoadElement<T>(ParseNode node, OpenApiDocument doc) where T : IOpenApiElement
    {
        if (Loaders.TryGetValue(typeof(T), out var loader) && loader(node, doc) is T result)
        {
            return result;
        }
        return default;
    }
    public virtual string? GetReferenceScalarValues(MapNode mapNode, string scalarValue)
    {
        if (mapNode.Any(static x => !"$ref".Equals(x.Name, StringComparison.OrdinalIgnoreCase)) &&
            mapNode
            .Where(x => x.Name.Equals(scalarValue))
            .Select(static x => x.Value)
            .OfType<ValueNode>().FirstOrDefault() is {} valueNode)
        {
            return valueNode.GetScalarValue();
        }

        return null;
    } 
}
