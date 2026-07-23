// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

namespace Microsoft.OpenApi;
/// <summary>
/// Interface for deep copyable objects.
/// </summary>
/// <typeparam name="T">The type of the resulting object.</typeparam>
[System.Diagnostics.CodeAnalysis.Experimental("OPENAPI001")]
public interface IDeepCopyable<out T>
{
    /// <summary>
    /// Create a deep copy of the current instance.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.Experimental("OPENAPI001")]
    T CreateDeepCopy();
}
