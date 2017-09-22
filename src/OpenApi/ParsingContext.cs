
namespace Tavis.OpenApi
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Tavis.OpenApi.Model;

    public class ParsingContext
    {
        public string Version { get; set; }
        public List<OpenApiError> ParseErrors { get; set; } = new List<OpenApiError>();
        public OpenApiDocument OpenApiDocument { get; internal set; }

        private Dictionary<string, object> tempStorage = new Dictionary<string, object>();

        private Dictionary<string, IReference> referenceStore = new Dictionary<string, IReference>();

        private IReferenceService referenceService;

        private Stack<string> currentLocation = new Stack<string>();

        public void SetReferenceService(IReferenceService referenceService)
        {
            this.referenceService = referenceService;
        }

        internal void StartObject(string objectName)
        {
            this.currentLocation.Push(objectName);
        }

        internal void EndObject()
        {
            this.currentLocation.Pop();
        }
        public string GetLocation() {
            return "#/" + String.Join("/", this.currentLocation.Reverse().ToArray());
        }

        public IReference GetReferencedObject(string pointer)
        {
            var reference = this.referenceService.ParseReference(pointer);
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
                returnValue = this.referenceService.LoadReference(reference);
                previousPointers.Pop();
                if (returnValue != null)
                {
                    returnValue.Pointer = reference;
                    referenceStore.Add(reference.ToString(), returnValue);
                }
                else
                {
                    ParseErrors.Add(new OpenApiError(this.GetLocation(), $"Cannot resolve $ref {reference.ToString()}"));
                }
            }

            return returnValue;
        }

        private Stack<string> previousPointers = new Stack<string>();

        public void SetTempStorage(string key, object value)
        {
            this.tempStorage[key] = value;
        }
        public T GetTempStorage<T>(string key)  where T:class
        {
            object value;
            if (this.tempStorage.TryGetValue(key,out value))
            {
                return (T)value;
            }
            return null;
        }
    }


}
