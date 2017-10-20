// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Readers.Interface;

namespace Microsoft.OpenApi.Readers.YamlReaders
{
    public class ParsingContext : ILog<OpenApiError>
    {
        private readonly Stack<string> currentLocation = new Stack<string>();

        private readonly Stack<string> previousPointers = new Stack<string>();

        private IReferenceService referenceService;

        private readonly Dictionary<string, IReference> referenceStore = new Dictionary<string, IReference>();

        private readonly Dictionary<string, object> tempStorage = new Dictionary<string, object>();

        public OpenApiDocument OpenApiDocument { get; set; }

        public IList<OpenApiError> Errors { get; set; } = new List<OpenApiError>();

        public string Version { get; set; }

        public void EndObject()
        {
            currentLocation.Pop();
        }

        public string GetLocation()
        {
            return "#/" + string.Join("/", currentLocation.Reverse().ToArray());
        }

        public IReference GetReferencedObject(string pointer)
        {
            var reference = referenceService.ParseReference(pointer);
            return GetReferencedObject(reference);
        }

        public IReference GetReferencedObject(OpenApiReference reference)
        {
            IReference returnValue = null;
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
                    Errors.Add(new OpenApiError(GetLocation(), $"Cannot resolve $ref {reference}"));
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

        public void SetReferenceService(IReferenceService referenceService)
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