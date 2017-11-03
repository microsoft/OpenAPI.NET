﻿// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Commons;
using Microsoft.OpenApi.Expressions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;

namespace Microsoft.OpenApi.Readers
{
    /// <summary>
    /// Class containing logic to deserialize Open API V3 document into
    /// runtime Open API object model.
    /// </summary>
    internal static class OpenApiV3Deserializer
    {
        #region OpenApiObject
        public static FixedFieldMap<OpenApiDocument> OpenApiFixedFields = new FixedFieldMap<OpenApiDocument> {
            { "openapi", (o,n) => { o.SpecVersion = new Version(n.GetScalarValue()); } },
            { "info", (o,n) => o.Info = LoadInfo(n) },
            { "servers", (o,n) => o.Servers = n.CreateList(LoadServer) },
            { "paths", (o,n) => o.Paths = LoadPaths(n) },
            { "components", (o,n) => o.Components = LoadComponents(n) },
            { "tags", (o,n) => o.Tags = n.CreateList(LoadTag)},
            { "externalDocs", (o,n) => o.ExternalDocs = LoadExternalDocs(n) },
            { "security", (o,n) => o.SecurityRequirements = n.CreateList(LoadSecurityRequirement)}

            };

        public static PatternFieldMap<OpenApiDocument> OpenApiPatternFields = new PatternFieldMap<OpenApiDocument>
        {
            // We have no semantics to verify X- nodes, therefore treat them as just values.
            { (s)=> s.StartsWith("x-"), (o,k,n)=> o.Extensions.Add(k, new OpenApiString(n.GetScalarValue())) }
        };

        public static OpenApiDocument LoadOpenApi(RootNode rootNode)
        {
            var openApidoc = new OpenApiDocument();

            var openApiNode = rootNode.GetMap();

            var required = new List<string>() { "info", "openapi", "paths" };

            ParseMap(openApiNode, openApidoc, OpenApiFixedFields, OpenApiPatternFields, required);

            ReportMissing(openApiNode, required);

            return openApidoc;
        }

        #endregion

        #region InfoObject

        public static FixedFieldMap<OpenApiInfo> InfoFixedFields = new FixedFieldMap<OpenApiInfo>
        {
            { "title",      (o,n) => { o.Title = n.GetScalarValue(); } },
            { "version",    (o,n) => { o.Version = new Version(n.GetScalarValue()); } },
            { "description", (o,n) => { o.Description = n.GetScalarValue(); } },
            { "termsOfService", (o,n) => { o.TermsOfService = new Uri(n.GetScalarValue()); } },
            { "contact",    (o,n) => { o.Contact = LoadContact(n); } },
            { "license",    (o,n) => { o.License = LoadLicense(n); } }
        };

        public static PatternFieldMap<OpenApiInfo> InfoPatternFields = new PatternFieldMap<OpenApiInfo>
        {
            { (s)=> s.StartsWith("x-"), (o,k,n)=> o.AddExtension(k, n.CreateAny()) }
        };

        public static OpenApiInfo LoadInfo(ParseNode node)
        {
            var mapNode = node.CheckMapNode("Info");

            var info = new OpenApiInfo();
            var required = new List<string>() { "title", "version" };

            ParseMap(mapNode, info, InfoFixedFields, InfoPatternFields, required);

            return info;
        }


        #endregion

        #region ContactObject

        public static FixedFieldMap<OpenApiContact> ContactFixedFields = new FixedFieldMap<OpenApiContact> {
            { "name", (o,n) => { o.Name = n.GetScalarValue(); } },
            { "email", (o,n) => { o.Email = n.GetScalarValue(); } },
            { "url", (o,n) => { o.Url = new Uri(n.GetScalarValue()); } },
        };

        public static PatternFieldMap<OpenApiContact> ContactPatternFields = new PatternFieldMap<OpenApiContact>
        {
            { (s)=> s.StartsWith("x-"), (o,k,n)=> o.Extensions.Add(k, new OpenApiString(n.GetScalarValue())) }
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
            { "url", (o,n) => { o.Url = new Uri(n.GetScalarValue()); } },
        };

        public static PatternFieldMap<OpenApiLicense> LicensePatternFields = new PatternFieldMap<OpenApiLicense>
        {
            { (s)=> s.StartsWith("x-"), (o,k,n)=> o.Extensions.Add(k, new OpenApiString(n.GetScalarValue())) }
        };

        internal static OpenApiLicense LoadLicense(ParseNode node)
        {
            var mapNode = node.CheckMapNode("License");

            var license = new OpenApiLicense();

            ParseMap(mapNode, license, LicenseFixedFields, LicensePatternFields);

            return license;
        }

        #endregion

        #region ServerObject

        private static FixedFieldMap<OpenApiServer> ServerFixedFields = new FixedFieldMap<OpenApiServer>
        {
            { "url", (o,n) => { o.Url=  n.GetScalarValue(); } },
            { "description", (o,n) => { o.Description = n.GetScalarValue();  } },
            { "variables", (o,n) => {  o.Variables = n.CreateMap(LoadServerVariable); } }
        };

        private static PatternFieldMap<OpenApiServer> ServerPatternFields = new PatternFieldMap<OpenApiServer>
        {
            { (s)=> s.StartsWith("x-"), (o,k,n)=> o.Extensions.Add(k, new OpenApiString(n.GetScalarValue())) }
        };

        public static OpenApiServer LoadServer(ParseNode node)
        {
            var mapNode = node.CheckMapNode("server");

            var server = new OpenApiServer();

            ParseMap(mapNode, server, ServerFixedFields, ServerPatternFields);

            return server;
        }

        #endregion

        #region ServerVariable

        private static FixedFieldMap<OpenApiServerVariable> ServerVariableFixedFields = new FixedFieldMap<OpenApiServerVariable>
        {
            { "enum", (o,n) => { o.Enum =  n.CreateSimpleList<string>((s)=> s.GetScalarValue()); } },
            { "default", (o,n) => { o.Default =  n.GetScalarValue(); } },
            { "description", (o,n) => { o.Description = n.GetScalarValue();  } },
        };

        private static PatternFieldMap<OpenApiServerVariable> ServerVariablePatternFields = new PatternFieldMap<OpenApiServerVariable>
        {
            { (s)=> s.StartsWith("x-"), (o,k,n)=> o.Extensions.Add(k, new OpenApiString(n.GetScalarValue())) }
        };

        public static OpenApiServerVariable LoadServerVariable(ParseNode node)
        {
            var mapNode = node.CheckMapNode("serverVariable");

            var serverVariable = new OpenApiServerVariable();

            ParseMap(mapNode, serverVariable, ServerVariableFixedFields, ServerVariablePatternFields);

            return serverVariable;
        }

        #endregion

        #region ComponentsObject

        public static FixedFieldMap<OpenApiComponents> ComponentsFixedFields = new FixedFieldMap<OpenApiComponents> {
            { "schemas", (o,n) => { o.Schemas = n.CreateMap(LoadSchema); } },
            { "responses", (o,n) => o.Responses = n.CreateMap(LoadResponse) },
            { "parameters", (o,n) => o.Parameters = n.CreateMap(LoadParameter) },
            { "examples", (o,n) => o.Examples = n.CreateMap(LoadExample) },
            { "requestBodies", (o,n) => o.RequestBodies = n.CreateMap(LoadRequestBody) },
            { "headers", (o,n) => o.Headers = n.CreateMap(LoadHeader) },
            { "securitySchemes", (o,n) => o.SecuritySchemes = n.CreateMap(LoadSecurityScheme) },
            { "links", (o,n) => o.Links = n.CreateMap(LoadLink) },
            { "callbacks", (o,n) => o.Callbacks = n.CreateMap(LoadCallback) },
         };

        public static PatternFieldMap<OpenApiComponents> ComponentsPatternFields = new PatternFieldMap<OpenApiComponents>
        {
            { (s)=> s.StartsWith("x-"), (o,k,n)=> o.Extensions.Add(k, new OpenApiString(n.GetScalarValue())) }
        };

        public static OpenApiComponents LoadComponents(ParseNode node)
        {
            var mapNode = node.CheckMapNode("components");
            var components = new OpenApiComponents();

            ParseMap(mapNode, components, ComponentsFixedFields, ComponentsPatternFields);

            return components;
        }

        #endregion

        #region PathsObject

        public static FixedFieldMap<OpenApiPaths> PathsFixedFields = new FixedFieldMap<OpenApiPaths>
        {
        };

        public static PatternFieldMap<OpenApiPaths> PathsPatternFields = new PatternFieldMap<OpenApiPaths>
        {
            { (s)=> s.StartsWith("/"), (o,k,n)=> o.Add(k, LoadPathItem(n)    ) },
            { (s)=> s.StartsWith("x-"), (o,k,n)=> o.Extensions.Add(k, new OpenApiString(n.GetScalarValue())) }
        };

        public static OpenApiPaths LoadPaths(ParseNode node)
        {
            var mapNode = node.CheckMapNode("Paths");

            OpenApiPaths domainObject = new OpenApiPaths();

            ParseMap(mapNode, domainObject, PathsFixedFields, PathsPatternFields);

            return domainObject;
        }
        #endregion

        #region PathItemObject

        private static FixedFieldMap<OpenApiPathItem> PathItemFixedFields = new FixedFieldMap<OpenApiPathItem>
        {
            // $ref
            { "summary", (o,n) => { o.Summary = n.GetScalarValue(); } },
            { "description", (o,n) => { o.Description = n.GetScalarValue(); } },
            { "get", (o, n) => o.AddOperation(OperationType.Get, LoadOperation(n)) },
            { "put", (o, n) => o.AddOperation(OperationType.Put, LoadOperation(n)) },
            { "post", (o, n) => o.AddOperation(OperationType.Post, LoadOperation(n)) },
            { "delete", (o, n) => o.AddOperation(OperationType.Delete, LoadOperation(n)) },
            { "options", (o, n) => o.AddOperation(OperationType.Options, LoadOperation(n)) },
            { "head", (o, n) => o.AddOperation(OperationType.Head, LoadOperation(n)) },
            { "patch", (o, n) => o.AddOperation(OperationType.Patch, LoadOperation(n)) },
            { "trace", (o, n) => o.AddOperation(OperationType.Trace, LoadOperation(n)) },
            { "servers", (o,n) => o.Servers = n.CreateList(LoadServer) },
            { "parameters", (o,n) => o.Parameters = n.CreateList(LoadParameter) }
        };

        private static PatternFieldMap<OpenApiPathItem> PathItemPatternFields = new PatternFieldMap<OpenApiPathItem>
        {
            { (s)=> s.StartsWith("x-"), (o, p, n)=> o.AddExtension(p, n.CreateAny()) }
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
                OpenApiReferenceService.LoadTagByReference(valueNode.Context, valueNode.Diagnostic, valueNode.GetScalarValue()))},
            { "summary", (o,n) => { o.Summary = n.GetScalarValue(); } },
            { "description", (o,n) => { o.Description = n.GetScalarValue(); } },
            { "externalDocs", (o,n) => { o.ExternalDocs = LoadExternalDocs(n); } },
            { "operationId", (o,n) => { o.OperationId = n.GetScalarValue(); } },
            { "parameters", (o,n) => { o.Parameters = n.CreateList(LoadParameter); } },
            { "requestBody", (o,n) => { o.RequestBody = LoadRequestBody(n)    ; } },
            { "responses", (o,n) => { o.Responses = LoadResponses(n); } },
            { "callbacks", (o,n) => { o.Callbacks = n.CreateMap(LoadCallback); } },
            { "deprecated", (o,n) => { o.Deprecated = bool.Parse(n.GetScalarValue()); } },
            { "security", (o,n) => { o.Security = n.CreateList(LoadSecurityRequirement); } },
            { "servers", (o,n) => { o.Servers = n.CreateList(LoadServer); }},
        };

        private static PatternFieldMap<OpenApiOperation> OperationPatternFields = new PatternFieldMap<OpenApiOperation>
        {
            { (s)=> s.StartsWith("x-"), (o, p, n)=> o.AddExtension(p, n.CreateAny()) },
        };

        internal static OpenApiOperation LoadOperation(ParseNode node)
        {
            var mapNode = node.CheckMapNode("Operation");

            OpenApiOperation operation = new OpenApiOperation();

            ParseMap(mapNode, operation, OperationFixedFields, OperationPatternFields);

            return operation;
        }

        #endregion

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
            { "in",             (o,n) => { o.In = (ParameterLocation)Enum.Parse(typeof(ParameterLocation), n.GetScalarValue()); } },
            { "description",    (o,n) => { o.Description = n.GetScalarValue(); } },
            { "required",       (o,n) => { o.Required = bool.Parse(n.GetScalarValue()); } },
            { "deprecated",     (o,n) => { o.Deprecated = bool.Parse(n.GetScalarValue()); } },
            { "allowEmptyValue", (o,n) => { o.AllowEmptyValue = bool.Parse(n.GetScalarValue()); } },
            { "allowReserved",  (o,n) => { o.AllowReserved = bool.Parse(n.GetScalarValue()); } },
            { "style",          (o,n) => { o.Style = n.GetScalarValue(); } },
            { "schema",         (o,n) => { o.Schema = LoadSchema(n); } },
            { "content",         (o,n) => { o.Content = n.CreateMap(LoadMediaType); } },
            { "examples",       (o,n) => { o.Examples = ((ListNode)n).Select(s=> LoadExample(s)).ToList(); } },
            { "example",        (o,n) => { o.Example = n.GetScalarValue(); } },
        };

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
            var required = new List<string>() { "name", "in" };

            ParseMap(mapNode, parameter, ParameterFixedFields, ParameterPatternFields,required);

            return parameter;
        }
        #endregion

        #region OpenApiRequestBody

        private static FixedFieldMap<OpenApiRequestBody> RequestBodyFixedFields = new FixedFieldMap<OpenApiRequestBody>
        {
            { "description", (o,n) => { o.Description = n.GetScalarValue(); } },
            { "content", (o,n) => { o.Content = n.CreateMap(LoadMediaType);  } },
            { "required", (o,n) => { o.Required = bool.Parse(n.GetScalarValue()); } },
        };

        private static PatternFieldMap<OpenApiRequestBody> RequestBodyPatternFields = new PatternFieldMap<OpenApiRequestBody>
        {
            { (s)=> s.StartsWith("x-"), (o,k,n)=> o.Extensions.Add(k, new OpenApiString(n.GetScalarValue())) },
        };

        public static OpenApiRequestBody LoadRequestBody(ParseNode node)
        {
            var mapNode = node.CheckMapNode("requestBody");

            var requestBody = new OpenApiRequestBody();
            foreach (var property in mapNode)
            {
                property.ParseField(requestBody, RequestBodyFixedFields, RequestBodyPatternFields);
            }

            return requestBody;
        }

        #endregion

       
        #region MediaTypeObject
        private static FixedFieldMap<OpenApiMediaType> MediaTypeFixedFields = new FixedFieldMap<OpenApiMediaType>
        {
            { "schema", (o,n) => { o.Schema = LoadSchema(n); } },
            { "examples", (o,n) => { o.Examples = n.CreateMap(LoadExample); } },
            { "example", (o,n) => { o.Example = new OpenApiString(n.GetScalarValue()); } },
            //Encoding
        };

        private static PatternFieldMap<OpenApiMediaType> MediaTypePatternFields = new PatternFieldMap<OpenApiMediaType>
        {
            { (s)=> s.StartsWith("x-"), (o,k,n)=> o.Extensions.Add(k, new OpenApiString(n.GetScalarValue())) }
        };

        public static OpenApiMediaType LoadMediaType(ParseNode node)
        {
            var mapNode = node.CheckMapNode("contentType");

            if (mapNode.Count() == 0) return null;

            var contentType = new OpenApiMediaType();

            ParseMap(mapNode, contentType, MediaTypeFixedFields, MediaTypePatternFields);

            return contentType;
        }

        #endregion

        #region EncodingObject

        #endregion


        #region ResponsesObject
        // Validate status codes
        #endregion

        #region ResponseObject

        private static FixedFieldMap<OpenApiResponse> ResponseFixedFields = new FixedFieldMap<OpenApiResponse>
        {
            { "description", (o,n) => { o.Description = n.GetScalarValue(); } },
            { "headers", (o,n) => { o.Headers = n.CreateMap(LoadHeader); } },
            { "content", (o,n) => { o.Content = n.CreateMap(LoadMediaType); } },
            { "links", (o,n) => { o.Links = n.CreateMap(LoadLink); } }
        };

        private static PatternFieldMap<OpenApiResponse> ResponsePatternFields = new PatternFieldMap<OpenApiResponse>
        {
            { (s)=> s.StartsWith("x-"), (o,k,n)=> o.Extensions.Add(k, new OpenApiString(n.GetScalarValue())) },
        };

        public static OpenApiResponse LoadResponse(ParseNode node)
        {
            var mapNode = node.CheckMapNode("response");

            var required = new List<string>() { "description" };
            var response = new OpenApiResponse();
            ParseMap(mapNode, response, ResponseFixedFields, ResponsePatternFields, required);

            return response;
        }

        #endregion

        #region CallbackObject
        private static FixedFieldMap<OpenApiCallback> CallbackFixedFields = new FixedFieldMap<OpenApiCallback>
        {
        };

        private static PatternFieldMap<OpenApiCallback> CallbackPatternFields = new PatternFieldMap<OpenApiCallback>
        {
             { (s)=> s.StartsWith("$"), (o, p, n)=> o.AddPathItem(RuntimeExpression.Build(p), LoadPathItem(n) ) },
             { (s)=> s.StartsWith("x-"), (o, p, n)=> o.AddExtension(p, n.CreateAny() ) },
        };

        public static OpenApiCallback LoadCallback(ParseNode node)
        {
            var mapNode = node.CheckMapNode("callback");

            var refpointer = mapNode.GetReferencePointer();
            if (refpointer != null)
            {
                return mapNode.GetReferencedObject<OpenApiCallback>(refpointer);
            }

            var domainObject = new OpenApiCallback();

            ParseMap(mapNode, domainObject, CallbackFixedFields, CallbackPatternFields);

            return domainObject;
        }

        #endregion

        #region LinkObject

        private static FixedFieldMap<OpenApiLink> LinkFixedFields = new FixedFieldMap<OpenApiLink>
        {
            { "operationRef", (o,n) => { o.OperationRef = n.GetScalarValue(); } },
            { "operationId", (o,n) => { o.OperationId = n.GetScalarValue(); } },
            { "parameters", (o,n) => { o.Parameters = n.CreateSimpleMap(LoadRuntimeExpressionAnyWrapper); } },
            { "requestBody", (o,n) => { o.RequestBody = LoadRuntimeExpressionAnyWrapper(n); } },
            { "description", (o,n) => { o.Description = n.GetScalarValue(); } },
            { "server", (o, n) => o.Server = LoadServer(n) }
        };

        private static PatternFieldMap<OpenApiLink> LinkPatternFields = new PatternFieldMap<OpenApiLink>
        {
            { (s)=> s.StartsWith("x-"), (o, p, n)=> o.AddExtension(p, n.CreateAny()) },
        };

        public static OpenApiLink LoadLink(ParseNode node)
        {
            var mapNode = node.CheckMapNode("link");
            var link = new OpenApiLink();

            var refpointer = mapNode.GetReferencePointer();
            if (refpointer != null)
            {
                return mapNode.GetReferencedObject<OpenApiLink>(refpointer);
            }

            ParseMap(mapNode, link, LinkFixedFields, LinkPatternFields);

            return link;
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
            { "schema", (o,n) => { o.Schema = LoadSchema(n); } }
        };

        private static PatternFieldMap<OpenApiHeader> HeaderPatternFields = new PatternFieldMap<OpenApiHeader>
        {
            { (s)=> s.StartsWith("x-"), (o,k,n)=> o.Extensions.Add(k, new OpenApiString(n.GetScalarValue())) },
        };


        public static OpenApiHeader LoadHeader(ParseNode node)
        {
            var mapNode = node.CheckMapNode("header");
            var header = new OpenApiHeader();
            foreach (var property in mapNode)
            {
                property.ParseField(header, HeaderFixedFields, HeaderPatternFields);
            }

            return header;
        }

        #endregion

        #region ExampleObject
        private static FixedFieldMap<OpenApiExample> ExampleFixedFields = new FixedFieldMap<OpenApiExample>
        {
            { "summary", (o,n) => { o.Summary= n.GetScalarValue(); } },
            { "description", (o,n) => { o.Description = n.GetScalarValue(); } },
            { "value", (o,n) => { o.Value = n.GetScalarValue(); } },
        };

        private static PatternFieldMap<OpenApiExample> ExamplePatternFields = new PatternFieldMap<OpenApiExample>
        {
            { (s)=> s.StartsWith("x-"), (o,k,n)=> o.Extensions.Add(k, new OpenApiString(n.GetScalarValue())) }
        };

        public static OpenApiExample LoadExample(ParseNode node)
        {
            var mapNode = node.CheckMapNode("Example");

            var refpointer = mapNode.GetReferencePointer();
            if (refpointer != null)
            {
                return mapNode.GetReferencedObject<OpenApiExample>(refpointer);
            }


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
                { "enum", (o, n) =>
                    {
                        o.Enum = n.CreateSimpleList<IOpenApiAny>((s) => new OpenApiString(s.GetScalarValue()));
                    }
                },
                { "type", (o,n) => { o.Type = n.GetScalarValue(); } },
                { "allOf", (o,n) => { o.AllOf = n.CreateList(LoadSchema); } },
                { "oneOf", (o,n) => { o.OneOf = n.CreateList(LoadSchema); } },
                { "anyOf", (o,n) => { o.AnyOf = n.CreateList(LoadSchema); } },
                { "not", (o,n) => { o.Not= LoadSchema(n); } },
                { "items", (o,n) => { o.Items = LoadSchema(n); } },
                { "properties", (o,n) => { o.Properties = n.CreateMap(LoadSchema); } },
                { "additionalProperties", (o,n) => { o.AdditionalProperties = LoadSchema(n); }
                                                     },
                { "description", (o,n) => { o.Description = n.GetScalarValue(); } },
                { "format", (o,n) => { o.Format = n.GetScalarValue(); } },
                { "default", (o,n) => { o.Default =  new OpenApiString( n.GetScalarValue() ); } },

                { "nullable", (o,n) => { o.Nullable = bool.Parse(n.GetScalarValue()); } },
                // discriminator
                { "readOnly", (o,n) => { o.ReadOnly = bool.Parse(n.GetScalarValue()); } },
                { "writeOnly", (o,n) => { o.WriteOnly = bool.Parse(n.GetScalarValue()); } },
                // xml
                { "externalDocs", (o,n) => { o.ExternalDocs = LoadExternalDocs(n); } },
                { "example", (o,n) => { o.Example =  new OpenApiString(n.GetScalarValue()); } },
                { "deprecated", (o,n) => { o.Deprecated = bool.Parse(n.GetScalarValue()); } },

        };

        private static PatternFieldMap<OpenApiSchema> SchemaPatternFields = new PatternFieldMap<OpenApiSchema>
        {
            { (s)=> s.StartsWith("x-"), (o,k,n)=> o.Extensions.Add(k, new OpenApiString(n.GetScalarValue())) }
        };

        public static OpenApiSchema LoadSchema(ParseNode node)
        {

            var mapNode = node.CheckMapNode("schema");

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
            { "type", (o,n) =>
                {
                    o.Type = n.GetScalarValue().GetEnumFromDisplayName<SecuritySchemeType>();
                } 
            },
            { "description", (o,n) => { o.Description = n.GetScalarValue();  } },
            { "name", (o,n) => { o.Name = n.GetScalarValue();  } },
            { "in", (o,n) => { o.In = (ParameterLocation)Enum.Parse(typeof(ParameterLocation), n.GetScalarValue()); } },
            { "scheme", (o,n) => { o.Scheme = n.GetScalarValue();  } },
            { "bearerFormat", (o,n) => { o.BearerFormat = n.GetScalarValue();  } },
            { "openIdConnectUrl", (o,n) => { o.OpenIdConnectUrl = new Uri(n.GetScalarValue(), UriKind.RelativeOrAbsolute);  } },
            { "flows", (o,n) => { o.Flows = LoadOAuthFlows(n); } }
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

                obj.Schemes.Add(scheme, property.Value.CreateSimpleList<string>(n2 => n2.GetScalarValue()));
            }
            return obj;
        }

        #endregion

        public static IOpenApiReference LoadReference(OpenApiReference pointer, object rootNode)
        {
            IOpenApiReference referencedObject = null;

            var node = ((RootNode)rootNode).Find(pointer.GetLocalPointer());
            if (node == null && pointer.ReferenceType != ReferenceType.Tags) return null;

            switch (pointer.ReferenceType)
            {
                case ReferenceType.Schema:
                    referencedObject = LoadSchema(node);
                    break;

                case ReferenceType.Parameter:
                    referencedObject = LoadParameter(node);
                    break;

                case ReferenceType.Callback:
                    referencedObject = LoadCallback(node);
                    break;

                case ReferenceType.SecurityScheme:
                    referencedObject = LoadSecurityScheme(node);
                    break;

                case ReferenceType.Link:
                    referencedObject = LoadLink(node);
                    break;

                case ReferenceType.Example:
                    referencedObject = LoadExample(node);
                    break;

                case ReferenceType.Tags:
                    ListNode list = (ListNode)node;
                    if (list != null)
                    {
                        foreach (var item in list)
                        {
                            var tag = OpenApiV3Deserializer.LoadTag(item);

                            if (tag.Name == pointer.TypeName)
                            {
                                return tag;
                            }
                        }
                    }
                    else
                    {
                        return new OpenApiTag() { Name = pointer.TypeName };
                    }

                    break;

                default:
                    throw new OpenApiException($"Unknown type of $ref {pointer.ReferenceType} at {pointer.ToString()}");
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

            ReportMissing(mapNode, requiredFields);
        }

        private static RuntimeExpression LoadRuntimeExpression(ParseNode node)
        {
            var value = node.GetScalarValue();
            return RuntimeExpression.Build(value);
        }
        private static RuntimeExpressionAnyWrapper LoadRuntimeExpressionAnyWrapper(ParseNode node)
        {
            var value = node.GetScalarValue();

            if (value != null && value.StartsWith("$"))
            {
                return new RuntimeExpressionAnyWrapper
                {
                    Expression = RuntimeExpression.Build(value)
                };
            }
            else
            {
                return new RuntimeExpressionAnyWrapper
                {
                    Any = node.CreateAny()
                };
            }
        }

        private static void ReportMissing(ParseNode node, IList<string> required)
        {
            if (required == null || !required.Any() )
            {
                return;
            }

            foreach (var error in required.Select(r => new OpenApiError("", $"{r} is a required property of {node.Context.GetLocation()}")).ToList())
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
