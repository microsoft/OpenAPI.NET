
using System.Collections.Generic;
using Microsoft.OpenApi.Expressions;
using Microsoft.OpenApi.Interfaces;

namespace Microsoft.OpenApi.Models.Interfaces;

/// <summary>
/// Defines the base properties for the callback object.
/// This interface is provided for type assertions but should not be implemented by package consumers beyond automatic mocking.
/// </summary>
public interface IOpenApiCallback : IOpenApiReadOnlyExtensible, IShallowCopyable<IOpenApiCallback>, IOpenApiReferenceable
{
    /// <summary>
    /// A Path Item Object used to define a callback request and expected responses.
    /// </summary>
    public OrderedDictionary<RuntimeExpression, IOpenApiPathItem>? PathItems { get; }
}
