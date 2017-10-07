
namespace Microsoft.OpenApi.Readers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class OpenApiV3Reader
    {

        #region OpenApiObject
        public static FixedFieldMap<OpenApiDocument> OpenApiFixedFields = new FixedFieldMap<OpenApiDocument> {
            { "openapi", (o,n) => { o.Version = n.GetScalarValue(); } },
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
            { (s)=> s.StartsWith("x-"), (o,k,n)=> o.Extensions.Add(k, new GenericOpenApiExtension(n.GetScalarValue())) }
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

        public static FixedFieldMap<Info> InfoFixedFields = new FixedFieldMap<Info>
        {
            { "title",      (o,n) => { o.Title = n.GetScalarValue(); } },
            { "version",    (o,n) => { o.Version = n.GetScalarValue(); } },
            { "description", (o,n) => { o.Description = n.GetScalarValue(); } },
            { "termsOfService", (o,n) => { o.TermsOfService = n.GetScalarValue(); } },
            { "contact",    (o,n) => { o.Contact = LoadContact(n); } },
            { "license",    (o,n) => { o.License = LoadLicense(n); } }
        };

        public static PatternFieldMap<Info> InfoPatternFields = new PatternFieldMap<Info>
        {
            { (s)=> s.StartsWith("x-"), (o,k,n)=> o.Extensions.Add(k, new GenericOpenApiExtension(n.GetScalarValue())) }
        };


        public static Info LoadInfo(ParseNode node)
        {
            var mapNode = node.CheckMapNode("Info");

            var info = new Info();
            var required = new List<string>() { "title", "version" };

            ParseMap(mapNode, info, InfoFixedFields, InfoPatternFields, required);

            return info;
        }


        #endregion

        #region ContactObject

        public static FixedFieldMap<Contact> ContactFixedFields = new FixedFieldMap<Contact> {
            { "name", (o,n) => { o.Name = n.GetScalarValue(); } },
            { "email", (o,n) => { o.Email = n.GetScalarValue(); } },
            { "url", (o,n) => { o.Url = new Uri(n.GetScalarValue()); } },
        };

        public static PatternFieldMap<Contact> ContactPatternFields = new PatternFieldMap<Contact>
        {
            { (s)=> s.StartsWith("x-"), (o,k,n)=> o.Extensions.Add(k, new GenericOpenApiExtension(n.GetScalarValue())) }
        };

        public static Contact LoadContact(ParseNode node)
        {
            var mapNode = node as MapNode;
            var contact = new Contact();

            ParseMap(mapNode, contact, ContactFixedFields,ContactPatternFields);

            return contact;
        }

        #endregion

        #region LicenseObject

        public static FixedFieldMap<License> LicenseFixedFields = new FixedFieldMap<License> {
            { "name", (o,n) => { o.Name = n.GetScalarValue(); } },
            { "url", (o,n) => { o.Url = new Uri(n.GetScalarValue()); } },
        };

        public static PatternFieldMap<License> LicensePatternFields = new PatternFieldMap<License>
        {
            { (s)=> s.StartsWith("x-"), (o,k,n)=> o.Extensions.Add(k, new GenericOpenApiExtension(n.GetScalarValue())) }
        };

        internal static License LoadLicense(ParseNode node)
        {
            var mapNode = node.CheckMapNode("License");

            var license = new License();

            ParseMap(mapNode, license, LicenseFixedFields, LicensePatternFields);

            return license;
        }

        #endregion

        #region ServerObject

        private static FixedFieldMap<Server> ServerFixedFields = new FixedFieldMap<Server>
        {
            { "url", (o,n) => { o.Url=  n.GetScalarValue(); } },
            { "description", (o,n) => { o.Description = n.GetScalarValue();  } },
            { "variables", (o,n) => {  o.Variables = n.CreateMap(LoadServerVariable); } }
        };

        private static PatternFieldMap<Server> ServerPatternFields = new PatternFieldMap<Server>
        {
            { (s)=> s.StartsWith("x-"), (o,k,n)=> o.Extensions.Add(k, n.GetScalarValue()) }
        };

        public static Server LoadServer(ParseNode node)
        {
            var mapNode = node.CheckMapNode("server");

            var server = new Server();

            ParseMap(mapNode, server, ServerFixedFields, ServerPatternFields);

            return server;
        }

        #endregion

        #region ServerVariable

        private static FixedFieldMap<ServerVariable> ServerVariableFixedFields = new FixedFieldMap<ServerVariable>
        {
            { "enum", (o,n) => { o.Enum =  n.CreateSimpleList<string>((s)=> s.GetScalarValue()); } },
            { "default", (o,n) => { o.Default =  n.GetScalarValue(); } },
            { "description", (o,n) => { o.Description = n.GetScalarValue();  } },
        };

        private static PatternFieldMap<ServerVariable> ServerVariablePatternFields = new PatternFieldMap<ServerVariable>
        {
            { (s)=> s.StartsWith("x-"), (o,k,n)=> o.Extensions.Add(k, n.GetScalarValue()) }
        };

        public static ServerVariable LoadServerVariable(ParseNode node)
        {
            var mapNode = node.CheckMapNode("serverVariable");

            var serverVariable = new ServerVariable();

            ParseMap(mapNode, serverVariable, ServerVariableFixedFields, ServerVariablePatternFields);

            return serverVariable;
        }

        #endregion

        #region ComponentsObject

        public static FixedFieldMap<Components> ComponentsFixedFields = new FixedFieldMap<Components> {
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

        public static PatternFieldMap<Components> ComponentsPatternFields = new PatternFieldMap<Components>
        {
            { (s)=> s.StartsWith("x-"), (o,k,n)=> o.Extensions.Add(k, new GenericOpenApiExtension(n.GetScalarValue())) }
        };

        public static Components LoadComponents(ParseNode node)
        {
            var mapNode = node.CheckMapNode("components");
            var components = new Components();

            ParseMap(mapNode, components, ComponentsFixedFields, ComponentsPatternFields);

            return components;
        }

        #endregion

        #region PathsObject

        public static FixedFieldMap<Paths> PathsFixedFields = new FixedFieldMap<Paths>
        {
        };

        public static PatternFieldMap<Paths> PathsPatternFields = new PatternFieldMap<Paths>
        {
            { (s)=> s.StartsWith("/"), (o,k,n)=> o.PathItems.Add(k, LoadPathItem(n)    ) },
            { (s)=> s.StartsWith("x-"), (o,k,n)=> o.Extensions.Add(k, new GenericOpenApiExtension(n.GetScalarValue())) }
        };

        public static Paths LoadPaths(ParseNode node)
        {
            var mapNode = node.CheckMapNode("Paths");

            Paths domainObject = new Paths();

            ParseMap(mapNode, domainObject, PathsFixedFields, PathsPatternFields);

            return domainObject;
        }
        #endregion

        #region PathItemObject

        private static FixedFieldMap<PathItem> PathItemFixedFields = new FixedFieldMap<PathItem>
        {
            // $ref
            { "summary", (o,n) => { o.Summary = n.GetScalarValue(); } },
            { "description", (o,n) => { o.Description = n.GetScalarValue(); } },
            { "servers", (o,n) => { o.Servers = n.CreateList(LoadServer); } },
            { "parameters", (o,n) => { o.Parameters = n.CreateList(LoadParameter); } },

        };

        private static PatternFieldMap<PathItem> PathItemPatternFields = new PatternFieldMap<PathItem>
        {
            { (s)=> s.StartsWith("x-"), (o,k,n)=> o.Extensions.Add(k, new GenericOpenApiExtension(n.GetScalarValue())) },
            { (s)=> "get,put,post,delete,patch,options,head,patch,trace".Contains(s),
                (o,k,n)=> o.AddOperation(OperationTypeExtensions.ParseOperationType(k), LoadOperation(n)    ) }
        };


        public static PathItem LoadPathItem(ParseNode node)
        {
            var mapNode = node.CheckMapNode("PathItem");

            var pathItem = new PathItem();

            ParseMap(mapNode, pathItem, PathItemFixedFields, PathItemPatternFields);

            return pathItem;
        }

        #endregion

        #region OperationObject

        private static FixedFieldMap<Operation> OperationFixedFields = new FixedFieldMap<Operation>
        {
            { "tags", (o,n) => o.Tags = n.CreateSimpleList((v) => ReferenceService.LoadTagByReference(v.Context,v.GetScalarValue()))},
            { "summary", (o,n) => { o.Summary = n.GetScalarValue(); } },
            { "description", (o,n) => { o.Description = n.GetScalarValue(); } },
            { "externalDocs", (o,n) => { o.ExternalDocs = LoadExternalDocs(n); } },
            { "operationId", (o,n) => { o.OperationId = n.GetScalarValue(); } },
            { "parameters", (o,n) => { o.Parameters = n.CreateList(LoadParameter); } },
            { "requestBody", (o,n) => { o.RequestBody = LoadRequestBody(n)    ; } },
            { "responses", (o,n) => { o.Responses = n.CreateMap(LoadResponse); } },
            { "callbacks", (o,n) => { o.Callbacks = n.CreateMap(LoadCallback); } },
            { "deprecated", (o,n) => { o.Deprecated = bool.Parse(n.GetScalarValue()); } },
            { "security", (o,n) => { o.Security = n.CreateList(LoadSecurityRequirement); } },
            { "servers", (o,n) => { o.Servers = n.CreateList(LoadServer); }},
        };

        private static PatternFieldMap<Operation> OperationPatternFields = new PatternFieldMap<Operation>
        {
            { (s)=> s.StartsWith("x-"), (o,k,n)=> o.Extensions.Add(k, new GenericOpenApiExtension(n.GetScalarValue())) },
        };

        internal static Operation LoadOperation(ParseNode node)
        {
            var mapNode = node.CheckMapNode("Operation");

            Operation operation = new Operation();

            ParseMap(mapNode, operation, OperationFixedFields, OperationPatternFields);

            return operation;
        }

        #endregion

        #region ExternalDocsObject

        private static FixedFieldMap<ExternalDocs> ExternalDocsFixedFields = new FixedFieldMap<ExternalDocs>
        {
            // $ref
            { "description", (o,n) => { o.Description = n.GetScalarValue(); } },
            { "url", (o,n) => { o.Url = new Uri(n.GetScalarValue()); } },
        };

        private static PatternFieldMap<ExternalDocs> ExternalDocsPatternFields = new PatternFieldMap<ExternalDocs>
        {
        };


        public static ExternalDocs LoadExternalDocs(ParseNode node)
        {
            var mapNode = node.CheckMapNode("externalDocs");

            var externalDocs = new ExternalDocs();

            ParseMap(mapNode, externalDocs, ExternalDocsFixedFields, ExternalDocsPatternFields);

            return externalDocs;
        }

        #endregion

        #region ParameterObject

        private static FixedFieldMap<Parameter> ParameterFixedFields = new FixedFieldMap<Parameter>
        {
            { "name",           (o,n) => { o.Name = n.GetScalarValue(); } },
            { "in",             (o,n) => { o.In = (InEnum)Enum.Parse(typeof(InEnum), n.GetScalarValue()); } },
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

        private static PatternFieldMap<Parameter> ParameterPatternFields = new PatternFieldMap<Parameter>
        {
            { (s)=> s.StartsWith("x-"), (o,k,n)=> o.Extensions.Add(k, new GenericOpenApiExtension(n.GetScalarValue())) },
        };


        public static Parameter LoadParameter(ParseNode node)
        {
            var mapNode = node.CheckMapNode("parameter");

            var refpointer = mapNode.GetReferencePointer();
            if (refpointer != null)
            {
                return mapNode.GetReferencedObject<Parameter>(refpointer);
            }

            var parameter = new Parameter();
            var required = new List<string>() { "name", "in" };

            ParseMap(mapNode, parameter, ParameterFixedFields, ParameterPatternFields,required);

            return parameter;
        }
        #endregion

        #region RequestBody

        private static FixedFieldMap<RequestBody> RequestBodyFixedFields = new FixedFieldMap<RequestBody>
        {
            { "description", (o,n) => { o.Description = n.GetScalarValue(); } },
            { "content", (o,n) => { o.Content = n.CreateMap(LoadMediaType);  } },
            { "required", (o,n) => { o.Required = bool.Parse(n.GetScalarValue()); } },
        };

        private static PatternFieldMap<RequestBody> RequestBodyPatternFields = new PatternFieldMap<RequestBody>
        {
            { (s)=> s.StartsWith("x-"), (o,k,n)=> o.Extensions.Add(k, n.GetScalarValue()) },
        };

        public static RequestBody LoadRequestBody(ParseNode node)
        {
            var mapNode = node.CheckMapNode("requestBody");

            var requestBody = new RequestBody();
            foreach (var property in mapNode)
            {
                property.ParseField(requestBody, RequestBodyFixedFields, RequestBodyPatternFields);
            }

            return requestBody;
        }

        #endregion

       
        #region MediaTypeObject
        private static FixedFieldMap<MediaType> MediaTypeFixedFields = new FixedFieldMap<MediaType>
        {
            { "schema", (o,n) => { o.Schema = LoadSchema(n); } },
            { "examples", (o,n) => { o.Examples = n.CreateMap(LoadExample); } },
            { "example", (o,n) => { o.Example = n.GetScalarValue(); } },
            //Encoding
        };

        private static PatternFieldMap<MediaType> MediaTypePatternFields = new PatternFieldMap<MediaType>
        {
            { (s)=> s.StartsWith("x-"), (o,k,n)=> o.Extensions.Add(k, n.GetScalarValue()) }
        };

        public static MediaType LoadMediaType(ParseNode node)
        {
            var mapNode = node.CheckMapNode("contentType");

            if (mapNode.Count() == 0) return null;

            var contentType = new MediaType();

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

        private static FixedFieldMap<Response> ResponseFixedFields = new FixedFieldMap<Response>
        {
            { "description", (o,n) => { o.Description = n.GetScalarValue(); } },
            { "headers", (o,n) => { o.Headers = n.CreateMap(LoadHeader); } },
            { "content", (o,n) => { o.Content = n.CreateMap(LoadMediaType); } },
            { "links", (o,n) => { o.Links = n.CreateMap(LoadLink); } }
        };

        private static PatternFieldMap<Response> ResponsePatternFields = new PatternFieldMap<Response>
        {
            { (s)=> s.StartsWith("x-"), (o,k,n)=> o.Extensions.Add(k, new GenericOpenApiExtension(n.GetScalarValue())) },
        };

        public static Response LoadResponse(ParseNode node)
        {
            var mapNode = node.CheckMapNode("response");

            var required = new List<string>() { "description" };
            var response = new Response();
            ParseMap(mapNode, response, ResponseFixedFields, ResponsePatternFields, required);

            return response;
        }

        #endregion

        #region CallbackObject
        private static FixedFieldMap<Callback> CallbackFixedFields = new FixedFieldMap<Callback>
        {
        };

        private static PatternFieldMap<Callback> CallbackPatternFields = new PatternFieldMap<Callback>
        {
             { (s)=> s.StartsWith("$"),
                (o,k,n)=> o.PathItems.Add(new RuntimeExpression(k), LoadPathItem(n)    ) }
        };

        public static Callback LoadCallback(ParseNode node)
        {
            var mapNode = node.CheckMapNode("callback");

            var refpointer = mapNode.GetReferencePointer();
            if (refpointer != null)
            {
                return mapNode.GetReferencedObject<Callback>(refpointer);
            }

            var domainObject = new Callback();

            ParseMap(mapNode, domainObject, CallbackFixedFields, CallbackPatternFields);

            return domainObject;
        }

        #endregion

        #region LinkObject

        private static FixedFieldMap<Link> LinkFixedFields = new FixedFieldMap<Link>
        {
            { "href", (o,n) => { o.Href = n.GetScalarValue(); } },
            { "operationId", (o,n) => { o.OperationId = n.GetScalarValue(); } },
            { "parameters", (o,n) => { o.Parameters = n.CreateSimpleMap(LoadRuntimeExpression); } },
            { "requestBody", (o,n) => { o.RequestBody = LoadRuntimeExpression(n); } },
            { "description", (o,n) => { o.Description = n.GetScalarValue(); } },
        };

        private static PatternFieldMap<Link> LinkPatternFields = new PatternFieldMap<Link>
        {
            { (s)=> s.StartsWith("x-"), (o,k,n)=> o.Extensions.Add(k, n.GetScalarValue()) },
        };


        public static Link LoadLink(ParseNode node)
        {
            var mapNode = node.CheckMapNode("link");
            var link = new Link();

            var refpointer = mapNode.GetReferencePointer();
            if (refpointer != null)
            {
                return mapNode.GetReferencedObject<Link>(refpointer);
            }

            ParseMap(mapNode, link, LinkFixedFields, LinkPatternFields);

            return link;
        }

        #endregion

        #region HeaderObject
        private static FixedFieldMap<Header> HeaderFixedFields = new FixedFieldMap<Header>
        {
            { "description", (o,n) => { o.Description = n.GetScalarValue(); } },
            { "required", (o,n) => { o.Required = bool.Parse(n.GetScalarValue()); } },
            { "deprecated", (o,n) => { o.Deprecated = bool.Parse(n.GetScalarValue()); } },
            { "allowReserved", (o,n) => { o.AllowReserved = bool.Parse(n.GetScalarValue()); } },
            { "style", (o,n) => { o.Style = n.GetScalarValue(); } },
            { "schema", (o,n) => { o.Schema = LoadSchema(n); } }
        };

        private static PatternFieldMap<Header> HeaderPatternFields = new PatternFieldMap<Header>
        {
            { (s)=> s.StartsWith("x-"), (o,k,n)=> o.Extensions.Add(k, new GenericOpenApiExtension(n.GetScalarValue())) },
        };


        public static Header LoadHeader(ParseNode node)
        {
            var mapNode = node.CheckMapNode("header");
            var header = new Header();
            foreach (var property in mapNode)
            {
                property.ParseField(header, HeaderFixedFields, HeaderPatternFields);
            }

            return header;
        }

        #endregion

        #region ExampleObject
        private static FixedFieldMap<Example> ExampleFixedFields = new FixedFieldMap<Example>
        {
            { "summary", (o,n) => { o.Summary= n.GetScalarValue(); } },
            { "description", (o,n) => { o.Description = n.GetScalarValue(); } },
            { "value", (o,n) => { o.Value = n.GetScalarValue(); } },
        };

        private static PatternFieldMap<Example> ExamplePatternFields = new PatternFieldMap<Example>
        {
            { (s)=> s.StartsWith("x-"), (o,k,n)=> o.Extensions.Add(k, new GenericOpenApiExtension(n.GetScalarValue())) }
        };

        public static Example LoadExample(ParseNode node)
        {
            var mapNode = node.CheckMapNode("Example");

            var refpointer = mapNode.GetReferencePointer();
            if (refpointer != null)
            {
                return mapNode.GetReferencedObject<Example>(refpointer);
            }


            var example = new Example();
            foreach (var property in mapNode)
            {
                property.ParseField(example, ExampleFixedFields, ExamplePatternFields);
            }

            return example;
        }


        #endregion

        #region TagObject
        internal static Tag LoadTag(ParseNode n)
        {
            var mapNode = n.CheckMapNode("tag");

            var obj = new Tag();

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

        private static FixedFieldMap<Schema> SchemaFixedFields = new FixedFieldMap<Schema>
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
                { "enum", (o,n) => { o.Enum =  n.CreateSimpleList<string>((s)=> s.GetScalarValue()); } },

                { "type", (o,n) => { o.Type = n.GetScalarValue(); } },
                { "allOf", (o,n) => { o.AllOf = n.CreateList(LoadSchema); } },
                { "oneOf", (o,n) => { o.OneOf = n.CreateList(LoadSchema); } },
                { "anyOf", (o,n) => { o.AnyOf = n.CreateList(LoadSchema); } },
                { "not", (o,n) => { o.Not= LoadSchema(n); } },
                { "items", (o,n) => { o.Items = LoadSchema(n); } },
                { "properties", (o,n) => { o.Properties = n.CreateMap(LoadSchema); } },
                { "additionalProperties", (o,n) => { if (n is ValueNode) { o.AdditionalPropertiesAllowed = bool.Parse(n.GetScalarValue()); }
                                                     else { o.AdditionalProperties = LoadSchema(n); }
                                                    } },
                { "description", (o,n) => { o.Description = n.GetScalarValue(); } },
                { "format", (o,n) => { o.Format = n.GetScalarValue(); } },
                { "default", (o,n) => { o.Default = n.GetScalarValue(); } },

                { "nullable", (o,n) => { o.Nullable = bool.Parse(n.GetScalarValue()); } },
                // discriminator
                { "readOnly", (o,n) => { o.ReadOnly = bool.Parse(n.GetScalarValue()); } },
                { "writeOnly", (o,n) => { o.WriteOnly = bool.Parse(n.GetScalarValue()); } },
                // xml
                { "externalDocs", (o,n) => { o.ExternalDocs = LoadExternalDocs(n); } },
                { "example", (o,n) => { o.Example = n.GetScalarValue(); } },
                { "deprecated", (o,n) => { o.Deprecated = bool.Parse(n.GetScalarValue()); } },

        };

        private static PatternFieldMap<Schema> SchemaPatternFields = new PatternFieldMap<Schema>
        {
            { (s)=> s.StartsWith("x-"), (o,k,n)=> o.Extensions.Add(k, new GenericOpenApiExtension(n.GetScalarValue())) }
        };

        public static Schema LoadSchema(string schema)
        {
            return LoadSchema(MapNode.Create(schema));
        }

        public static Schema LoadSchema(ParseNode node)
        {

            var mapNode = node.CheckMapNode("schema");

            var refpointer = mapNode.GetReferencePointer();
            if (refpointer != null)
            {
                return mapNode.GetReferencedObject<Schema>(refpointer);
            }

            var domainObject = new Schema();

            foreach (var propertyNode in mapNode)
            {
                propertyNode.ParseField(domainObject, SchemaFixedFields, SchemaPatternFields);
            }

            return domainObject;
        }


        #endregion

        #region SecuritySchemeObject

        private static FixedFieldMap<SecurityScheme> SecuritySchemeFixedFields = new FixedFieldMap<SecurityScheme>
        {
            { "type", (o,n) => { o.Type = n.GetScalarValue();  } },
            { "description", (o,n) => { o.Description = n.GetScalarValue();  } },
            { "name", (o,n) => { o.Name = n.GetScalarValue();  } },
            { "in", (o,n) => { o.In = n.GetScalarValue();  } },
            { "scheme", (o,n) => { o.Scheme = n.GetScalarValue();  } },
            { "bearerFormat", (o,n) => { o.BearerFormat = n.GetScalarValue();  } },
            { "openIdConnectUrl", (o,n) => { o.OpenIdConnectUrl = new Uri(n.GetScalarValue(), UriKind.RelativeOrAbsolute);  } },
            { "flow", (o,n) => { o.Flow = n.GetScalarValue();  } },
            { "authorizationUrl", (o,n) => { o.AuthorizationUrl = new Uri(n.GetScalarValue(), UriKind.RelativeOrAbsolute);  } },
            { "tokenUrl", (o,n) => { o.TokenUrl = new Uri(n.GetScalarValue(), UriKind.RelativeOrAbsolute);  } },
            { "scopes", (o,n) => { o.Scopes= n.CreateMap<string>(v => v.GetScalarValue()  ); } },
        };

        private static PatternFieldMap<SecurityScheme> SecuritySchemePatternFields = new PatternFieldMap<SecurityScheme>
        {
            { (s)=> s.StartsWith("x-"), (o,k,n)=> o.Extensions.Add(k, new GenericOpenApiExtension(n.GetScalarValue())) }
        };

        public static SecurityScheme LoadSecurityScheme(ParseNode node)
        {
            var mapNode = node.CheckMapNode("securityScheme");

            var securityScheme = new SecurityScheme();
            foreach (var property in mapNode)
            {
                property.ParseField(securityScheme, SecuritySchemeFixedFields, SecuritySchemePatternFields);
            }

            return securityScheme;
        }

        #endregion

        #region SecurityRequirement
        public static SecurityRequirement LoadSecurityRequirement(ParseNode node)
        {

            var mapNode = node.CheckMapNode("security");

            var obj = new SecurityRequirement();

            foreach (var property in mapNode)
            {
                var scheme = ReferenceService.LoadSecuritySchemeByReference(mapNode.Context, property.Name);

                obj.Schemes.Add(scheme, property.Value.CreateSimpleList<string>(n2 => n2.GetScalarValue()));
            }
            return obj;
        }

        #endregion

        public static IReference LoadReference(OpenApiReference pointer, object rootNode)
        {
            IReference referencedObject = null;

            var node = ((RootNode)rootNode).Find(pointer.GetLocalPointer());
            if (node == null && pointer.ReferenceType != ReferenceType.Tags) return null;

            switch (pointer.ReferenceType)
            {
                case ReferenceType.Schema:
                    referencedObject = OpenApiV3Reader.LoadSchema(node);
                    break;
                case ReferenceType.Parameter:

                    referencedObject = OpenApiV3Reader.LoadParameter(node);
                    break;
                case ReferenceType.Callback:
                    referencedObject = OpenApiV3Reader.LoadCallback(node);
                    break;
                case ReferenceType.SecurityScheme:
                    referencedObject = OpenApiV3Reader.LoadSecurityScheme(node);
                    break;
                case ReferenceType.Link:
                    referencedObject = OpenApiV3Reader.LoadLink(node);
                    break;
                case ReferenceType.Example:
                    referencedObject = OpenApiV3Reader.LoadExample(node);
                    break;
                case ReferenceType.Tags:
                    ListNode list = (ListNode)node;
                    if (list != null)
                    {
                        foreach (var item in list)
                        {
                            var tag = OpenApiV3Reader.LoadTag(item);

                            if (tag.Name == pointer.TypeName)
                            {
                                return tag;
                            }
                        }
                    }
                    else
                    {
                        return new Tag() { Name = pointer.TypeName };
                    }

                    break;
                default:
                    throw new DomainParseException($"Unknown type of $ref {pointer.ReferenceType} at {pointer.ToString()}");

            }
            return referencedObject;
        }

        private static void ParseMap<T>(MapNode mapNode, T domainObject, FixedFieldMap<T> fixedFieldMap, PatternFieldMap<T> patternFieldMap, List<string> requiredFields  = null)
        {
            if (mapNode == null) return;

            foreach (var propertyNode in mapNode)
            {
                propertyNode.ParseField<T>(domainObject, fixedFieldMap, patternFieldMap);
                if (requiredFields != null) requiredFields.Remove(propertyNode.Name);
            }
            ReportMissing(mapNode, requiredFields);
        }

        private static RuntimeExpression LoadRuntimeExpression(ParseNode node)
        {
            var value = node.GetScalarValue();
            return new RuntimeExpression(value);
        }
        private static void ReportMissing(ParseNode node, List<string> required)
        {
            if (required != null && required.Count > 0)
            {
                node.Context.ParseErrors.AddRange(required.Select(r => new OpenApiError("", $"{r} is a required property of {node.Context.GetLocation()}")));
            }
        }

    }
}
