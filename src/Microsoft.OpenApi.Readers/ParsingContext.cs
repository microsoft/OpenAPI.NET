// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.Exceptions;
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
        private readonly Dictionary<string, object> _tempStorage = new Dictionary<string, object>();
        private readonly Dictionary<object, Dictionary<string, object>> _scopedTempStorage = new Dictionary<object, Dictionary<string, object>>();
        private readonly Dictionary<string, Stack<string>> _loopStacks = new Dictionary<string, Stack<string>>();
        internal Dictionary<string, Func<IOpenApiAny, OpenApiSpecVersion, IOpenApiExtension>> ExtensionParsers { get; set; } = new Dictionary<string, Func<IOpenApiAny, OpenApiSpecVersion, IOpenApiExtension>>();
        internal RootNode RootNode { get; set; }
        internal List<OpenApiTag> Tags { get; private set; } = new List<OpenApiTag>();
        internal Uri BaseUrl { get; set; }

        /// <summary>
        /// Diagnostic object that returns metadata about the parsing process.
        /// </summary>
        public OpenApiDiagnostic Diagnostic { get; }

        /// <summary>
        /// Create Parsing Context
        /// </summary>
        /// <param name="diagnostic">Provide instance for diagnotic object for collecting and accessing information about the parsing.</param>
        public ParsingContext(OpenApiDiagnostic diagnostic)
        {
            Diagnostic = diagnostic;
        }

        /// <summary>
        /// Initiates the parsing process.  Not thread safe and should only be called once on a parsing context
        /// </summary>
        /// <param name="yamlDocument">Yaml document to parse.</param>
        /// <returns>An OpenApiDocument populated based on the passed yamlDocument </returns>
        internal OpenApiDocument Parse(YamlDocument yamlDocument)
        {
            RootNode = new RootNode(this, yamlDocument);

            var inputVersion = GetVersion(RootNode);

            OpenApiDocument doc;

            switch (inputVersion)
            {
                case string version when version == "2.0":
                    VersionService = new OpenApiV2VersionService();
                    doc = VersionService.LoadDocument(RootNode);
                    this.Diagnostic.SpecificationVersion = OpenApiSpecVersion.OpenApi2_0;
                    break;

                case string version when version.StartsWith("3.0"):
                    VersionService = new OpenApiV3VersionService();
                    doc = VersionService.LoadDocument(RootNode);
                    this.Diagnostic.SpecificationVersion = OpenApiSpecVersion.OpenApi3_0;
                    break;

                default:
                    throw new OpenApiUnsupportedSpecVersionException(inputVersion);
            }

            return doc;
        }

        /// <summary>
        /// Initiates the parsing process of a fragment.  Not thread safe and should only be called once on a parsing context
        /// </summary>
        /// <param name="yamlDocument"></param>
        /// <param name="version">OpenAPI version of the fragment</param>
        /// <returns>An OpenApiDocument populated based on the passed yamlDocument </returns>
        internal T ParseFragment<T>(YamlDocument yamlDocument, OpenApiSpecVersion version) where T : IOpenApiElement
        {
            var node = ParseNode.Create(this, yamlDocument.RootNode);

            T element = default(T);

            switch (version)
            {
                case OpenApiSpecVersion.OpenApi2_0:
                    VersionService = new OpenApiV2VersionService();
                    element = this.VersionService.LoadElement<T>(node);
                    break;

                case OpenApiSpecVersion.OpenApi3_0:
                    this.VersionService = new OpenApiV3VersionService();
                    element = this.VersionService.LoadElement<T>(node);
                    break;
            }

            return element;
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

        /// <summary>
        /// Service providing all Version specific conversion functions
        /// </summary>
        internal IOpenApiVersionService VersionService { get; set; }

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
            return "#/" + string.Join("/", _currentLocation.Reverse().Select(s=> s.Replace("~","~0").Replace("/","~1")).ToArray());
        }

        /// <summary>
        /// Gets the value from the temporary storage matching the given key.
        /// </summary>
        public T GetFromTempStorage<T>(string key, object scope = null)
        {
            Dictionary<string, object> storage;

            if (scope == null)
            {
                storage = _tempStorage;
            }
            else if (!_scopedTempStorage.TryGetValue(scope, out storage))
            {
                return default(T);
            }

            return storage.TryGetValue(key, out var value) ? (T)value : default(T);
        }

        /// <summary>
        /// Sets the temporary storge for this key and value.
        /// </summary>
        public void SetTempStorage(string key, object value, object scope = null)
        {
            Dictionary<string, object> storage;

            if (scope == null)
            {
                storage = _tempStorage;
            }
            else if (!_scopedTempStorage.TryGetValue(scope, out storage))
            {
                storage = _scopedTempStorage[scope] = new Dictionary<string, object>();
            }

            if (value == null)
            {
                storage.Remove(key);
            }
            else
            {
                storage[key] = value;
            }
        }

        /// <summary>
        /// Starts an object with the given object name.
        /// </summary>
        public void StartObject(string objectName)
        {
            _currentLocation.Push(objectName);
        }

        /// <summary>
        /// Maintain history of traversals to avoid stack overflows from cycles
        /// </summary>
        /// <param name="loopId">Any unique identifier for a stack.</param>
        /// <param name="key">Identifier used for current context.</param>
        /// <returns>If method returns false a loop was detected and the key is not added.</returns>
        public bool PushLoop(string loopId, string key)
        {
            Stack<string> stack;
            if (!_loopStacks.TryGetValue(loopId, out stack))
            {
                stack = new Stack<string>();
                _loopStacks.Add(loopId, stack);
            }

            if (!stack.Contains(key))
            {
                stack.Push(key);
                return true;
            }
            else
            {
                return false;  // Loop detected
            }
        }

        /// <summary>
        /// Reset loop tracking stack
        /// </summary>
        /// <param name="loopid">Identifier of loop to clear</param>
        internal void ClearLoop(string loopid)
        {
            _loopStacks[loopid].Clear();
        }

        /// <summary>
        /// Exit from the context in cycle detection
        /// </summary>
        /// <param name="loopid">Identifier of loop</param>
        public void PopLoop(string loopid)
        {
            if (_loopStacks[loopid].Count > 0)
            {
                _loopStacks[loopid].Pop();
            }
        }

    }
}
