// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Properties;
using Microsoft.OpenApi.Interfaces;

namespace Microsoft.OpenApi.Validations.Visitors
{
    /// <summary>
    /// Class to cache the <see cref="IVisitor"/>.
    /// </summary>
    internal static class OpenApiVisitorSet
    {
        private static IDictionary<Type, IVisitor> _visitors;

        /// <summary>
        /// Gets the visitors
        /// </summary>
        public static IDictionary<Type, IVisitor> Visitors
        {
            get
            {
                if (_visitors == null)
                {
                    _visitors = new Lazy<IDictionary<Type, IVisitor>>(() => BuildVisitorSet(), isThreadSafe: false).Value;
                }

                return _visitors;
            }
        }
        /// <summary>
        /// Get the element visitor.
        /// </summary>
        /// <param name="elementType">The element type.</param>
        /// <returns>The element visitor or null.</returns>
        public static IVisitor GetVisitor(Type elementType)
        {
            IVisitor visitor;
            if (Visitors.TryGetValue(elementType, out visitor))
            {
                return visitor;
            }

            throw new OpenApiException(String.Format(SRResource.UnknownVisitorType, elementType.FullName));
        }

        private static IDictionary<Type, IVisitor> BuildVisitorSet()
        {
            IDictionary<Type, IVisitor> visitors = new Dictionary<Type, IVisitor>();

            IEnumerable<Type> allTypes = typeof(OpenApiVisitorSet).Assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && t != typeof(object));

            Type visitorInterfaceType = typeof(IVisitor);
            Type elementType = typeof(IOpenApiElement);
            foreach (Type type in allTypes)
            {
                if (!visitorInterfaceType.IsAssignableFrom(type))
                {
                    continue;
                }

                Type baseType = type.BaseType;
                if (baseType == null || !baseType.IsGenericType ||
                    baseType.GetGenericTypeDefinition() != typeof(VisitorBase<>))
                {
                    continue;
                }

                Type validationType = baseType.GetGenericArguments()[0];
                if (!elementType.IsAssignableFrom(validationType))
                {
                    continue;
                }

                object instance = Activator.CreateInstance(type);
                IVisitor visitor = instance as IVisitor;
                if (visitor != null)
                {
                    visitors[validationType] = visitor;
                }
            }

            return visitors;
        }
    }
}
