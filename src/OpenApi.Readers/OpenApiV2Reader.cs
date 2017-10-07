
namespace Microsoft.OpenApi.Readers
{
    using Microsoft.OpenApi.Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public static class OpenApiV2Reader
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



        private static void MakeServers(List<Server> servers, ParsingContext context)
        {
            string host = context.GetTempStorage<string>("host");
            string basePath = context.GetTempStorage<string>("basePath");
            List<string> schemes = context.GetTempStorage<List<string>>("schemes");

            if (schemes != null)
            {
                foreach (var scheme in schemes)
                {
                    var server = new Server();
                    server.Url = scheme + "://" + (host ?? "example.org/") + (basePath ?? "/");
                    servers.Add(server);
                }
            }
        }

        public static PatternFieldMap<OpenApiDocument> OpenApiPatternFields = new PatternFieldMap<OpenApiDocument>
        {
            // We have no semantics to verify X- nodes, therefore treat them as just values.
            { (s)=> s.StartsWith("x-"), (o,k,n)=> o.Extensions.Add(k, new GenericOpenApiExtension(n.GetScalarValue())) }
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

        public static FixedFieldMap<Info> InfoFixedFields = new FixedFieldMap<Info>
        {
            { "title",      (o,n) => { o.Title = n.GetScalarValue(); } },
            { "description", (o,n) => { o.Description = n.GetScalarValue(); } },
            { "termsOfService", (o,n) => { o.TermsOfService = n.GetScalarValue(); } },
            { "contact",    (o,n) => { o.Contact = LoadContact(n); } },
            { "license",    (o,n) => { o.License = LoadLicense(n); } },
            { "version",    (o,n) => { o.Version = n.GetScalarValue(); } }
        };

        public static PatternFieldMap<Info> InfoPatternFields = new PatternFieldMap<Info>
        {
            { (s)=> s.StartsWith("x-"), (o,k,n)=> o.Extensions.Add(k,  new GenericOpenApiExtension(n.GetScalarValue())) }
        };


        public static Info LoadInfo(IParseNode node)
        {
            var mapNode = node.CheckMapNode("Info");

            var info = new Info();
            var required = new List<string>() { "title", "version" };

            ParseMap(mapNode, info, InfoFixedFields, InfoPatternFields, required);

            ReportMissing(node, required);

            return info;
        }


        #endregion

        #region ContactObject

        public static FixedFieldMap<Contact> ContactFixedFields = new FixedFieldMap<Contact> {
            { "name", (o,n) => { o.Name = n.GetScalarValue(); } },
            { "url", (o,n) => { o.Url = new Uri(n.GetScalarValue()); } },
            { "email", (o,n) => { o.Email = n.GetScalarValue(); } },
        };

        public static PatternFieldMap<Contact> ContactPatternFields = new PatternFieldMap<Contact>
        {
            { (s)=> s.StartsWith("x-"), (o,k,n)=> o.Extensions.Add(k,  new GenericOpenApiExtension(n.GetScalarValue())) }
        };

        public static Contact LoadContact(IParseNode node)
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
            { "url", (o,n) => { o.Url = new Uri(n.GetScalarValue(), UriKind.RelativeOrAbsolute); } },
        };

        public static PatternFieldMap<License> LicensePatternFields = new PatternFieldMap<License>
        {
            { (s)=> s.StartsWith("x-"), (o,k,n)=> o.Extensions.Add(k,  new GenericOpenApiExtension(n.GetScalarValue())) }
        };

        internal static License LoadLicense(IParseNode node)
        {
            var mapNode = node.CheckMapNode("License");

            var license = new License();

            ParseMap(mapNode, license, LicenseFixedFields, LicensePatternFields);

            return license;
        }

        #endregion

        #region PathsObject

        public static FixedFieldMap<Paths> PathsFixedFields = new FixedFieldMap<Paths>
        {
        };

        public static PatternFieldMap<Paths> PathsPatternFields = new PatternFieldMap<Paths>
        {
            { (s)=> s.StartsWith("/"), (o,k,n)=> o.PathItems.Add(k, OpenApiV2Reader.LoadPathItem(n)    ) },
            { (s)=> s.StartsWith("x-"), (o,k,n)=> o.Extensions.Add(k, new GenericOpenApiExtension(n.GetScalarValue())) }
        };

        public static Paths LoadPaths(IParseNode node)
        {
            IMapNode mapNode = node.CheckMapNode("Paths");

            Paths domainObject = new Paths();

            ParseMap(mapNode, domainObject, PathsFixedFields, PathsPatternFields);

            return domainObject;
        }
        #endregion

        #region PathItemObject

        private static FixedFieldMap<PathItem> PathItemFixedFields = new FixedFieldMap<PathItem>
        {
            { "$ref", (o,n) => { /* Not supported yet */} },
            { "parameters", (o,n) => { o.Parameters = n.CreateList(OpenApiV2Reader.LoadParameter); } },

        };

        private static PatternFieldMap<PathItem> PathItemPatternFields = new PatternFieldMap<PathItem>
        {
            { (s)=> s.StartsWith("x-"), (o,k,n)=> o.Extensions.Add(k,  new GenericOpenApiExtension(n.GetScalarValue())) },
            { (s)=> "get,put,post,delete,patch,options,head,patch".Contains(s),
                (o,k,n)=> o.AddOperation(OperationTypeExtensions.ParseOperationType(k), OpenApiV2Reader.LoadOperation(n)    ) }
        };


        public static PathItem LoadPathItem(IParseNode node)
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
            { "consumes", (o,n) => n.Context.SetTempStorage("operationconsumes", n.CreateSimpleList<String>((s) => s.GetScalarValue()))},
            { "produces", (o,n) => n.Context.SetTempStorage("operationproduces", n.CreateSimpleList<String>((s) => s.GetScalarValue()))},
            { "responses", (o,n) => { o.Responses = n.CreateMap(LoadResponse); } },
            { "deprecated", (o,n) => { o.Deprecated = bool.Parse(n.GetScalarValue()); } },
            { "security", (o,n) => { o.Security = n.CreateList(LoadSecurityRequirement); } },
          };

        private static PatternFieldMap<Operation> OperationPatternFields = new PatternFieldMap<Operation>
        {
            { (s)=> s.StartsWith("x-"), (o,k,n)=> o.Extensions.Add(k, new GenericOpenApiExtension(n.GetScalarValue())) },
        };

        internal static Operation LoadOperation(IParseNode node)
        {
            var mapNode = node.CheckMapNode("Operation");

            Operation operation = new Operation();

            ParseMap(mapNode, operation, OperationFixedFields, OperationPatternFields);

            // Build request body based on information determined while parsing Operation
            var bodyParameter = node.Context.GetTempStorage<Parameter>("bodyParameter");
            if (bodyParameter != null)
            {
                operation.RequestBody = CreateRequestBody(node.Context, bodyParameter);
            }
            else
            {
                var formParameters = node.Context.GetTempStorage<List<Parameter>>("formParameters");
                if (formParameters != null)
                {
                    operation.RequestBody = CreateFormBody(formParameters);
                }
            }


            return operation;
        }

        private static RequestBody CreateFormBody(List<Parameter> formParameters)
        {
                var mediaType = new MediaType()
                {
                    Schema = new Schema()
                    {
                        Properties = formParameters.ToDictionary(k => k.Name, v => v.Schema)
                    }
                };

                var formBody = new RequestBody()
                {
                    Content = new Dictionary<string, MediaType>() {
                            { "application/x-www-form-urlencoded", mediaType } }
                };

                return formBody;
        }

        private static RequestBody CreateRequestBody(ParsingContext context, Parameter bodyParameter)
        {
            var consumes = context.GetTempStorage<List<string>>("operationproduces")
                      ?? context.GetTempStorage<List<string>>("globalproduces")
                      ?? new List<string>() { "application/json" };

            var requestBody = new RequestBody()
            {
                Description = bodyParameter.Description,
                Required = bodyParameter.Required,
                Content = consumes.ToDictionary(k => k, v => new MediaType()
                {
                    Schema = bodyParameter.Schema  // Should we clone this?
                })
            };

            return requestBody;
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


        public static ExternalDocs LoadExternalDocs(IParseNode node)
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
            { "default", (o,n) => { GetOrCreateSchema(o).Default = n.GetScalarValue(); } },
            { "pattern", (o,n) => { GetOrCreateSchema(o).Pattern = n.GetScalarValue(); } },
            { "enum", (o,n) => { GetOrCreateSchema(o).Enum = n.CreateSimpleList<String>(l=>l.GetScalarValue()); } },
            { "schema", (o,n) => { o.Schema = LoadSchema(n); } },
        };

        private static void LoadStyle(Parameter p, string v)
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

        private static Schema GetOrCreateSchema(Parameter p)
        {
            if (p.Schema == null)
            {
                p.Schema = new Schema();
            }
            return p.Schema;
        }

        private static Schema GetOrCreateSchema(Header p)
        {
            if (p.Schema == null)
            {
                p.Schema = new Schema();
            }
            return p.Schema;
        }

        private static void ProcessIn(Parameter o, IParseNode n)
        {

            string value = n.GetScalarValue();
            switch(value)
            {
                case "body":
                    n.Context.SetTempStorage("bodyParameter",o);
                    break;
                case "form":
                    var formParameters = n.Context.GetTempStorage<List<Parameter>>("formParameters");
                    if (formParameters == null)
                    {
                        formParameters = new List<Parameter>();
                        n.Context.SetTempStorage("formParameters", formParameters);
                    }
                    formParameters.Add(o);
                    break;
                default:
                    o.In = (InEnum)Enum.Parse(typeof(InEnum), value);
                    break;
            }
            
        }

        private static PatternFieldMap<Parameter> ParameterPatternFields = new PatternFieldMap<Parameter>
        {
            { (s)=> s.StartsWith("x-"), (o,k,n)=> o.Extensions.Add(k, new GenericOpenApiExtension(n.GetScalarValue())) },
        };


        public static Parameter LoadParameter(IParseNode node)
        {
            var mapNode = node.CheckMapNode("parameter");

            var refpointer = mapNode.GetReferencePointer();
            if (refpointer != null)
            {
                return mapNode.GetReferencedObject<Parameter>(refpointer);
            }

            var parameter = new Parameter();

            ParseMap(mapNode, parameter, ParameterFixedFields, ParameterPatternFields);

            var schema = node.Context.GetTempStorage<Schema>("schema");
            if (schema != null)
            {
                parameter.Schema = schema;
                node.Context.SetTempStorage("schema", null);
            }

            if (parameter.In == 0)
            {
                return null; // Don't include Form or Body parameters in Operation.Parameters list
            }

            return parameter;
        }
        #endregion
        
        #region ResponseObject

        private static FixedFieldMap<Response> ResponseFixedFields = new FixedFieldMap<Response>
        {
            { "description", (o,n) => { o.Description = n.GetScalarValue(); } },
            { "headers", (o,n) => { o.Headers = n.CreateMap(LoadHeader); } },
            { "examples",       (o,n) => { /*o.Examples = ((ListNode)n).Select(s=> new AnyNode(s)).ToList();*/ } },
            { "schema", (o,n) => { n.Context.SetTempStorage("operationschema", LoadSchema(n)); } },

        };

        private static PatternFieldMap<Response> ResponsePatternFields = new PatternFieldMap<Response>
        {
            { (s)=> s.StartsWith("x-"), (o,k,n)=> o.Extensions.Add(k,  new GenericOpenApiExtension(n.GetScalarValue())) },
        };

        public static Response LoadResponse(IParseNode node)
        {
            var mapNode = node.CheckMapNode("response");

            var response = new Response();
            foreach (var property in mapNode)
            {
                property.ParseField(response, ResponseFixedFields, ResponsePatternFields);
            }

            ProcessProduces(response, node.Context);

            return response;
        }

        private static void ProcessProduces(Response response, ParsingContext context)
        {

            var produces = context.GetTempStorage<List<string>>("operationproduces") 
                  ?? context.GetTempStorage<List<string>>("globalproduces") 
                  ?? new List<string>() { "application/json" };

            response.Content = new Dictionary<string, MediaType>();
            foreach (var mt in produces)
            {
                var schema = context.GetTempStorage<Schema>("operationschema");
                MediaType mediaType = null;
                if (schema != null)
                {
                    mediaType = new MediaType()
                    {
                        Schema = schema
                    };
                }
                response.Content.Add(mt, mediaType);
            }

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
            { "type", (o,n) => { GetOrCreateSchema(o).Type = n.GetScalarValue(); } },
            { "format", (o,n) => { GetOrCreateSchema(o).Format = n.GetScalarValue(); } },

        };

        private static PatternFieldMap<Header> HeaderPatternFields = new PatternFieldMap<Header>
        {
            { (s)=> s.StartsWith("x-"), (o,k,n)=> o.Extensions.Add(k,  new GenericOpenApiExtension(n.GetScalarValue())) },
        };


        public static Header LoadHeader(IParseNode node)
        {
            var mapNode = node.CheckMapNode("header");
            var header = new Header();
            foreach (var property in mapNode)
            {
                property.ParseField(header, HeaderFixedFields, HeaderPatternFields);
            }

            var schema = node.Context.GetTempStorage<Schema>("schema");
            if (schema != null)
            {
                header.Schema = schema;
                node.Context.SetTempStorage("schema", null);
            }

            return header;
        }

        #endregion

        #region ExampleObject
        private static FixedFieldMap<Example> ExampleFixedFields = new FixedFieldMap<Example>
        {
        };

        private static PatternFieldMap<Example> ExamplePatternFields = new PatternFieldMap<Example>
        {
            { (s)=> s.StartsWith("x-"), (o,k,n)=> o.Extensions.Add(k,  new GenericOpenApiExtension(n.GetScalarValue())) }
        };

        public static Example LoadExample(IParseNode node)
        {
            var mapNode = node.CheckMapNode("Example");
            var example = new Example();
            foreach (var property in mapNode)
            {
                property.ParseField(example, ExampleFixedFields, ExamplePatternFields);
            }

            return example;
        }


        #endregion

        #region TagObject
        internal static Tag LoadTag(IParseNode n)
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
                { "items", (o,n) => { o.Items = LoadSchema(n); } },
                { "properties", (o,n) => { o.Properties = n.CreateMap(LoadSchema); } },
                { "additionalProperties", (o,n) => { if (n is ValueNode) { o.AdditionalPropertiesAllowed = bool.Parse(n.GetScalarValue()); }
                                                     else { o.AdditionalProperties = LoadSchema(n); }
                                                    } },
                { "description", (o,n) => { o.Type = n.GetScalarValue(); } },
                { "format", (o,n) => { o.Description = n.GetScalarValue(); } },
                { "default", (o,n) => { o.Default = n.GetScalarValue(); } },

                // discriminator
                { "readOnly", (o,n) => { o.ReadOnly = bool.Parse(n.GetScalarValue()); } },
                // xml
                { "externalDocs", (o,n) => { o.ExternalDocs = LoadExternalDocs(n); } },
                { "example", (o,n) => { o.Example = n.GetScalarValue(); } },


        };

        private static PatternFieldMap<Schema> SchemaPatternFields = new PatternFieldMap<Schema>
        {
            { (s)=> s.StartsWith("x-"), (o,k,n)=> o.Extensions.Add(k, new GenericOpenApiExtension(n.GetScalarValue())) }
        };


        public static Schema LoadSchema(IParseNode node)
        {
            IMapNode mapNode = node.CheckMapNode("schema");

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
            { "scopes", (o,n) => { o.Scopes= n.CreateSimpleMap<string>(v => v.GetScalarValue()  ); } },
        };

        private static PatternFieldMap<SecurityScheme> SecuritySchemePatternFields = new PatternFieldMap<SecurityScheme>
        {
            { (s)=> s.StartsWith("x-"), (o,k,n)=> o.Extensions.Add(k, new GenericOpenApiExtension(n.GetScalarValue())) }
        };

        public static SecurityScheme LoadSecurityScheme(IParseNode node)
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
        public static SecurityRequirement LoadSecurityRequirement(IParseNode node)
        {

            var mapNode = node.CheckMapNode("security");

            var obj = new SecurityRequirement();

            foreach (var property in mapNode)
            {
                var scheme = ReferenceService.LoadSecuritySchemeByReference(mapNode.Context, property.Name);
                if (scheme != null)
                {
                    obj.Schemes.Add(scheme, property.Value.CreateSimpleList<string>(n2 => n2.GetScalarValue()));
                } else
                {
                    node.Context.ParseErrors.Add(new OpenApiError(node.Context.GetLocation(), $"Scheme {property.Name} is not found"));
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


        public static IReference LoadReference(OpenApiReference reference, object rootNode)
        {
            IReference referencedObject = null;
            IParseNode node = ((RootNode)rootNode).Find(GetPointer(reference));

            if (node == null &&  reference.ReferenceType != ReferenceType.Tags ) return null;
            switch (reference.ReferenceType)
            {
                case ReferenceType.Schema:
                    referencedObject = OpenApiV2Reader.LoadSchema(node);
                    break;
                case ReferenceType.Parameter:
                    referencedObject = OpenApiV2Reader.LoadParameter(node);
                    break;
                case ReferenceType.SecurityScheme:
                    referencedObject = OpenApiV2Reader.LoadSecurityScheme(node);
                    break;
                case ReferenceType.Tags:
                    ListNode list = (ListNode)node;
                    if (list != null)
                    {
                        foreach (var item in list)
                        {
                            var tag = OpenApiV2Reader.LoadTag(item);

                            if (tag.Name == reference.TypeName)
                            {
                                return tag;
                            }
                        }
                    }
                    else
                    {
                        return new Tag() { Name = reference.TypeName };
                    }
                    break;

                default:
                    throw new DomainParseException($"Unknown $ref {reference.ReferenceType} at {reference.ToString()}");
            }

            return referencedObject;
        }

        private static void ParseMap<T>(IMapNode mapNode, T domainObject, FixedFieldMap<T> fixedFieldMap, PatternFieldMap<T> patternFieldMap, List<string> requiredFields  = null)
        {
            if (mapNode == null) return;

            foreach (var propertyNode in mapNode)
            {
                propertyNode.ParseField<T>(domainObject, fixedFieldMap, patternFieldMap);
                if (requiredFields != null) requiredFields.Remove(propertyNode.Name);
            }
        }

        private static void ReportMissing(IParseNode node, List<string> required)
        {
            node.Context.ParseErrors.AddRange(required.Select(r => new OpenApiError("", $"{r} is a required property")));
        }

    }
}
