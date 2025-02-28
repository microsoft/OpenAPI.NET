namespace Microsoft.OpenApi.Interfaces;
/// <summary>
/// Interface for shallow copyable objects.
/// </summary>
/// <typeparam name="T">The type of the resulting object</typeparam>
public interface IShallowCopyable<out T>
{
    /// <summary>
    /// Create a shallow copy of the current instance.
    /// </summary>
    T CreateShallowCopy();
}
