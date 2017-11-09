// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;

namespace Microsoft.OpenApi.Models
{
    internal static class OpenApiConstants
    {
        public const string OpenApi = "openapi";

        public const string Info = "info";

        public const string Title = "title";

        public const string Type = "type";

        public const string Format = "format";

        public const string Version = "version";

        public const string Contact = "contact";

        public const string License = "license";

        public const string TermsOfService = "termsOfService";

        public const string Servers = "servers";

        public const string Server = "server";

        public const string Paths = "paths";

        public const string Components = "components";

        public const string Security = "security";

        public const string Tags = "tags";

        public const string ExternalDocs = "externalDocs";

        public const string OperationRef = "operationRef";

        public const string OperationId = "operationId";

        public const string Parameters = "parameters";

        public const string RequestBody = "requestBody";

        public const string ExtensionFieldNamePrefix = "x-";

        public const string Name = "name";

        public const string Namespace = "namespace";

        public const string Prefix = "prefix";

        public const string Attribute = "attribute";

        public const string Wrapped = "wrapped";

        public const string In = "in";

        public const string Summary = "summary";

        public const string Variables = "variables";

        public const string Description = "description";

        public const string Required = "required";

        public const string Deprecated = "deprecated";

        public const string Style = "style";

        public const string Explode = "explode";

        public const string AllowReserved = "allowReserved";

        public const string Schema = "schema";

        public const string Schemas = "schemas";

        public const string Responses = "responses";

        public const string Example = "example";

        public const string Examples = "examples";

        public const string Encoding = "encoding";

        public const string RequestBodies = "requestBodies";

        public const string AllowEmptyValue = "allowEmptyValue";

        public const string Value = "value";

        public const string ExternalValue = "externalValue";

        public const string DollarRef = "$ref";

        public const string Headers = "headers";

        public const string SecuritySchemes = "securitySchemes";

        public const string Content = "content";

        public const string Links = "links";

        public const string Callbacks = "callbacks";

        public const string Url = "url";

        public const string Email = "email";

        public const string Default = "default";

        public const string Enum = "enum";

        public const string MultipleOf = "multipleOf";

        public const string Maximum = "maximum";

        public const string ExclusiveMaximum = "exclusiveMaximum";

        public const string Minimum = "minimum";

        public const string ExclusiveMinimum = "exclusiveMinimum";

        public const string MaxLength = "maxLength";

        public const string MinLength = "minLength";

        public const string Pattern = "pattern";

        public const string MaxItems = "maxItems";

        public const string MinItems = "minItems";

        public const string UniqueItems = "uniqueItems";

        public const string MaxProperties = "maxProperties";

        public const string MinProperties = "minProperties";

        public const string AllOf = "allOf";

        public const string OneOf = "oneOf";

        public const string AnyOf = "anyOf";

        public const string Not = "not";

        public const string Items = "items";

        public const string Properties = "properties";

        public const string AdditionalProperties = "additionalProperties";

        public const string Nullable = "nullable";

        public const string Discriminator = "discriminator";

        public const string ReadOnly = "readOnly";

        public const string WriteOnly = "writeOnly";

        public const string Xml = "xml";

        public const string Flow = "flow";

        public const string Application = "application";

        public const string AccessCode = "accessCode";

        public const string Implicit = "implicit";

        public const string Password = "password";

        public const string ClientCredentials = "clientCredentials";

        public const string AuthorizationCode = "authorizationCode";

        public const string AuthorizationUrl = "authorizationUrl";

        public const string TokenUrl = "tokenUrl";

        public const string RefreshUrl = "refreshUrl";

        public const string Scopes = "scopes";

        public const string ContentType = "contentType";

        public const string Get = "get";

        public const string Put = "put";

        public const string Post = "post";

        public const string Delete = "delete";

        public const string Options = "options";

        public const string Head = "head";

        public const string Patch = "patch";

        public const string Trace = "trace";

        public const string PropertyName = "propertyName";

        public const string Mapping = "mapping";

        public const string Scheme = "scheme";

        public const string BearerFormat = "bearerFormat";

        public const string Flows = "flows";

        public const string OpenIdConnectUrl = "openIdConnectUrl";

        public const string DefaultName = "Default Name";

        public const string DefaultDefault = "Default Default";

        public const string DefaultTitle = "Default Title";

        public static readonly Version DefaultVersion = new Version(3, 0, 0);

        public static readonly Uri DefaultUrl = new Uri("http://localhost/");

        public const string DefaultDescription = "Default Description";

        #region V2
        public const string Host = "host";

        public const string Swagger = "swagger";

        public static readonly Version SwaggerVersion = new Version(2, 0);

        public const string BasePath = "basePath";

        public const string Schemes = "schemes";

        public const string SecurityDefinitions = "securityDefinitions";

        public const string Definitions = "definitions";

        public const string Basic = "basic";

        public const string Consumes = "consumes";

        public const string Produces = "produces";

        #endregion
    }
}
