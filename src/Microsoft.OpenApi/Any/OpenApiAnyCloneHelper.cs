// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Diagnostics.CodeAnalysis;

namespace Microsoft.OpenApi.Any
{
    /// <summary>
    /// Contains logic for cloning objects through copy constructors.
    /// </summary>
    public class OpenApiAnyCloneHelper
    {
        /// <summary>
        /// Clones an instance of <see cref="IOpenApiAny"/> object from the copy constructor
        /// </summary>
        /// <param name="obj">The object instance.</param>
        /// <returns>A clone copy or the object itself.</returns>
        public static IOpenApiAny CloneFromCopyConstructor(IOpenApiAny obj)
        {
            if (obj != null)
            {
                var t = obj.GetType();
                foreach (var ci in t.GetConstructors())
                {
                    var pi = ci.GetParameters();
                    if (pi.Length == 1 && pi[0].ParameterType == t)
                    {
                        return (IOpenApiAny)ci.Invoke(new object[] { obj });
                    }
                }
            }

            return obj;
        }
        
        /// <summary>
        /// Clones an instance of <see cref="IOpenApiAny"/> object from the copy constructor
        /// </summary>
        /// <param name="obj">The object instance.</param>
        /// <returns>A clone copy or the object itself.</returns>
        public static IOpenApiAny CloneFromCopyConstructor<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>(T obj) where T : IOpenApiAny
        {
            if (obj != null)
            {
                foreach (var ci in typeof(T).GetConstructors())
                {
                    var pi = ci.GetParameters();
                    if (pi.Length == 1 && pi[0].ParameterType == typeof(T))
                    {
                        return (IOpenApiAny)ci.Invoke([obj]);
                    }
                }
            }

            return obj;
        }
    }
}
