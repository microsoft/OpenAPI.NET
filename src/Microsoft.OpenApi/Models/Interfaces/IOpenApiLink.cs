using System.Collections.Generic;

namespace Microsoft.OpenApi;

/// <summary>
/// Defines the base properties for the link object.
/// This interface is provided for type assertions but should not be implemented by package consumers beyond automatic mocking.
/// </summary>
public interface IOpenApiLink : IOpenApiDescribedElement, IOpenApiReadOnlyExtensible, IShallowCopyable<IOpenApiLink>, IOpenApiReferenceable
{
    /// <summary>
    /// A relative or absolute reference to an OAS operation.
    /// This field is mutually exclusive of the operationId field, and MUST point to an Operation Object.
    /// </summary>
    public string? OperationRef { get; }

    /// <summary>
    /// The name of an existing, resolvable OAS operation, as defined with a unique operationId.
    /// This field is mutually exclusive of the operationRef field.
    /// </summary>
    public string? OperationId { get; }

    /// <summary>
    /// A map representing parameters to pass to an operation as specified with operationId or identified via operationRef.
    /// </summary>
    public Dictionary<string, RuntimeExpressionAnyWrapper>? Parameters { get; }

    /// <summary>
    /// A literal value or {expression} to use as a request body when calling the target operation.
    /// </summary>
    public RuntimeExpressionAnyWrapper? RequestBody { get; }
    /// <summary>
    /// A server object to be used by the target operation.
    /// </summary>
    public OpenApiServer? Server { get; }
}
