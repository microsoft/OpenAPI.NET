// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using Microsoft.OpenApi.Reader.V2;
using Microsoft.OpenApi.Reader.V3;
using Microsoft.OpenApi.Reader.V31;
using Microsoft.OpenApi.Reader.V32;

namespace Microsoft.OpenApi.Reader
{
    /// <summary>
    /// The Parsing Context holds temporary state needed whilst parsing an OpenAPI Document
    /// </summary>
    public class ParsingContext
    {
        private readonly Stack<string> _currentLocation = new();
        private readonly Dictionary<string, object> _tempStorage = new();
        private readonly Dictionary<object, Dictionary<string, object>> _scopedTempStorage = new();

        /// <summary>
        /// Extension parsers
        /// </summary>
        public Dictionary<string, Func<JsonNode, OpenApiSpecVersion, IOpenApiExtension>>? ExtensionParsers { get; set; } = 
            new();

        internal RootNode? RootNode { get; set; }
        internal List<OpenApiTag> Tags { get; private set; } = new();

        /// <summary>
        /// The base url for the document
        /// </summary>
        public Uri? BaseUrl { get; set; }

        /// <summary>
        /// Default content type for a response object
        /// </summary>
        public List<string>? DefaultContentType { get; set; }

        /// <summary>
        /// Diagnostic object that returns metadata about the parsing process.
        /// </summary>
        public OpenApiDiagnostic Diagnostic { get; }

        /// <summary>
        /// Create Parsing Context
        /// </summary>
        /// <param name="diagnostic">Provide instance for diagnostic object for collecting and accessing information about the parsing.</param>
        public ParsingContext(OpenApiDiagnostic diagnostic)
        {
            Diagnostic = diagnostic;
        }

        /// <summary>
        /// Initiates the parsing process.  Not thread safe and should only be called once on a parsing context
        /// </summary>
        /// <param name="jsonNode">Set of Json nodes to parse.</param>
        /// <param name="location">Location of where the document that is getting loaded is saved</param>
        /// <returns>An OpenApiDocument populated based on the passed yamlDocument </returns>
        public OpenApiDocument Parse(JsonNode jsonNode, Uri location)
        {
            RootNode = new RootNode(this, jsonNode);

            var inputVersion = GetVersion(RootNode);

            OpenApiDocument doc;

            switch (inputVersion)
            {
                case string version when version.is2_0():
                    VersionService = new OpenApiV2VersionService(Diagnostic);
                    doc = VersionService.LoadDocument(RootNode, location);
                    this.Diagnostic.SpecificationVersion = OpenApiSpecVersion.OpenApi2_0;
                    ValidateRequiredFields(doc, version);
                    break;

                case string version when version.is3_0():
                    VersionService = new OpenApiV3VersionService(Diagnostic);
                    doc = VersionService.LoadDocument(RootNode, location);
                    this.Diagnostic.SpecificationVersion = version.is3_1() ? OpenApiSpecVersion.OpenApi3_1 : OpenApiSpecVersion.OpenApi3_0;
                    ValidateRequiredFields(doc, version);
                    break;
                case string version when version.is3_1():
                    VersionService = new OpenApiV31VersionService(Diagnostic);
                    doc = VersionService.LoadDocument(RootNode, location);
                    this.Diagnostic.SpecificationVersion = OpenApiSpecVersion.OpenApi3_1;
                    ValidateRequiredFields(doc, version);
                    break;
                case string version when version.is3_2():
                    VersionService = new OpenApiV32VersionService(Diagnostic);
                    doc = VersionService.LoadDocument(RootNode, location);
                    this.Diagnostic.SpecificationVersion = OpenApiSpecVersion.OpenApi3_2;
                    ValidateRequiredFields(doc, version);
                    break;
                default:
                    throw new OpenApiUnsupportedSpecVersionException(inputVersion);
            }

            return doc;
        }

        /// <summary>
        /// Initiates the parsing process of a fragment.  Not thread safe and should only be called once on a parsing context
        /// </summary>
        /// <param name="jsonNode"></param>
        /// <param name="version">OpenAPI version of the fragment</param>
        /// <param name="openApiDocument">The OpenApiDocument object to which the fragment belongs, used to lookup references.</param>
        /// <returns>An OpenApiDocument populated based on the passed yamlDocument </returns>
        public T? ParseFragment<T>(JsonNode jsonNode, OpenApiSpecVersion version, OpenApiDocument openApiDocument) where T : IOpenApiElement
        {
            var node = ParseNode.Create(this, jsonNode);

            var element = default(T);

            switch (version)
            {
                case OpenApiSpecVersion.OpenApi2_0:
                    VersionService = new OpenApiV2VersionService(Diagnostic);
                    element = this.VersionService.LoadElement<T>(node, openApiDocument);
                    break;

                case OpenApiSpecVersion.OpenApi3_0:
                    this.VersionService = new OpenApiV3VersionService(Diagnostic);
                    element = this.VersionService.LoadElement<T>(node, openApiDocument);
                    break;
                case OpenApiSpecVersion.OpenApi3_1:
                    this.VersionService = new OpenApiV31VersionService(Diagnostic);
                    element = this.VersionService.LoadElement<T>(node, openApiDocument);
                    break;
                case OpenApiSpecVersion.OpenApi3_2:
                    this.VersionService = new OpenApiV32VersionService(Diagnostic);
                    element = this.VersionService.LoadElement<T>(node, openApiDocument);
                    break;
            }

            return element;
        }

        /// <summary>
        /// Gets the version of the Open API document.
        /// </summary>
        private static string GetVersion(RootNode rootNode)
        {
            var versionNode = rootNode.Find(new("/openapi"));

            if (versionNode is not null)
            {
                return versionNode.GetScalarValue().Replace("\"", string.Empty);
            }

            versionNode = rootNode.Find(new("/swagger"));

            return versionNode?.GetScalarValue().Replace("\"", string.Empty) ?? throw new OpenApiException("Version node not found.");
        }

        /// <summary>
        /// Service providing all Version specific conversion functions
        /// </summary>
        internal IOpenApiVersionService? VersionService { get; set; }

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
            return "#/" + string.Join("/", _currentLocation.Reverse().Select(s => s.Replace("~", "~0").Replace("/", "~1")).ToArray());
        }

        /// <summary>
        /// Gets the value from the temporary storage matching the given key.
        /// </summary>
        public T? GetFromTempStorage<T>(string key, object? scope = null)
        {
            Dictionary<string, object>? storage;

            if (scope == null)
            {
                storage = _tempStorage;
            }
            else if (!_scopedTempStorage.TryGetValue(scope, out storage))
            {
                return default;
            }

            return storage.TryGetValue(key, out var value) ? (T)value : default;
        }

        /// <summary>
        /// Sets the temporary storage for this key and value.
        /// </summary>
        public void SetTempStorage(string key, object? value, object? scope = null)
        {
            Dictionary<string, object>? storage;

            if (scope == null)
            {
                storage = _tempStorage;
            }
            else if (!_scopedTempStorage.TryGetValue(scope, out storage))
            {
                storage = _scopedTempStorage[scope] = new();
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
        private void ValidateRequiredFields(OpenApiDocument doc, string version)
        {
            if ((version.is2_0() || version.is3_0()) && (doc.Paths == null) && RootNode is not null)
            {
                // paths is a required field in OpenAPI 2.0 and 3.0 but optional in 3.1
                RootNode.Context.Diagnostic.Errors.Add(new OpenApiError("", $"Paths is a REQUIRED field at {RootNode.Context.GetLocation()}"));
            }
        }
    }
}
