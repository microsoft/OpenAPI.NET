// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.Interface;
using Microsoft.OpenApi.Readers.ParseNodes;
using Microsoft.OpenApi.Readers.V2;
using Microsoft.OpenApi.Readers.V3;
using SharpYaml.Serialization;

namespace Microsoft.OpenApi.Readers
{
    /// <summary>
    /// The Parsing Context holds temporary state needed whilst parsing an OpenAPI Document
    /// </summary>
    public class ParsingContext
    {
        private readonly Stack<string> _currentLocation = new Stack<string>();
        private readonly Dictionary<string, IOpenApiReferenceable> _referenceStore = new Dictionary<string, IOpenApiReferenceable>();
        private readonly Dictionary<string, object> _tempStorage = new Dictionary<string, object>();
        private IOpenApiVersionService _versionService;
        internal RootNode RootNode { get; set; }
        internal List<OpenApiTag> Tags { get; private set; } = new List<OpenApiTag>();


        /// <summary>
        /// Initiates the parsing process.  Not thread safe and should only be called once on a parsing context
        /// </summary>
        /// <param name="yamlDocument"></param>
        /// <param name="diagnostic"></param>
        /// <returns>An OpenApiDocument populated based on the passed yamlDocument </returns>
        internal OpenApiDocument Parse(YamlDocument yamlDocument, OpenApiDiagnostic diagnostic)
        {
            RootNode = new RootNode(this, diagnostic, yamlDocument);

            var inputVersion = GetVersion(RootNode);

            OpenApiDocument doc;

            if ( inputVersion == "2.0")
            {
                VersionService = new OpenApiV2VersionService();
                doc = this.VersionService.LoadDocument(this.RootNode);
            }
            else if (inputVersion.StartsWith("3.0."))
            {
                this.VersionService = new OpenApiV3VersionService();
                doc = this.VersionService.LoadDocument(this.RootNode);
            }
            else
            {
                // If version number is not recognizable,
                // our best effort will try to deserialize the document to V3.
                this.VersionService = new OpenApiV3VersionService();
                doc = this.VersionService.LoadDocument(this.RootNode);
            }
            return doc;
        }

        /// <summary>
        /// Gets the version of the Open API document.
        /// </summary>
        private static string GetVersion(RootNode rootNode)
        {
            var versionNode = rootNode.Find(new JsonPointer("/openapi"));

            if (versionNode != null)
            {
                return versionNode.GetScalarValue();
            }

            versionNode = rootNode.Find(new JsonPointer("/swagger"));

            return versionNode?.GetScalarValue();
        }

        private void ComputeTags(List<OpenApiTag> tags, Func<MapNode,OpenApiTag> loadTag )
        {
            // Precompute the tags array so that each tag reference does not require a new deserialization.
            var tagListPointer = new JsonPointer("#/tags");

            var tagListNode = RootNode.Find(tagListPointer);

            if (tagListNode != null && tagListNode is ListNode)
            {
                var tagListNodeAsListNode = (ListNode)tagListNode;
                tags.AddRange(tagListNodeAsListNode.CreateList(loadTag));
            }
        }




        /// <summary>
        /// Service providing all Version specific conversion functions
        /// </summary>
        internal IOpenApiVersionService VersionService
        {
            get
            {
                return _versionService;
            }
            set
            {
                _versionService = value;
                ComputeTags(Tags, VersionService.TagLoader);
            }
        }

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

            var reference = VersionService.ConvertToOpenApiReference(referenceString, referenceType);

            var isReferencedObjectFound = VersionService.TryLoadReference(this, reference, out referencedObject);

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