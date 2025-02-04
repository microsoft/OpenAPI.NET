using System;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models.References;
/// <summary>
/// Base class for OpenApiReferenceHolder.
/// </summary>
/// <typeparam name="T">The concrete class implementation type for the model.</typeparam>
/// <typeparam name="V">The interface type for the model.</typeparam>
public abstract class BaseOpenApiReferenceHolder<T, V> : IOpenApiReferenceHolder<T, V> where T : class, IOpenApiReferenceable, V where V : IOpenApiSerializable
{
    /// <summary>
    /// The resolved target object. This should remain readonly, otherwise mutating the reference will have side effects.
    /// </summary>
    protected readonly T _target;
    /// <inheritdoc/>
    public virtual T Target
    {
        get
        {
            if (_target is not null) return _target;
            return Reference.HostDocument?.ResolveReferenceTo<T>(Reference);
        }
    }
    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="source">The parameter reference to copy</param>
    protected BaseOpenApiReferenceHolder(BaseOpenApiReferenceHolder<T, V> source)
    {
        Utils.CheckArgumentNull(source);
        Reference = source.Reference != null ? new(source.Reference) : null;
        //no need to copy summary and description as if they are not overridden, they will be fetched from the target
        //if they are, the reference copy will handle it
    }
    private protected BaseOpenApiReferenceHolder(T target, string referenceId, ReferenceType referenceType)
    {
        Utils.CheckArgumentNull(target);
        _target = target;

        Reference = new OpenApiReference()
        {
            Id = referenceId,
            Type = referenceType,
        };
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
    protected BaseOpenApiReferenceHolder(string referenceId, OpenApiDocument hostDocument, ReferenceType referenceType, string externalResource = null)
    {
        Utils.CheckArgumentNullOrEmpty(referenceId);

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
    /// <inheritdoc/>
    public OpenApiReference Reference { get; init; }
    /// <inheritdoc/>
    public abstract V CopyReferenceAsTargetElementWithOverrides(V source);
    /// <inheritdoc/>
    public virtual void SerializeAsV3(IOpenApiWriter writer)
    {
        if (!writer.GetSettings().ShouldInlineReference(Reference))
        {
            Reference.SerializeAsV3(writer);
        }
        else
        {
            SerializeInternal(writer, (writer, element) => element.SerializeAsV3(writer));
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
            SerializeInternal(writer, (writer, element) => element.SerializeAsV2(writer));
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
        action(writer, Target);
    }
}
