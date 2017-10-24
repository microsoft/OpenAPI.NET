// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OpenApi.Readers.YamlReaders
{
    public class ParsingContext
    {
        private readonly Stack<string> currentLocation = new Stack<string>();

        private readonly Stack<string> previousPointers = new Stack<string>();

        private IOpenApiReferenceService referenceService;

        private readonly Dictionary<string, IOpenApiReference> referenceStore = new Dictionary<string, IOpenApiReference>();

        private readonly Dictionary<string, object> tempStorage = new Dictionary<string, object>();
        
        public string Version { get; set; }

        public void EndObject()
        {
            currentLocation.Pop();
        }

        public string GetLocation()
        {
            return "#/" + string.Join("/", currentLocation.Reverse().ToArray());
        }

        public IOpenApiReference GetReferencedObject(OpenApiDiagnostic diagnostic, string pointer)
        {
            var reference = referenceService.ParseReference(pointer);
            return GetReferencedObject(diagnostic, reference);
        }

        public IOpenApiReference GetReferencedObject(OpenApiDiagnostic diagnostic, OpenApiReference reference)
        {
            IOpenApiReference returnValue = null;
            referenceStore.TryGetValue(reference.ToString(), out returnValue);

            if (returnValue == null)
            {
                if (previousPointers.Contains(reference.ToString()))
                {
                    return null; // Return reference object?
                }

                previousPointers.Push(reference.ToString());
                returnValue = referenceService.LoadReference(reference);
                previousPointers.Pop();

                if (returnValue != null)
                {
                    returnValue.Pointer = reference;
                    referenceStore.Add(reference.ToString(), returnValue);
                }
                else
                {
                    diagnostic.Errors.Add(new OpenApiError(GetLocation(), $"Cannot resolve $ref {reference}"));
                }
            }

            return returnValue;
        }

        public T GetTempStorage<T>(string key) where T : class
        {
            object value;
            if (tempStorage.TryGetValue(key, out value))
            {
                return (T)value;
            }

            return null;
        }

        public void SetReferenceService(IOpenApiReferenceService referenceService)
        {
            this.referenceService = referenceService;
        }

        public void SetTempStorage(string key, object value)
        {
            tempStorage[key] = value;
        }

        public void StartObject(string objectName)
        {
            currentLocation.Push(objectName);
        }
    }
}