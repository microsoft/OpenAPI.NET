using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Diff.BusinessObjects;
using Microsoft.OpenApi.Diff.Compare;
using Microsoft.OpenApi.Diff.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;

namespace Microsoft.OpenApi.Diff
{
    public class OpenAPICompare : IOpenAPICompare
    {
        private readonly ILogger<OpenAPICompare> _logger;
        private readonly IEnumerable<IExtensionDiff> _extensions;

        public OpenAPICompare(ILogger<OpenAPICompare> logger, IEnumerable<IExtensionDiff> extensions)
        {
            _logger = logger;
            _extensions = extensions;
        }

        public ChangedOpenApiBO FromLocations(string oldLocation, string newLocation, OpenApiReaderSettings settings = null)
        {
            return FromLocations(oldLocation, Path.GetFileNameWithoutExtension(oldLocation), newLocation, Path.GetFileNameWithoutExtension(newLocation), settings);
        }

        public ChangedOpenApiBO FromLocations(string oldLocation, string oldIdentifier, string newLocation, string newIdentifier, OpenApiReaderSettings settings = null)
        {
            return FromSpecifications(ReadLocation(oldLocation, settings: settings), oldIdentifier, ReadLocation(newLocation, settings: settings), newIdentifier);
        }

        public ChangedOpenApiBO FromSpecifications(OpenApiDocument oldSpec, string oldSpecIdentifier, OpenApiDocument newSpec, string newSpecIdentifier)
        {
            return OpenApiDiff.Compare(oldSpec, oldSpecIdentifier, newSpec, newSpecIdentifier, _extensions, _logger);
        }

        private static OpenApiDocument ReadLocation(string location, List<OpenApiOAuthFlow> auths = null, OpenApiReaderSettings settings = null)
        {
            using var sr = new StreamReader(location);

            var openAPIDoc =  new OpenApiStreamReader(settings).Read(sr.BaseStream, out var diagnostic);
            if (!diagnostic.Errors.IsNullOrEmpty())
                throw new Exception($"Error reading file. Error: {string.Join(", ", diagnostic.Errors)}");

            return openAPIDoc;
        }
    }
}
