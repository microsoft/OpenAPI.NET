// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

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
        private readonly Stack<string> _currentLocation = new Stack<string>();

        private readonly Dictionary<string, IOpenApiReferenceable> _referenceStore =
            new Dictionary<string, IOpenApiReferenceable>();

        private readonly Dictionary<string, object> _tempStorage = new Dictionary<string, object>();

        /// <summary>
        /// Reference service.
        /// </summary>
        internal IOpenApiReferenceService ReferenceService { get; set; }

        /// <summary>
        /// End the current object.
        /// </summary>
        public void EndObject()
        {
            _currentLocation.Pop();
        }

        /// <summary>
        /// Get the current location as string representing JSON pointer.
        /// </summary>
        public string GetLocation()
        {
            return "#/" + string.Join("/", _currentLocation.Reverse().ToArray());
        }

        /// <summary>
        /// Gets the referenced object.
        /// </summary>
        public IOpenApiReferenceable GetReferencedObject(
            OpenApiDiagnostic diagnostic,
            ReferenceType referenceType,
            string referenceString)
        {
            _referenceStore.TryGetValue(referenceString, out var referencedObject);

            // If reference has already been accessed once, simply return the same reference object.
            if (referencedObject != null)
            {
                return referencedObject;
            }

            var reference = ReferenceService.ConvertToOpenApiReference(referenceString, referenceType);

            var isReferencedObjectFound = ReferenceService.TryLoadReference(reference, out referencedObject);

            if (isReferencedObjectFound)
            {
                // Populate the Reference section of the object, so that the writers
                // can recognize that this is referencing another object.
                referencedObject.Reference = reference;
                _referenceStore.Add(referenceString, referencedObject);
            }
            else if (referencedObject != null)
            {
                return referencedObject;
            }
            else
            {
                diagnostic.Errors.Add(
                    new OpenApiError(
                        GetLocation(),
                        $"Cannot resolve the reference {referenceString}"));
            }

            return referencedObject;
        }

        /// <summary>
        /// Gets the value from the temporary storage matching the given key.
        /// </summary>
        public T GetFromTempStorage<T>(string key) where T : class
        {
            if (_tempStorage.TryGetValue(key, out var value))
            {
                return (T)value;
            }

            return null;
        }

        /// <summary>
        /// Sets the temporary storge for this key and value.
        /// </summary>
        public void SetTempStorage(string key, object value)
        {
            _tempStorage[key] = value;
        }

        /// <summary>
        /// Starts an object with the given object name.
        /// </summary>
        public void StartObject(string objectName)
        {
            _currentLocation.Push(objectName);
        }
    }
}