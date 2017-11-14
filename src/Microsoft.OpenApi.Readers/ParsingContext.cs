// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.Interface;

namespace Microsoft.OpenApi.Readers
{
    /// <summary>
    /// Parsing context.
    /// </summary>
    public class ParsingContext
    {
        private readonly Stack<string> currentLocation = new Stack<string>();

        private readonly Stack<string> previousPointers = new Stack<string>();

        private IOpenApiReferenceService _referenceService;

        private readonly Dictionary<string, IOpenApiReferenceable> referenceStore = new Dictionary<string, IOpenApiReferenceable>();

        private readonly Dictionary<string, object> tempStorage = new Dictionary<string, object>();
        
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
        /// Get the referenced object.
        /// </summary>
        /// <param name="diagnostic"></param>
        /// <param name="pointer"></param>
        /// <returns></returns>
        public IOpenApiReferenceable GetReferencedObject(OpenApiDiagnostic diagnostic, string pointer)
        {
            var reference = _referenceService.FromString(pointer);
            return GetReferencedObject(diagnostic, reference);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="diagnostic"></param>
        /// <param name="reference"></param>
        /// <returns></returns>
        public IOpenApiReferenceable GetReferencedObject(OpenApiDiagnostic diagnostic, OpenApiReference reference)
        {
            IOpenApiReferenceable returnValue = null;
            string referenceString = _referenceService.ToString(reference);
            referenceStore.TryGetValue(referenceString, out returnValue);

            if (returnValue == null)
            {
                if (previousPointers.Contains(referenceString))
                {
                    return null; // Return reference object?
                }

                previousPointers.Push(referenceString);
                returnValue = _referenceService.LoadReference(reference);
                previousPointers.Pop();

                if (returnValue != null)
                {
                    returnValue.Pointer = reference;
                    referenceStore.Add(referenceString, returnValue);
                }
                else
                {
                    diagnostic.Errors.Add(new OpenApiError(GetLocation(), $"Cannot resolve $ref {reference}"));
                }
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
            this._referenceService = referenceService;
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