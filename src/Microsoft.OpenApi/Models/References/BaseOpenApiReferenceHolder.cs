using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.OpenApi;
/// <summary>
/// Base class for OpenApiReferenceHolder.
/// </summary>
/// <typeparam name="T">The concrete class implementation type for the model.</typeparam>
/// <typeparam name="U">The interface type for the model.</typeparam>
/// <typeparam name="V">The type for the reference holding the additional fields and annotations</typeparam>
public abstract class BaseOpenApiReferenceHolder<T, U, V> : IOpenApiReferenceHolder<T, U, V> where T : class, IOpenApiReferenceable, U where U : IOpenApiReferenceable, IOpenApiSerializable where V : BaseOpenApiReference, new()
{
    [ThreadStatic]
    private static HashSet<BaseOpenApiReferenceHolder<T, U, V>>? t_activeReferenceAccesses;
    [ThreadStatic]
    private static HashSet<BaseOpenApiReferenceHolder<T, U, V>>? t_activeTargetActions;

    /// <inheritdoc/>
    public virtual U? Target
    {
        get
        {
            if (Reference.HostDocument is null) return default;
            return Reference.HostDocument.ResolveReferenceTo<U>(Reference, this as IOpenApiSchema);
        }
    }

    /// <summary>
    /// Gets a value from the resolved target while detecting cycles in delegated member access.
    /// </summary>
    /// <typeparam name="TResult">The type of value to get from the target.</typeparam>
    /// <param name="selector">Selects the value from the resolved target.</param>
    /// <returns>The selected value, or the default value when the target cannot be resolved.</returns>
    /// <remarks>
    /// The guard remains active while <paramref name="selector"/> reads the target member. This covers
    /// the complete delegated call chain without changing the immediate-resolution semantics of
    /// <see cref="Target"/> or walking an acyclic chain more than once.
    /// </remarks>
    private protected TResult GetFromTarget<TResult>(Func<U, TResult> selector)
    {
        Utils.CheckArgumentNull(selector);
        return ExecuteWithReferenceAccessGuard(this, () =>
        {
            return Target is { } target ? selector(target) : default!;
        });
    }

    /// <summary>
    /// Executes an action against the resolved target while detecting cycles in delegated access.
    /// </summary>
    /// <param name="action">The action to execute against the resolved target.</param>
    private protected void ApplyToTarget(Action<U> action)
    {
        Utils.CheckArgumentNull(action);
        ExecuteWithTargetActionGuard<object?>(this, () =>
        {
            if (Target is { } target)
            {
                action(target);
            }
            return null;
        });
    }

    private static TResult ExecuteWithReferenceAccessGuard<TResult>(
        BaseOpenApiReferenceHolder<T, U, V> holder,
        Func<TResult> action)
    {
        return ExecuteWithReferenceGuard(ref t_activeReferenceAccesses, holder, action);
    }

    private static TResult ExecuteWithTargetActionGuard<TResult>(
        BaseOpenApiReferenceHolder<T, U, V> holder,
        Func<TResult> action)
    {
        return ExecuteWithReferenceGuard(ref t_activeTargetActions, holder, action);
    }

    private static TResult ExecuteWithReferenceGuard<TResult>(
        ref HashSet<BaseOpenApiReferenceHolder<T, U, V>>? activeReferences,
        BaseOpenApiReferenceHolder<T, U, V> holder,
        Func<TResult> action)
    {
        activeReferences ??= new HashSet<BaseOpenApiReferenceHolder<T, U, V>>(ReferenceHolderComparer.Instance);
        if (!activeReferences.Add(holder))
        {
            throw new InvalidOperationException($"Circular reference detected while resolving reference: {holder.Reference.ReferenceV3}");
        }

        try
        {
            RuntimeHelpers.EnsureSufficientExecutionStack();
            return action();
        }
        catch (InsufficientExecutionStackException ex)
        {
            throw new InvalidOperationException(
                $"The chain of references starting at {holder.Reference.ReferenceV3} is nested too deeply to resolve.",
                ex);
        }
        finally
        {
            activeReferences.Remove(holder);
            if (activeReferences.Count == 0)
            {
                activeReferences = null;
            }
        }
    }

    /// <inheritdoc/>
    public T? RecursiveTarget
    {
        get
        {
            var visitedReferences = new HashSet<BaseOpenApiReferenceHolder<T, U, V>>(ReferenceHolderComparer.Instance);
            BaseOpenApiReferenceHolder<T, U, V> current = this;

            while (visitedReferences.Add(current))
            {
                switch (current.Target)
                {
                    case BaseOpenApiReferenceHolder<T, U, V> recursiveTarget:
                        current = recursiveTarget;
                        break;
                    case T concrete:
                        return concrete;
                    default:
                        return null;
                }
            }

            throw new InvalidOperationException($"Circular reference detected while resolving reference: {current.Reference.ReferenceV3}");
        }
    }

    private sealed class ReferenceHolderComparer : IEqualityComparer<BaseOpenApiReferenceHolder<T, U, V>>
    {
        internal static ReferenceHolderComparer Instance { get; } = new();

        public bool Equals(BaseOpenApiReferenceHolder<T, U, V>? x, BaseOpenApiReferenceHolder<T, U, V>? y)
        {
            return ReferenceEquals(x, y);
        }

        public int GetHashCode(BaseOpenApiReferenceHolder<T, U, V> obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }
    }
    /// <summary>
    /// Copy the reference as a target element with overrides.
    /// </summary>
    /// <param name="sourceReference">The source reference to copy</param>
    /// <returns>The copy of the reference</returns>
    protected abstract V CopyReference(V sourceReference);

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="source">The parameter reference to copy</param>
    protected BaseOpenApiReferenceHolder(BaseOpenApiReferenceHolder<T, U, V> source)
    {
        Utils.CheckArgumentNull(source);
        Reference = CopyReference(source.Reference);
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

        Reference = new V()
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
    public required V Reference { get; init; }
#else
    /// <inheritdoc/>
    public V Reference { get; init; }
#endif
    /// <inheritdoc/>
    public abstract U CopyReferenceAsTargetElementWithOverrides(U source);
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
    public virtual void SerializeAsV32(IOpenApiWriter writer)
    {
        if (!writer.GetSettings().ShouldInlineReference(Reference))
        {
            Reference.SerializeAsV32(writer);
        }
        else
        {
            SerializeInternal(writer, (writer, element) => CopyReferenceAsTargetElementWithOverrides(element).SerializeAsV32(writer));
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
        Action<IOpenApiWriter, U> action)
    {
        Utils.CheckArgumentNull(writer);
        ApplyToTarget(element => action(writer, element));
    }
}
