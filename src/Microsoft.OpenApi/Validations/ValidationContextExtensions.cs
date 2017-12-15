// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Validations.Visitors;

namespace Microsoft.OpenApi.Validations
{
    internal static class ValidationContextExtensions
    {
        /// <summary>
        /// Validate the Open API element.
        /// </summary>
        /// <typeparam name="T">The Open API element.</typeparam>
        /// <param name="context">The validation cotext.</param>
        /// <param name="element">The Open API element.</param>
        public static void Validate<T>(this ValidationContext context, T element) where T : IOpenApiElement
        {
            if (context == null)
            {
                throw Error.ArgumentNull(nameof(context));
            }

            if (element != null)
            {
                IVisitor visitor = OpenApiVisitorSet.GetVisitor(typeof(T));
                if (visitor != null)
                {
                    visitor.Visit(context, element);
                }
            }
        }

        /// <summary>
        /// Validate the collection of Open API element.
        /// </summary>
        /// <typeparam name="T">The Open API element.</typeparam>
        /// <param name="context">The validation cotext.</param>
        /// <param name="collection">The collection of Open API element</param>
        public static void ValidateCollection<T>(this ValidationContext context, IEnumerable<T> collection)
            where T : IOpenApiElement
        {
            if (context == null)
            {
                throw Error.ArgumentNull(nameof(context));
            }

            if (collection != null)
            {
                foreach (var element in collection)
                {
                    context.Validate(element);
                }
            }
        }

        /// <summary>
        /// Validate the map of Open API element.
        /// </summary>
        /// <typeparam name="T">The Open API element.</typeparam>
        /// <param name="context">The validation cotext.</param>
        /// <param name="elements">The map of Open API element.</param>
        public static void ValidateMap<T>(this ValidationContext context, IDictionary<string, T> elements)
            where T : IOpenApiElement
        {
            if (context == null)
            {
                throw Error.ArgumentNull(nameof(context));
            }

            if (elements != null)
            {
                foreach (var element in elements)
                {
                    context.Validate(element.Value);
                }
            }
        }
    }
}
