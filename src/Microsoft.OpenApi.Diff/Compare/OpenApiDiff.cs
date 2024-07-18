using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Diff.BusinessObjects;
using Microsoft.OpenApi.Diff.Extensions;
using Microsoft.OpenApi.Diff.Utils;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Diff.Compare
{
    public class OpenApiDiff
    {
        private readonly ILogger _logger;

        public string OldIdentifier { get; }
        public string NewIdentifier { get; }

        public PathsDiff PathsDiff { get; set; }
        public PathDiff PathDiff { get; set; }
        public SchemaDiff SchemaDiff { get; set; }
        public ContentDiff ContentDiff { get; set; }
        public ParametersDiff ParametersDiff { get; set; }
        public ParameterDiff ParameterDiff { get; set; }
        public RequestBodyDiff RequestBodyDiff { get; set; }
        public ResponseDiff ResponseDiff { get; set; }
        public HeadersDiff HeadersDiff { get; set; }
        public HeaderDiff HeaderDiff { get; set; }
        public ApiResponseDiff APIResponseDiff { get; set; }
        public OperationDiff OperationDiff { get; set; }
        public SecurityRequirementsDiff SecurityRequirementsDiff { get; set; }
        public SecurityRequirementDiff SecurityRequirementDiff { get; set; }
        public SecuritySchemeDiff SecuritySchemeDiff { get; set; }
        public OAuthFlowsDiff OAuthFlowsDiff { get; set; }
        public OAuthFlowDiff OAuthFlowDiff { get; set; }
        public ExtensionsDiff ExtensionsDiff { get; set; }
        public MetadataDiff MetadataDiff { get; set; }
        public OpenApiDocument OldSpecOpenApi { get; set; }
        public OpenApiDocument NewSpecOpenApi { get; set; }
        public List<EndpointBO> NewEndpoints { get; set; }
        public List<EndpointBO> MissingEndpoints { get; set; }
        public List<ChangedOperationBO> ChangedOperations { get; set; }
        public ChangedExtensionsBO ChangedExtensions { get; set; }

        public OpenApiDiff(OpenApiDocument oldSpecOpenApi, string oldSpecIdentifier, OpenApiDocument newSpecOpenApi, string newSpecIdentifier, IEnumerable<IExtensionDiff> extensions, ILogger logger)
        {
            _logger = logger;
            OldSpecOpenApi = oldSpecOpenApi;
            NewSpecOpenApi = newSpecOpenApi;
            OldIdentifier = oldSpecIdentifier;
            NewIdentifier = newSpecIdentifier;

            if (null == oldSpecOpenApi || null == newSpecOpenApi)
                throw new Exception("one of the old or new object is null");

            InitializeFields(extensions);
        }

        public static ChangedOpenApiBO Compare(OpenApiDocument oldSpecOpenApi, string oldSpecIdentifier, OpenApiDocument newSpecOpenApi, string newSpecIdentifier, IEnumerable<IExtensionDiff> extensions, ILogger logger)
        {
            return new OpenApiDiff(oldSpecOpenApi, oldSpecIdentifier, newSpecOpenApi, newSpecIdentifier, extensions, logger).Compare();
        }

        private void InitializeFields(IEnumerable<IExtensionDiff> extensions)
        {
            PathsDiff = new PathsDiff(this);
            PathDiff = new PathDiff(this);
            SchemaDiff = new SchemaDiff(this);
            ContentDiff = new ContentDiff(this);
            ParametersDiff = new ParametersDiff(this);
            ParameterDiff = new ParameterDiff(this);
            RequestBodyDiff = new RequestBodyDiff(this);
            ResponseDiff = new ResponseDiff(this);
            HeadersDiff = new HeadersDiff(this);
            HeaderDiff = new HeaderDiff(this);
            APIResponseDiff = new ApiResponseDiff(this);
            OperationDiff = new OperationDiff(this);
            SecurityRequirementsDiff = new SecurityRequirementsDiff(this);
            SecurityRequirementDiff = new SecurityRequirementDiff(this);
            SecuritySchemeDiff = new SecuritySchemeDiff(this);
            OAuthFlowsDiff = new OAuthFlowsDiff(this);
            OAuthFlowDiff = new OAuthFlowDiff(this);
            ExtensionsDiff = new ExtensionsDiff(this, extensions);
            MetadataDiff = new MetadataDiff(this);
        }

        private ChangedOpenApiBO Compare()
        {
            PreProcess(OldSpecOpenApi);
            PreProcess(NewSpecOpenApi);
            var paths =
                PathsDiff.Diff(PathsDiff.ValOrEmpty(OldSpecOpenApi.Paths), PathsDiff.ValOrEmpty(NewSpecOpenApi.Paths));
            NewEndpoints = new List<EndpointBO>();
            MissingEndpoints = new List<EndpointBO>();
            ChangedOperations = new List<ChangedOperationBO>();

            if (paths != null)
            {
                NewEndpoints = EndpointUtils.ConvertToEndpointList<EndpointBO>(paths.Increased);
                MissingEndpoints = EndpointUtils.ConvertToEndpointList<EndpointBO>(paths.Missing);
                foreach (var (key, value) in paths.Changed)
                {
                    NewEndpoints.AddRange(EndpointUtils.ConvertToEndpoints<EndpointBO>(key, value.Increased));
                    MissingEndpoints.AddRange(EndpointUtils.ConvertToEndpoints<EndpointBO>(key, value.Missing));
                    ChangedOperations.AddRange(value.Changed);
                }
            }

            var diff = ExtensionsDiff
                .Diff(OldSpecOpenApi.Extensions, NewSpecOpenApi.Extensions);

            if (diff != null)
                ChangedExtensions = diff;
            return GetChangedOpenApi();
        }

        private static void PreProcess(OpenApiDocument openApi)
        {
            var securityRequirements = openApi.SecurityRequirements;

            if (securityRequirements != null)
            {
                var distinctSecurityRequirements =
                    securityRequirements.Distinct().ToList();
                var paths = openApi.Paths;
                if (paths != null)
                {
                    foreach (var openApiPathItem in paths.Values)
                    {
                        var operationsWithSecurity = openApiPathItem
                            .Operations
                            .Values
                            .Where(x => !x.Security.IsNullOrEmpty());
                        foreach (var openApiOperation in operationsWithSecurity)
                        {
                            openApiOperation.Security = openApiOperation.Security.Distinct().ToList();
                        }
                        var operationsWithoutSecurity = openApiPathItem
                            .Operations
                            .Values
                            .Where(x => x.Security.IsNullOrEmpty());
                        foreach (var openApiOperation in operationsWithoutSecurity)
                        {
                            openApiOperation.Security = distinctSecurityRequirements;
                        }
                    }
                }

                openApi.SecurityRequirements = null;
            }
        }

        private ChangedOpenApiBO GetChangedOpenApi()
        {
            return new ChangedOpenApiBO(OldIdentifier, NewIdentifier)
            {
                MissingEndpoints = MissingEndpoints,
                NewEndpoints = NewEndpoints,
                NewSpecOpenApi = NewSpecOpenApi,
                OldSpecOpenApi = OldSpecOpenApi,
                ChangedOperations = ChangedOperations,
                ChangedExtensions = ChangedExtensions
            };
        }
    }
}
