// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;

namespace Microsoft.OpenApi.Readers
{
    /// <summary>
    /// Class containing logic to deserialize Open API V2 document into
    /// runtime Open API object model.
    /// </summary>
    internal static class OpenApiV2Deserializer
    {
        #region OpenApiObject

        public static FixedFieldMap<OpenApiDocument> OpenApiFixedFields = new FixedFieldMap<OpenApiDocument> {
            { "swagger", (o,n) => { /* Ignore it */} },
            { "info", (o,n) => o.Info = LoadInfo(n) },
            { "host", (o,n) => n.Context.SetTempStorage("host", n.GetScalarValue()) },
            { "basePath", (o,n) => n.Context.SetTempStorage("basePath",n.GetScalarValue()) },
            { "schemes", (o,n) => n.Context.SetTempStorage("schemes", n.CreateSimpleList<String>((s) => { return s.GetScalarValue(); })) },
            { "consumes", (o,n) => n.Context.SetTempStorage("globalconsumes", n.CreateSimpleList<String>((s) => s.GetScalarValue()))},
            { "produces", (o,n) => n.Context.SetTempStorage("globalproduces", n.CreateSimpleList<String>((s) => s.GetScalarValue()))},
            { "paths", (o,n) => o.Paths = LoadPaths(n) },
            { "definitions", (o,n) =>  o.Components.Schemas = n.CreateMapWithReference("#/definitions/",LoadSchema)  },
            { "parameters", (o,n) =>  o.Components.Parameters = n.CreateMapWithReference("#/parameters/",LoadParameter) },
            { "responses", (o,n) => o.Components.Responses = n.CreateMap(LoadResponse) },
            { "securityDefinitions", (o,n) => o.Components.SecuritySchemes = n.CreateMap(LoadSecurityScheme) },
            { "security", (o,n) => o.SecurityRequirements = n.CreateList(LoadSecurityRequirement)},
            { "tags", (o,n) => o.Tags = n.CreateList(LoadTag)},
            { "externalDocs", (o,n) => o.ExternalDocs = LoadExternalDocs(n) }
        };

        private static void MakeServers(IList<OpenApiServer> servers, ParsingContext context)
        {
            string host = context.GetTempStorage<string>("host");
            string basePath = context.GetTempStorage<string>("basePath");
            List<string> schemes = context.GetTempStorage<List<string>>("schemes");

            if (schemes != null)
            {
                foreach (var scheme in schemes)
                {
                    var server = new OpenApiServer();
                    server.Url = scheme + "://" + (host ?? "example.org/") + (basePath ?? "/");
                    servers.Add(server);
                }
            }
        }

        public static PatternFieldMap<OpenApiDocument> OpenApiPatternFields = new PatternFieldMap<OpenApiDocument>
        {
            // We have no semantics to verify X- nodes, therefore treat them as just values.
            { (s)=> s.StartsWith("x-"), (o,k,n)=> o.Extensions.Add(k, new OpenApiString(n.GetScalarValue())) }
        };

        public static OpenApiDocument LoadOpenApi(RootNode rootNode)
        {
            var openApidoc = new OpenApiDocument();

            var openApiNode = rootNode.GetMap();

            var required = new List<string>() { "info", "swagger", "paths" };

            ParseMap(openApiNode, openApidoc, OpenApiFixedFields, OpenApiPatternFields, required);

            ReportMissing(openApiNode, required);

            // Post Process OpenApi Object
            MakeServers(openApidoc.Servers, openApiNode.Context);

            return openApidoc;
        }

        #endregion

        #region InfoObject

        public static FixedFieldMap<OpenApiInfo> InfoFixedFields = new FixedFieldMap<OpenApiInfo>
        {
            { "title",      (o,n) => { o.Title = n.GetScalarValue(); } },
            { "description", (o,n) => { o.Description = n.GetScalarValue(); } },
            { "termsOfService", (o,n) => { o.TermsOfService = new Uri(n.GetScalarValue()); } },
            { "contact",    (o,n) => { o.Contact = LoadContact(n); } },
            { "license",    (o,n) => { o.License = LoadLicense(n); } },
            { "version",    (o,n) => { o.Version = new Version(n.GetScalarValue()); } }
        };

        public static PatternFieldMap<OpenApiInfo> InfoPatternFields = new PatternFieldMap<OpenApiInfo>
        {
            { (s)=> s.StartsWith("x-"), (o,k,n)=> o.Extensions.Add(k,  new OpenApiString(n.GetScalarValue())) }
        };


        public static OpenApiInfo LoadInfo(ParseNode node)
        {
            var mapNode = node.CheckMapNode("Info");

            var info = new OpenApiInfo();
            var required = new List<string>() { "title", "version" };

            ParseMap(mapNode, info, InfoFixedFields, InfoPatternFields, required);

            ReportMissing(node, required);

            return info;
        }


        #endregion

        #region ContactObject

        public static FixedFieldMap<OpenApiContact> ContactFixedFields = new FixedFieldMap<OpenApiContact> {
            { "name", (o,n) => { o.Name = n.GetScalarValue(); } },
            { "url", (o,n) => { o.Url = new Uri(n.GetScalarValue()); } },
            { "email", (o,n) => { o.Email = n.GetScalarValue(); } },
        };

        public static PatternFieldMap<OpenApiContact> ContactPatternFields = new PatternFieldMap<OpenApiContact>
        {
            { (s)=> s.StartsWith("x-"), (o,k,n)=> o.Extensions.Add(k,  new OpenApiString(n.GetScalarValue())) }
        };

        public static OpenApiContact LoadContact(ParseNode node)
        {
            var mapNode = node as MapNode;
            var contact = new OpenApiContact();

            ParseMap(mapNode, contact, ContactFixedFields,ContactPatternFields);

            return contact;
        }

        #endregion

        #region LicenseObject

        public static FixedFieldMap<OpenApiLicense> LicenseFixedFields = new FixedFieldMap<OpenApiLicense> {
            { "name", (o,n) => { o.Name = n.GetScalarValue(); } },
            { "url", (o,n) => { o.Url = new Uri(n.GetScalarValue(), UriKind.RelativeOrAbsolute); } },
        };

        public static PatternFieldMap<OpenApiLicense> LicensePatternFields = new PatternFieldMap<OpenApiLicense>
        {
            { (s)=> s.StartsWith("x-"), (o,k,n)=> o.Extensions.Add(k,  new OpenApiString(n.GetScalarValue())) }
        };

        internal static OpenApiLicense LoadLicense(ParseNode node)
        {
            var mapNode = node.CheckMapNode("OpenApiLicense");

            var license = new OpenApiLicense();

            ParseMap(mapNode, license, LicenseFixedFields, LicensePatternFields);

            return license;
        }

        #endregion

        #region PathsObject

        public static FixedFieldMap<OpenApiPaths> PathsFixedFields = new FixedFieldMap<OpenApiPaths>
        {
        };

        public static PatternFieldMap<OpenApiPaths> PathsPatternFields = new PatternFieldMap<OpenApiPaths>
        {
            { (s)=> s.StartsWith("/"), (o,k,n)=> o.Add(k, OpenApiV2Deserializer.LoadPathItem(n)    ) },
            { (s)=> s.StartsWith("x-"), (o,k,n)=> o.Extensions.Add(k, new OpenApiString(n.GetScalarValue())) }
        };

        public static OpenApiPaths LoadPaths(ParseNode node)
        {
            MapNode mapNode = node.CheckMapNode("Paths");

            OpenApiPaths domainObject = new OpenApiPaths();

            ParseMap(mapNode, domainObject, PathsFixedFields, PathsPatternFields);

            return domainObject;
        }
        #endregion

        #region PathItemObject

        private static FixedFieldMap<OpenApiPathItem> PathItemFixedFields = new FixedFieldMap<OpenApiPathItem>
        {
            { "$ref", (o,n) => { /* Not supported yet */} },
            { "get", (o, n) => o.AddOperation(OperationType.Get, LoadOperation(n)) },
            { "put", (o, n) => o.AddOperation(OperationType.Put, LoadOperation(n)) },
            { "post", (o, n) => o.AddOperation(OperationType.Post, LoadOperation(n)) },
            { "delete", (o, n) => o.AddOperation(OperationType.Delete, LoadOperation(n)) },
            { "options", (o, n) => o.AddOperation(OperationType.Options, LoadOperation(n)) },
            { "head", (o, n) => o.AddOperation(OperationType.Head, LoadOperation(n)) },
            { "patch", (o, n) => o.AddOperation(OperationType.Patch, LoadOperation(n)) },
            { "parameters", (o,n) => { o.Parameters = n.CreateList(OpenApiV2Deserializer.LoadParameter); } },
        };

        private static PatternFieldMap<OpenApiPathItem> PathItemPatternFields = new PatternFieldMap<OpenApiPathItem>
        {
            { (s)=> s.StartsWith("x-"), (o, p, n)=> o.AddExtension(p, n.CreateAny()) },
        };


        public static OpenApiPathItem LoadPathItem(ParseNode node)
        {
            var mapNode = node.CheckMapNode("PathItem");

            var pathItem = new OpenApiPathItem();

            ParseMap(mapNode, pathItem, PathItemFixedFields, PathItemPatternFields);

            return pathItem;
        }

        #endregion

        #region OperationObject

        private static FixedFieldMap<OpenApiOperation> OperationFixedFields = new FixedFieldMap<OpenApiOperation>
        {
            { "tags", (o,n) => o.Tags = n.CreateSimpleList((valueNode) => 
                OpenApiReferenceService.LoadTagByReference(
                    valueNode.Context, 
                    valueNode.Diagnostic, 
                    valueNode.GetScalarValue()))},
            { "summary", (o,n) => { o.Summary = n.GetScalarValue(); } },
            { "description", (o,n) => { o.Description = n.GetScalarValue(); } },
            { "externalDocs", (o,n) => { o.ExternalDocs = LoadExternalDocs(n); } },
            { "operationId", (o,n) => { o.OperationId = n.GetScalarValue(); } },
            { "parameters", (o,n) => { o.Parameters = n.CreateList(LoadParameter); } },
            { "consumes", (o,n) => n.Context.SetTempStorage(
                "operationconsumes",
                n.CreateSimpleList<String>((s) => s.GetScalarValue()))},
            { "produces", (o,n) => n.Context.SetTempStorage(
                "operationproduces",
                n.CreateSimpleList<String>((s) => s.GetScalarValue()))},
            { "responses", (o,n) => { o.Responses = LoadResponses(n); } },
            { "deprecated", (o,n) => { o.Deprecated = bool.Parse(n.GetScalarValue()); } },
            { "security", (o,n) => { o.Security = n.CreateList(LoadSecurityRequirement); } },
          };

        private static PatternFieldMap<OpenApiOperation> OperationPatternFields = new PatternFieldMap<OpenApiOperation>
        {
            { (s)=> s.StartsWith("x-"), (o,k,n)=> o.Extensions.Add(k, new OpenApiString(n.GetScalarValue())) },
        };

        internal static OpenApiOperation LoadOperation(ParseNode node)
        {
            var mapNode = node.CheckMapNode("OpenApiOperation");

            OpenApiOperation operation = new OpenApiOperation();

            ParseMap(mapNode, operation, OperationFixedFields, OperationPatternFields);

            // Build request body based on information determined while parsing OpenApiOperation
            var bodyParameter = node.Context.GetTempStorage<OpenApiParameter>("bodyParameter");
            if (bodyParameter != null)
            {
                operation.RequestBody = CreateRequestBody(node.Context, bodyParameter);
            }
            else
            {
                var formParameters = node.Context.GetTempStorage<List<OpenApiParameter>>("formParameters");
                if (formParameters != null)
                {
                    operation.RequestBody = CreateFormBody(formParameters);
                }
            }

            return operation;
        }

        #region Responses Object

        public static FixedFieldMap<OpenApiResponses> ResponsesFixedFields = new FixedFieldMap<OpenApiResponses>
        {
        };

        public static PatternFieldMap<OpenApiResponses> ResponsesPatternFields = new PatternFieldMap<OpenApiResponses>
        {
            { (s)=> !s.StartsWith("x-"), (o, p, n)=> o.Add(p, LoadResponse(n)) },
            { (s)=> s.StartsWith("x-"), (o, p, n)=> o.AddExtension(p, n.CreateAny()) }
        };

        public static OpenApiResponses LoadResponses(ParseNode node)
        {
            MapNode mapNode = node.CheckMapNode("Responses");

            OpenApiResponses domainObject = new OpenApiResponses();

            ParseMap(mapNode, domainObject, ResponsesFixedFields, ResponsesPatternFields);

            return domainObject;
        }

        #endregion

        private static OpenApiRequestBody CreateFormBody(List<OpenApiParameter> formParameters)
        {
                var mediaType = new OpenApiMediaType()
                {
                    Schema = new OpenApiSchema()
                    {
                        Properties = formParameters.ToDictionary(k => k.Name, v => v.Schema)
                    }
                };

                var formBody = new OpenApiRequestBody()
                {
                    Content = new Dictionary<string, OpenApiMediaType>() {
                            { "application/x-www-form-urlencoded", mediaType } }
                };

                return formBody;
        }

        private static OpenApiRequestBody CreateRequestBody(ParsingContext context, OpenApiParameter bodyParameter)
        {
            var consumes = context.GetTempStorage<List<string>>("operationproduces")
                      ?? context.GetTempStorage<List<string>>("globalproduces")
                      ?? new List<string>() { "application/json" };

            var requestBody = new OpenApiRequestBody()
            {
                Description = bodyParameter.Description,
                Required = bodyParameter.Required,
                Content = consumes.ToDictionary(k => k, v => new OpenApiMediaType()
                {
                    Schema = bodyParameter.Schema  // Should we clone this?
                })
            };

            return requestBody;
        }

        #endregion

        #region ExternalDocsObject

        private static FixedFieldMap<OpenApiExternalDocs> ExternalDocsFixedFields = new FixedFieldMap<OpenApiExternalDocs>
        {
            // $ref
            { "description", (o,n) => { o.Description = n.GetScalarValue(); } },
            { "url", (o,n) => { o.Url = new Uri(n.GetScalarValue()); } },
        };

        private static PatternFieldMap<OpenApiExternalDocs> ExternalDocsPatternFields = new PatternFieldMap<OpenApiExternalDocs>
        {
        };


        public static OpenApiExternalDocs LoadExternalDocs(ParseNode node)
        {
            var mapNode = node.CheckMapNode("externalDocs");

            var externalDocs = new OpenApiExternalDocs();

            ParseMap(mapNode, externalDocs, ExternalDocsFixedFields, ExternalDocsPatternFields);

            return externalDocs;
        }

        #endregion

        #region ParameterObject

        private static FixedFieldMap<OpenApiParameter> ParameterFixedFields = new FixedFieldMap<OpenApiParameter>
        {
            { "name",           (o,n) => { o.Name = n.GetScalarValue(); } },
            { "in",             (o,n) => { ProcessIn(o,n); } },
            { "description",    (o,n) => { o.Description = n.GetScalarValue(); } },
            { "required",       (o,n) => { o.Required = bool.Parse(n.GetScalarValue()); } },
            { "deprecated",     (o,n) => { o.Deprecated = bool.Parse(n.GetScalarValue()); } },
            { "allowEmptyValue", (o,n) => { o.AllowEmptyValue = bool.Parse(n.GetScalarValue()); } },
            { "example",        (o,n) => { o.Example = n.GetScalarValue(); } },
            { "type", (o,n) => { GetOrCreateSchema(o).Type = n.GetScalarValue(); } },
            { "items", (o,n) => { GetOrCreateSchema(o).Items = LoadSchema(n); } },
            { "collectionFormat", (o,n) => { LoadStyle(o,n.GetScalarValue());  } },
            { "format", (o,n) => { GetOrCreateSchema(o).Format = n.GetScalarValue(); } },
            { "minimum", (o,n) => { GetOrCreateSchema(o).Minimum = decimal.Parse(n.GetScalarValue()); } },
            { "maximum", (o,n) => { GetOrCreateSchema(o).Maximum = decimal.Parse(n.GetScalarValue()); } },
            { "maxLength", (o,n) => { GetOrCreateSchema(o).MaxLength = int.Parse(n.GetScalarValue()); } },
            { "minLength", (o,n) => { GetOrCreateSchema(o).MinLength = int.Parse(n.GetScalarValue()); } },
            { "readOnly", (o,n) => { GetOrCreateSchema(o).ReadOnly = bool.Parse(n.GetScalarValue()); } },
            { "default", (o,n) => { GetOrCreateSchema(o).Default =  new OpenApiString(n.GetScalarValue()); } },
            { "pattern", (o,n) => { GetOrCreateSchema(o).Pattern = n.GetScalarValue(); } },
            { "enum", (o,n) => { GetOrCreateSchema(o).Enum = n.CreateSimpleList<IOpenApiAny>(l=> new OpenApiString(l.GetScalarValue()) ); } },
            { "schema", (o,n) => { o.Schema = LoadSchema(n); } },
        };

        private static void LoadStyle(OpenApiParameter p, string v)
        {
            switch (v)
            {
                case "csv":
                    p.Style = "simple";
                    return;
                case "ssv":
                    p.Style = "spaceDelimited";
                    return;
                case "pipes":
                    p.Style = "pipeDelimited";
                    return;
                case "tsv":
                    throw new NotSupportedException();
                case "multi":
                    p.Style = "form";
                    p.Explode = true;
                    return;
            }
        }

        private static OpenApiSchema GetOrCreateSchema(OpenApiParameter p)
        {
            if (p.Schema == null)
            {
                p.Schema = new OpenApiSchema();
            }
            return p.Schema;
        }

        private static OpenApiSchema GetOrCreateSchema(OpenApiHeader p)
        {
            if (p.Schema == null)
            {
                p.Schema = new OpenApiSchema();
            }
            return p.Schema;
        }

        private static void ProcessIn(OpenApiParameter o, ParseNode n)
        {

            string value = n.GetScalarValue();
            switch(value)
            {
                case "body":
                    n.Context.SetTempStorage("bodyParameter",o);
                    break;
                case "form":
                    var formParameters = n.Context.GetTempStorage<List<OpenApiParameter>>("formParameters");
                    if (formParameters == null)
                    {
                        formParameters = new List<OpenApiParameter>();
                        n.Context.SetTempStorage("formParameters", formParameters);
                    }
                    formParameters.Add(o);
                    break;
                default:
                    o.In = (ParameterLocation)Enum.Parse(typeof(ParameterLocation), value);
                    break;
            }
            
        }

        private static PatternFieldMap<OpenApiParameter> ParameterPatternFields = new PatternFieldMap<OpenApiParameter>
        {
            { (s)=> s.StartsWith("x-"), (o,k,n)=> o.Extensions.Add(k, new OpenApiString(n.GetScalarValue())) },
        };


        public static OpenApiParameter LoadParameter(ParseNode node)
        {
            var mapNode = node.CheckMapNode("parameter");

            var refpointer = mapNode.GetReferencePointer();
            if (refpointer != null)
            {
                return mapNode.GetReferencedObject<OpenApiParameter>(refpointer);
            }

            var parameter = new OpenApiParameter();

            ParseMap(mapNode, parameter, ParameterFixedFields, ParameterPatternFields);

            var schema = node.Context.GetTempStorage<OpenApiSchema>("schema");
            if (schema != null)
            {
                parameter.Schema = schema;
                node.Context.SetTempStorage("schema", null);
            }

            if (parameter.In == 0)
            {
                return null; // Don't include Form or Body parameters in OpenApiOperation.Parameters list
            }

            return parameter;
        }
        #endregion
        
        #region ResponseObject

        private static FixedFieldMap<OpenApiResponse> ResponseFixedFields = new FixedFieldMap<OpenApiResponse>
        {
            { "description", (o,n) => { o.Description = n.GetScalarValue(); } },
            { "headers", (o,n) => { o.Headers = n.CreateMap(LoadHeader); } },
            { "examples",       (o,n) => { /*o.Examples = ((ListNode)n).Select(s=> new AnyNode(s)).ToList();*/ } },
            { "schema", (o,n) => { n.Context.SetTempStorage("operationschema", LoadSchema(n)); } },

        };

        private static PatternFieldMap<OpenApiResponse> ResponsePatternFields = new PatternFieldMap<OpenApiResponse>
        {
            { (s)=> s.StartsWith("x-"), (o,k,n)=> o.Extensions.Add(k,  new OpenApiString(n.GetScalarValue())) },
        };

        public static OpenApiResponse LoadResponse(ParseNode node)
        {
            var mapNode = node.CheckMapNode("response");

            var response = new OpenApiResponse();
            foreach (var property in mapNode)
            {
                property.ParseField(response, ResponseFixedFields, ResponsePatternFields);
            }

            ProcessProduces(response, node.Context);

            return response;
        }

        private static void ProcessProduces(OpenApiResponse response, ParsingContext context)
        {

            var produces = context.GetTempStorage<List<string>>("operationproduces") 
                  ?? context.GetTempStorage<List<string>>("globalproduces") 
                  ?? new List<string>() { "application/json" };

            response.Content = new Dictionary<string, OpenApiMediaType>();
            foreach (var mt in produces)
            {
                var schema = context.GetTempStorage<OpenApiSchema>("operationschema");
                OpenApiMediaType mediaType = null;
                if (schema != null)
                {
                    mediaType = new OpenApiMediaType()
                    {
                        Schema = schema
                    };
                }
                response.Content.Add(mt, mediaType);
            }
        }

        #endregion

        #region HeaderObject
        private static FixedFieldMap<OpenApiHeader> HeaderFixedFields = new FixedFieldMap<OpenApiHeader>
        {
            { "description", (o,n) => { o.Description = n.GetScalarValue(); } },
            { "required", (o,n) => { o.Required = bool.Parse(n.GetScalarValue()); } },
            { "deprecated", (o,n) => { o.Deprecated = bool.Parse(n.GetScalarValue()); } },
            { "allowReserved", (o,n) => { o.AllowReserved = bool.Parse(n.GetScalarValue()); } },
            { "style", (o,n) => { o.Style = n.GetScalarValue(); } },
            { "type", (o,n) => { GetOrCreateSchema(o).Type = n.GetScalarValue(); } },
            { "format", (o,n) => { GetOrCreateSchema(o).Format = n.GetScalarValue(); } },

        };

        private static PatternFieldMap<OpenApiHeader> HeaderPatternFields = new PatternFieldMap<OpenApiHeader>
        {
            { (s)=> s.StartsWith("x-"), (o,k,n)=> o.Extensions.Add(k,  new OpenApiString(n.GetScalarValue())) },
        };


        public static OpenApiHeader LoadHeader(ParseNode node)
        {
            var mapNode = node.CheckMapNode("header");
            var header = new OpenApiHeader();
            foreach (var property in mapNode)
            {
                property.ParseField(header, HeaderFixedFields, HeaderPatternFields);
            }

            var schema = node.Context.GetTempStorage<OpenApiSchema>("schema");
            if (schema != null)
            {
                header.Schema = schema;
                node.Context.SetTempStorage("schema", null);
            }

            return header;
        }

        #endregion

        #region ExampleObject
        private static FixedFieldMap<OpenApiExample> ExampleFixedFields = new FixedFieldMap<OpenApiExample>
        {
        };

        private static PatternFieldMap<OpenApiExample> ExamplePatternFields = new PatternFieldMap<OpenApiExample>
        {
            { (s)=> s.StartsWith("x-"), (o,k,n)=> o.Extensions.Add(k,  new OpenApiString(n.GetScalarValue())) }
        };

        public static OpenApiExample LoadExample(ParseNode node)
        {
            var mapNode = node.CheckMapNode("Example");
            var example = new OpenApiExample();
            foreach (var property in mapNode)
            {
                property.ParseField(example, ExampleFixedFields, ExamplePatternFields);
            }

            return example;
        }


        #endregion

        #region TagObject
        internal static OpenApiTag LoadTag(ParseNode n)
        {
            var mapNode = n.CheckMapNode("tag");

            var obj = new OpenApiTag();

            foreach (var node in mapNode)
            {
                var key = node.Name;
                switch (key)
                {
                    case "description":
                        obj.Description = node.Value.GetScalarValue();
                        break;
                    case "name":
                        obj.Name = node.Value.GetScalarValue();
                        break;

                }
            }
            return obj;
        }


        #endregion

        #region SchemaObject

        private static FixedFieldMap<OpenApiSchema> SchemaFixedFields = new FixedFieldMap<OpenApiSchema>
        {
                { "title", (o,n) => { o.Title = n.GetScalarValue();  } },
                { "multipleOf", (o,n) => { o.MultipleOf = decimal.Parse(n.GetScalarValue()); } },
                { "maximum", (o,n) => { o.Maximum = decimal.Parse(n.GetScalarValue()); } },
                { "exclusiveMaximum", (o,n) => { o.ExclusiveMaximum = bool.Parse(n.GetScalarValue()); } },
                { "minimum", (o,n) => { o.Minimum = decimal.Parse(n.GetScalarValue()); } },
                { "exclusiveMinimum", (o,n) => { o.ExclusiveMinimum = bool.Parse(n.GetScalarValue()); } },
                { "maxLength", (o,n) => { o.MaxLength = int.Parse(n.GetScalarValue()); } },
                { "minLength", (o,n) => { o.MinLength = int.Parse(n.GetScalarValue()); } },
                { "pattern", (o,n) => { o.Pattern = n.GetScalarValue(); } },
                { "maxItems", (o,n) => { o.MaxItems = int.Parse(n.GetScalarValue()); } },
                { "minItems", (o,n) => { o.MinItems = int.Parse(n.GetScalarValue()); } },
                { "uniqueItems", (o,n) => { o.UniqueItems = bool.Parse(n.GetScalarValue()); } },
                { "maxProperties", (o,n) => { o.MaxProperties = int.Parse(n.GetScalarValue()); } },
                { "minProperties", (o,n) => { o.MinProperties = int.Parse(n.GetScalarValue()); } },
                { "required", (o,n) => { o.Required = n.CreateSimpleList<string>(n2 => n2.GetScalarValue()).ToArray(); } },
                { "enum", (o,n) => { o.Enum =  n.CreateSimpleList<IOpenApiAny>((s)=> new OpenApiString(s.GetScalarValue())); } },

                { "type", (o,n) => { o.Type = n.GetScalarValue(); } },
                { "allOf", (o,n) => { o.AllOf = n.CreateList(LoadSchema); } },
                { "items", (o,n) => { o.Items = LoadSchema(n); } },
                { "properties", (o,n) => { o.Properties = n.CreateMap(LoadSchema); } },
                { "additionalProperties", (o,n) => { o.AdditionalProperties = LoadSchema(n); }
                                                     },
                { "description", (o,n) => { o.Type = n.GetScalarValue(); } },
                { "format", (o,n) => { o.Description = n.GetScalarValue(); } },
                { "default", (o,n) =>
                {
                    o.Default = new OpenApiString(n.GetScalarValue());
                } },

                // discriminator
                { "readOnly", (o,n) => { o.ReadOnly = bool.Parse(n.GetScalarValue()); } },
                // xml
                { "externalDocs", (o,n) => { o.ExternalDocs = LoadExternalDocs(n); } },
                { "example", (o,n) => { o.Example =new OpenApiString( n.GetScalarValue()); } },


        };

        private static PatternFieldMap<OpenApiSchema> SchemaPatternFields = new PatternFieldMap<OpenApiSchema>
        {
            { (s)=> s.StartsWith("x-"), (o,k,n)=> o.Extensions.Add(k, new OpenApiString(n.GetScalarValue())) }
        };


        public static OpenApiSchema LoadSchema(ParseNode node)
        {
            MapNode mapNode = node.CheckMapNode("schema");

            var refpointer = mapNode.GetReferencePointer();
            if (refpointer != null)
            {
                return mapNode.GetReferencedObject<OpenApiSchema>(refpointer);
            }

            var domainObject = new OpenApiSchema();

            foreach (var propertyNode in mapNode)
            {
                propertyNode.ParseField(domainObject, SchemaFixedFields, SchemaPatternFields);
            }

            return domainObject;
        }


        #endregion

        #region SecuritySchemeObject

        private static FixedFieldMap<OpenApiSecurityScheme> SecuritySchemeFixedFields = new FixedFieldMap<OpenApiSecurityScheme>
        {
            { "type", (o,n) => o.Type = (SecuritySchemeType)Enum.Parse(typeof(SecuritySchemeType), n.GetScalarValue())  },
            { "description", (o,n) => o.Description = n.GetScalarValue() },
            { "name", (o,n) => o.Name = n.GetScalarValue() },
            { "in", (o,n) => o.In = (ParameterLocation)Enum.Parse(typeof(ParameterLocation), n.GetScalarValue()) },
            { "scheme", (o,n) => o.Scheme = n.GetScalarValue() },
            { "bearerFormat", (o,n) => o.BearerFormat = n.GetScalarValue() },
            { "openIdConnectUrl", (o,n) => o.OpenIdConnectUrl = new Uri(n.GetScalarValue(), UriKind.RelativeOrAbsolute) },
            { "flows", (o,n) => o.Flows = LoadOAuthFlows(n) }
        };

        private static PatternFieldMap<OpenApiSecurityScheme> SecuritySchemePatternFields = new PatternFieldMap<OpenApiSecurityScheme>
        {
            { (s)=> s.StartsWith("x-"), (o,k,n)=> o.Extensions.Add(k, new OpenApiString(n.GetScalarValue())) }
        };

        public static OpenApiSecurityScheme LoadSecurityScheme(ParseNode node)
        {
            var mapNode = node.CheckMapNode("securityScheme");

            var securityScheme = new OpenApiSecurityScheme();
            foreach (var property in mapNode)
            {
                property.ParseField(securityScheme, SecuritySchemeFixedFields, SecuritySchemePatternFields);
            }

            return securityScheme;
        }

        #endregion

        #region OAuthFlowsObject

        private static FixedFieldMap<OpenApiOAuthFlows> OAuthFlowsFixedFileds = new FixedFieldMap<OpenApiOAuthFlows>
        {
            { "implicit", (o,n) => o.Implicit = LoadOAuthFlow(n) },
            { "password", (o,n) => o.Password = LoadOAuthFlow(n) },
            { "clientCredentials", (o,n) => o.ClientCredentials = LoadOAuthFlow(n) },
            { "authorizationCode", (o,n) => o.AuthorizationCode = LoadOAuthFlow(n) }
        };

        private static PatternFieldMap<OpenApiOAuthFlows> OAuthFlowsPatternFields = new PatternFieldMap<OpenApiOAuthFlows>
        {
            { (s)=> s.StartsWith("x-"), (o,k,n)=> o.Extensions.Add(k, new OpenApiString(n.GetScalarValue())) }
        };

        public static OpenApiOAuthFlows LoadOAuthFlows(ParseNode node)
        {
            var mapNode = node.CheckMapNode("OAuthFlows");

            var oAuthFlows = new OpenApiOAuthFlows();
            foreach (var property in mapNode)
            {
                property.ParseField(oAuthFlows, OAuthFlowsFixedFileds, OAuthFlowsPatternFields);
            }

            return oAuthFlows;
        }
        #endregion

        #region OAuthFlowObject

        private static FixedFieldMap<OpenApiOAuthFlow> OAuthFlowFixedFileds = new FixedFieldMap<OpenApiOAuthFlow>
        {
            { "authorizationUrl", (o,n) => o.AuthorizationUrl = new Uri(n.GetScalarValue()) },
            { "tokenUrl", (o,n) => o.TokenUrl = new Uri(n.GetScalarValue()) },
            { "refreshUrl", (o,n) => o.RefreshUrl = new Uri(n.GetScalarValue()) },
            { "scopes", (o,n) => o.Scopes = n.CreateMap(LoadString) }
        };

        private static PatternFieldMap<OpenApiOAuthFlow> OAuthFlowPatternFields = new PatternFieldMap<OpenApiOAuthFlow>
        {
            { (s)=> s.StartsWith("x-"), (o,k,n)=> o.Extensions.Add(k, new OpenApiString(n.GetScalarValue())) }
        };

        public static OpenApiOAuthFlow LoadOAuthFlow(ParseNode node)
        {
            var mapNode = node.CheckMapNode("OAuthFlow");

            var oauthFlow = new OpenApiOAuthFlow();
            foreach (var property in mapNode)
            {
                property.ParseField(oauthFlow, OAuthFlowFixedFileds, OAuthFlowPatternFields);
            }

            return oauthFlow;
        }
        #endregion

        #region SecurityRequirement
        public static OpenApiSecurityRequirement LoadSecurityRequirement(ParseNode node)
        {
            var mapNode = node.CheckMapNode("security");

            var obj = new OpenApiSecurityRequirement();

            foreach (var property in mapNode)
            {
                var scheme = OpenApiReferenceService.LoadSecuritySchemeByReference(mapNode.Context, mapNode.Diagnostic, property.Name);
                if (scheme != null)
                {
                    obj.Schemes.Add(scheme, property.Value.CreateSimpleList<string>(n2 => n2.GetScalarValue()));
                } else
                {
                    node.Diagnostic.Errors.Add(new OpenApiError(node.Context.GetLocation(), $"Scheme {property.Name} is not found"));
                }
            }
            return obj;
        }

        #endregion

        public static OpenApiReference ParseReference(string pointer)
        {
            var pointerbits = pointer.Split('#');

            if (pointerbits.Length == 1)
            {
                return new OpenApiReference()
                {
                    ReferenceType = ReferenceType.Schema,
                    TypeName = pointerbits[0]
                }; 
            }
            else
            {
                var pointerParts = pointerbits[1].Split('/');
                return new OpenApiReference()
                {
                    ExternalFilePath = pointerbits[0],
                    ReferenceType = ParseReferenceTypeName(pointerParts[1]),
                    TypeName = pointerParts[2]
                };
            }

        }

        public static JsonPointer GetPointer(OpenApiReference reference)
        {
            return new JsonPointer("#/" + GetReferenceTypeName(reference.ReferenceType) + "/" + reference.TypeName); 
        }

        private static ReferenceType ParseReferenceTypeName(string referenceTypeName)
        {
            switch (referenceTypeName)
            {
                case "definitions": return ReferenceType.Schema;
                case "parameters": return ReferenceType.Parameter;
                case "responses": return ReferenceType.Response;
                case "headers": return ReferenceType.Header;
                case "tags": return ReferenceType.Tags;
                case "securityDefinitions": return ReferenceType.SecurityScheme;
                default: throw new ArgumentException();
            }
        }

        private static string GetReferenceTypeName(ReferenceType referenceType)
        {
            switch (referenceType)
            {
                case ReferenceType.Schema: return "definitions";
                case ReferenceType.Parameter: return "parameters";
                case ReferenceType.Response: return "responses";
                case ReferenceType.Header: return "headers";
                case ReferenceType.Tags: return "tags";
                case ReferenceType.SecurityScheme: return "securityDefinitions";
                default: throw new ArgumentException();
            }
        }


        public static IOpenApiReference LoadReference(OpenApiReference reference, object rootNode)
        {
            IOpenApiReference referencedObject = null;
            ParseNode node = ((RootNode)rootNode).Find(GetPointer(reference));

            if (node == null &&  reference.ReferenceType != ReferenceType.Tags ) return null;
            switch (reference.ReferenceType)
            {
                case ReferenceType.Schema:
                    referencedObject = LoadSchema(node);
                    break;
                case ReferenceType.Parameter:
                    referencedObject = LoadParameter(node);
                    break;
                case ReferenceType.SecurityScheme:
                    referencedObject = LoadSecurityScheme(node);
                    break;
                case ReferenceType.Tags:
                    ListNode list = (ListNode)node;
                    if (list != null)
                    {
                        foreach (var item in list)
                        {
                            var tag = LoadTag(item);

                            if (tag.Name == reference.TypeName)
                            {
                                return tag;
                            }
                        }
                    }
                    else
                    {
                        return new OpenApiTag() { Name = reference.TypeName };
                    }
                    break;

                default:
                    throw new OpenApiException($"Unknown $ref {reference.ReferenceType} at {reference.ToString()}");
            }

            return referencedObject;
        }

        private static void ParseMap<T>(MapNode mapNode, T domainObject, FixedFieldMap<T> fixedFieldMap, PatternFieldMap<T> patternFieldMap, List<string> requiredFields  = null)
        {
            if (mapNode == null) return;

            foreach (var propertyNode in mapNode)
            {
                propertyNode.ParseField<T>(domainObject, fixedFieldMap, patternFieldMap);
                requiredFields?.Remove(propertyNode.Name);
            }
        }

        private static void ReportMissing(ParseNode node, IList<string> required)
        {
            foreach ( var error in required.Select(r => new OpenApiError("", $"{r} is a required property")).ToList() )
            {
                node.Diagnostic.Errors.Add(error);
            }
        }

        private static string LoadString(ParseNode node)
        {
            return node.GetScalarValue();
        }
    }
}
