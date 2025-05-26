using System;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi;
/// <summary>
/// Base class for OpenApiReferenceHolder.
/// </summary>
/// <typeparam name="T">The concrete class implementation type for the model.</typeparam>
/// <typeparam name="V">The interface type for the model.</typeparam>
public abstract class BaseOpenApiReferenceHolder<T, V> : IOpenApiReferenceHolder<T, V> where T : class, IOpenApiReferenceable, V where V : IOpenApiReferenceable, IOpenApiSerializable
{
    /// <inheritdoc/>
    public virtual V? Target
    {
        get
        {
            if (Reference.HostDocument is null) return default;
            return Reference.HostDocument.ResolveReferenceTo<V>(Reference);
        }
    }
    /// <inheritdoc/>
    public T? RecursiveTarget
    {
        get
        {
            return Target switch {
                BaseOpenApiReferenceHolder<T, V> recursiveTarget => recursiveTarget.RecursiveTarget,
                T concrete => concrete,
                _ => null
            };
        }
    }

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="source">The parameter reference to copy</param>
    protected BaseOpenApiReferenceHolder(BaseOpenApiReferenceHolder<T, V> source)
    {
        Utils.CheckArgumentNull(source);
        Reference = new(source.Reference);
        //no need to copy summary and description as if they are not overridden, they will be fetched from the target
        //if they are, the reference copy will handle it
    }
    /// <summary>
    /// Constructor initializing the reference object.
    /// </summary>
    /// <param name="referenceId">The reference Id.</param>
    /// <param name="hostDocument">The host OpenAPI document.</param>
    /// <param name="referenceType">The reference type.</param>
    /// <param name="externalResource">Optional: External resource in the reference.
    /// It may be:
    /// 1. a absolute/relative file path, for example:  ../commons/pet.json
    /// 2. a Url, for example: http://localhost/pet.json
    /// </param>
    protected BaseOpenApiReferenceHolder(string referenceId, OpenApiDocument? hostDocument, ReferenceType referenceType, string? externalResource)
    {
        Utils.CheckArgumentNullOrEmpty(referenceId);
        // we're not checking for null hostDocument as it's optional and can be set via additional methods by a walker
        // this way object initialization of a whole document is supported

        Reference = new OpenApiReference()
        {
            Id = referenceId,
            HostDocument = hostDocument,
            Type = referenceType,
            ExternalResource = externalResource
        };
    }
    /// <inheritdoc/>
    public bool UnresolvedReference { get => Reference is null || Target is null; }

#if NETSTANDARD2_1_OR_GREATER
    /// <inheritdoc/>
    public required OpenApiReference Reference { get; init; }
#else
    /// <inheritdoc/>
    public OpenApiReference Reference { get; init; }
#endif
    /// <inheritdoc/>
    public abstract V CopyReferenceAsTargetElementWithOverrides(V source);
    /// <inheritdoc/>
    public virtual void SerializeAsV3(IOpenApiWriter writer)
    {
        if (!writer.GetSettings().ShouldInlineReference(Reference) 
            || Reference.Type == ReferenceType.Tag) // tags are held as references need to drop in.
        {
            Reference.SerializeAsV3(writer);
        }
        else
        {
            SerializeInternal(writer, (writer, element) => element?.SerializeAsV3(writer));
        }
    }

    /// <inheritdoc/>
    public virtual void SerializeAsV31(IOpenApiWriter writer)
    {
        if (!writer.GetSettings().ShouldInlineReference(Reference))
        {
            Reference.SerializeAsV31(writer);
        }
        else
        {
            SerializeInternal(writer, (writer, element) => CopyReferenceAsTargetElementWithOverrides(element).SerializeAsV31(writer));
        }
    }

    /// <inheritdoc/>
    public virtual void SerializeAsV2(IOpenApiWriter writer)
    {
        if (!writer.GetSettings().ShouldInlineReference(Reference))
        {
            Reference.SerializeAsV2(writer);
        }
        else
        {
            SerializeInternal(writer, (writer, element) => element?.SerializeAsV2(writer));
        }
    }

    /// <summary>
    /// Serialize the reference as a reference or the target object.
    /// This method is used to accelerate the serialization methods implementations.
    /// </summary>
    /// <param name="writer">The OpenApiWriter.</param>
    /// <param name="action">The action to serialize the target object.</param>
    private protected void SerializeInternal(IOpenApiWriter writer,
        Action<IOpenApiWriter, V> action)
    {
        Utils.CheckArgumentNull(writer);
        if (Target is not null)
        {
            action(writer, Target);
        }
    }
}
