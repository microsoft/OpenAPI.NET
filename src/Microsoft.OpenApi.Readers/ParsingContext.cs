// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ReferenceServices;

namespace Microsoft.OpenApi.Readers
{
    /// <summary>
    /// Parsing context.
    /// </summary>
    public class ParsingContext
    {
        private readonly Stack<string> currentLocation = new Stack<string>();

        private readonly Dictionary<string, IOpenApiReferenceable> referenceStore =
            new Dictionary<string, IOpenApiReferenceable>();

        private readonly Dictionary<string, object> tempStorage = new Dictionary<string, object>();

        private IOpenApiReferenceService _referenceService;

        /// <summary>
        /// End the current object.
        /// </summary>
        public void EndObject()
        {
            currentLocation.Pop();
        }

        /// <summary>
        /// Get the current location as string representing JSON pointer.
        /// </summary>
        public string GetLocation()
        {
            return "#/" + string.Join("/", currentLocation.Reverse().ToArray());
        }

        /// <summary>
        /// Gets the referenced object
        /// </summary>
        public IOpenApiReferenceable GetReferencedObject(
            OpenApiDiagnostic diagnostic,
            ReferenceType referenceType,
            string referenceString)
        {
            referenceStore.TryGetValue(referenceString, out var returnValue);

            // If reference has already been accessed once, simply return the same reference object.
            if (returnValue != null)
            {
                return returnValue;
            }

            var reference = _referenceService.ConvertToOpenApiReference(referenceString, referenceType);

            returnValue = _referenceService.LoadReference(reference);

            if (returnValue != null)
            {
                returnValue.Pointer = reference;
                referenceStore.Add(referenceString, returnValue);
            }
            else
            {
                diagnostic.Errors.Add(
                    new
                        OpenApiError(
                            GetLocation(),
                            $"Cannot resolve the reference {referenceString}"));
            }

            return returnValue;
        }

        /// <summary>
        /// Gets the value from the temporary storage matching the given key.
        /// </summary>
        public T GetFromTempStorage<T>(string key) where T : class
        {
            if (tempStorage.TryGetValue(key, out var value))
            {
                return (T)value;
            }

            return null;
        }

        /// <summary>
        /// Sets the reference service.
        /// </summary>
        /// <param name="referenceService"></param>
        public void SetReferenceService(IOpenApiReferenceService referenceService)
        {
            _referenceService = referenceService;
        }

        /// <summary>
        /// Sets the temporary storge for this key and value.
        /// </summary>
        public void SetTempStorage(string key, object value)
        {
            tempStorage[key] = value;
        }

        /// <summary>
        /// Starts an object with the given object name.
        /// </summary>
        public void StartObject(string objectName)
        {
            currentLocation.Push(objectName);
        }
    }
}