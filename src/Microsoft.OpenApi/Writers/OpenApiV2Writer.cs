//---------------------------------------------------------------------
// <copyright file="OpenApiV2Writer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OpenApi.Writers
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Collections.Generic;

    public class OpenApiV2Writer : IOpenApiStructureWriter
    {
        Func<Stream, IOpenApiWriter> defaultWriterFactory = s => new OpenApiYamlWriter(new StreamWriter(s));
        Func<Stream, IOpenApiWriter> writerFactory;

        public OpenApiV2Writer(Func<Stream, IOpenApiWriter> writerFactory = null)
        {
            this.writerFactory = writerFactory ?? defaultWriterFactory;
        }

        public void Write(Stream stream, OpenApiDocument document)
        {

            var writer = writerFactory(stream);
           // writer.WriteStartDocument();
            WriteOpenApiDocument(writer, document);
           // writer.WriteEndDocument();
            writer.Flush();
        }

        public static void WriteOpenApiDocument(IOpenApiWriter writer, OpenApiDocument doc)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("swagger");
            writer.WriteValue("2.0");

            writer.WriteObject("info", doc.Info, WriteInfo);
            WriteHostInfo(writer, doc.Servers);

            writer.WritePropertyName("paths");

            writer.WriteStartObject();
            WritePaths(writer, doc.Paths);
            writer.WriteEndObject();

            writer.WriteList("tags", doc.Tags, WriteTag);
            if (!doc.Components.IsEmpty())
            {
                WriteComponents(writer, doc.Components);
            }
            if (doc.ExternalDocs.Url != null)
            {
                writer.WriteObject("externalDocs", doc.ExternalDocs, WriteExternalDocs);
            }
            writer.WriteList("security", doc.SecurityRequirements, WriteSecurityRequirement);

            writer.WriteEndObject();

        }

        private static void WriteHostInfo(IOpenApiWriter writer, IList<OpenApiServer> servers)
        {
            if (servers == null || servers.Count == 0) return;
            var firstServer = servers.First();
            var url = new Uri(firstServer.Url);
            writer.WriteStringProperty("host", url.GetComponents(UriComponents.Host | UriComponents.Port, UriFormat.SafeUnescaped));
            writer.WriteStringProperty("basePath", url.AbsolutePath);
            var schemes = servers.Select(s => new Uri(s.Url).Scheme).Distinct();
            writer.WritePropertyName("schemes");
            writer.WriteStartArray();
            foreach(var scheme in schemes)
            {
                writer.WriteValue(scheme);
            }
            writer.WriteEndArray();
        }

        public static void WriteInfo(IOpenApiWriter writer, OpenApiInfo info)
        {
            writer.WriteStartObject();

            writer.WriteStringProperty("title", info.Title);
            writer.WriteStringProperty("description", info.Description);
            writer.WriteStringProperty("termsOfService", info.TermsOfService);
            writer.WriteObject("contact", info.Contact, WriteContact);
            writer.WriteObject("license", info.License, WriteLicense);
            writer.WriteStringProperty("version", info.Version.ToString());

            writer.WriteEndObject();
        }

        public static void WriteContact(IOpenApiWriter writer, OpenApiContact contact)
        {

            writer.WriteStartObject();

            writer.WriteStringProperty("name", contact.Name);
            writer.WriteStringProperty("url", contact.Url?.OriginalString);
            writer.WriteStringProperty("email", contact.Email);

            writer.WriteEndObject();
        }

        public static void WriteLicense(IOpenApiWriter writer, OpenApiLicense license)
        {
            writer.WriteStartObject();

            writer.WriteStringProperty("name", license.Name);
            writer.WriteStringProperty("url", license.Url?.OriginalString);

            writer.WriteEndObject();
        }
        public static void WriteTag(IOpenApiWriter writer, OpenApiTag tag)
        {
            writer.WriteStartObject();
            writer.WriteStringProperty("name", tag.Name);
            writer.WriteStringProperty("description", tag.Description);
            writer.WriteEndObject();
        }

        public static void WriteTagRef(IOpenApiWriter writer, OpenApiTag tag)
        {
            writer.WriteValue(tag.Name);
        }
        public static void WriteComponents(IOpenApiWriter writer, OpenApiComponents components)
        {
            writer.WriteMap("definitions", components.Schemas, WriteSchema);
            writer.WriteMap("responses", components.Responses, WriteResponse);
            writer.WriteMap("parameters", components.Parameters, WriteParameter);
            writer.WriteMap("securityDefinitions", components.SecuritySchemes, WriteSecurityScheme);

        }
        
        public static void WriteExternalDocs(IOpenApiWriter writer, OpenApiExternalDocs externalDocs)
        {
            writer.WriteStartObject();
            writer.WriteStringProperty("description", externalDocs.Description);
            writer.WriteStringProperty("url", externalDocs.Url?.OriginalString);
            writer.WriteEndObject();
        }


        public static void WriteSecurityRequirement(IOpenApiWriter writer, OpenApiSecurityRequirement securityRequirement)
        {

            writer.WriteStartObject();

            foreach (var scheme in securityRequirement.Schemes)
            {

                writer.WritePropertyName(scheme.Key.Pointer.TypeName);
                writer.WriteStartArray();
                    
                foreach (var scope in scheme.Value)
                {
                    writer.WriteValue(scope);
                }

                writer.WriteEndArray();
            }

            writer.WriteEndObject();
        }


        public static void WritePaths(IOpenApiWriter writer, OpenApiPaths paths)
        {
            foreach (var pathItem in paths)
            {
                writer.WritePropertyName(pathItem.Key);
                WritePathItem(writer, pathItem.Value);
            }
        }

        public static void WritePathItem(IOpenApiWriter writer, OpenApiPathItem pathItem)
        {
            writer.WriteStartObject();
            writer.WriteStringProperty("x-summary", pathItem.Summary);
            writer.WriteStringProperty("x-description", pathItem.Description);
            if (pathItem.Parameters != null && pathItem.Parameters.Count > 0)
            {
                writer.WritePropertyName("parameters");
                writer.WriteStartArray();
                foreach (var parameter in pathItem.Parameters)
                {
                    WriteParameter(writer, parameter);
                }
                writer.WriteEndArray();

            }
            //writer.WriteList("x-servers", pathItem.Servers, WriteServer);

            foreach (var operationPair in pathItem.Operations)
            {
                writer.WritePropertyName(operationPair.Key);
                WriteOperation(writer, operationPair.Value);
            }
            writer.WriteEndObject();
        }


        public static void WriteOperation(IOpenApiWriter writer, OpenApiOperation operation)
        {
            writer.WriteStartObject();
            writer.WriteList("tags", operation.Tags, WriteTagRef);
            writer.WriteStringProperty("summary", operation.Summary);
            writer.WriteStringProperty("description", operation.Description);
            writer.WriteObject("externalDocs", operation.ExternalDocs, WriteExternalDocs);

            writer.WriteStringProperty("operationId", operation.OperationId);

            var parameters = new List<OpenApiParameter>(operation.Parameters);

            OpenApiParameter bodyParameter = null;
            if (operation.RequestBody != null)
            {
                writer.WritePropertyName("consumes");
                writer.WriteStartArray();
                var consumes = operation.RequestBody.Content.Keys.Distinct();
                foreach (var mediaType in consumes)
                {
                    writer.WriteValue(mediaType);
                }
                writer.WriteEndArray();

                // Create bodyParameter
                bodyParameter = new BodyParameter()
                {
                    Name = "body",
                    Description = operation.RequestBody.Description,
                    Schema = operation.RequestBody.Content.First().Value.Schema
                };
                // add to parameters
                parameters.Add(bodyParameter);
            }

            var produces = operation.Responses.Where(r => r.Value.Content != null).SelectMany(r => r.Value.Content?.Keys).Distinct();
            if (produces.Count() > 0)
            {
                writer.WritePropertyName("produces");
                writer.WriteStartArray();
                foreach (var mediaType in produces)
                {
                    writer.WriteValue(mediaType);
                }
                writer.WriteEndArray();
            }

            writer.WriteList<OpenApiParameter>("parameters", parameters, WriteParameterOrReference);
            writer.WriteMap<OpenApiResponse>("responses", operation.Responses, WriteResponseOrReference);
            writer.WriteBoolProperty("deprecated", operation.Deprecated, OpenApiOperation.DeprecatedDefault);
            writer.WriteList("security", operation.Security, WriteSecurityRequirement);

            writer.WriteEndObject();
        }

        public static void WriteParameterOrReference(IOpenApiWriter writer, OpenApiParameter parameter)
        {
            if (parameter.IsReference())
            {
                parameter.WriteRef(writer);
                return;
            }
            else
            {
                WriteParameter(writer, parameter);
            }
        }
        public static void WriteParameter(IOpenApiWriter writer, OpenApiParameter parameter)
        {
            writer.WriteStartObject();
            writer.WriteStringProperty("name", parameter.Name);
            if (parameter is BodyParameter)
            {
                writer.WriteStringProperty("in", "body");   // form?
            }
            else
            {
                writer.WriteStringProperty("in", parameter.In.ToString());
            }
            writer.WriteStringProperty("description", parameter.Description);
            writer.WriteBoolProperty("required", parameter.Required, false);
            writer.WriteBoolProperty("deprecated", parameter.Deprecated, false);
            writer.WriteBoolProperty("allowEmptyValue", parameter.AllowEmptyValue, false);

            writer.WriteBoolProperty("allowReserved", parameter.AllowReserved, false);
            if (parameter is BodyParameter)
            {
                writer.WriteObject("schema", parameter.Schema, WriteSchemaOrReference);
            }
            else
            {
                WriteSchemaProperties(writer, parameter.Schema);
            }
//            writer.WriteList("examples", parameter.Examples, AnyNode.Write);
//            writer.WriteObject("example", parameter.Example, AnyNode.Write);
            writer.WriteEndObject();
        }



        public static void WriteResponseOrReference(IOpenApiWriter writer, OpenApiResponse response)
        {
            if (response.IsReference())
            {
                response.WriteRef(writer);
            }
            else
            {
                WriteResponse(writer, response);
            }
        }

        public static void WriteResponse(IOpenApiWriter writer, OpenApiResponse response)
        {
            writer.WriteStartObject();

            writer.WriteStringProperty("description", response.Description);
            if (response.Content != null)
            {
                var mediatype = response.Content.FirstOrDefault();
                if (mediatype.Value != null)
                {

                    writer.WriteObject("schema", mediatype.Value.Schema, WriteSchemaOrReference);

                    if (mediatype.Value.Example != null)
                    {
                        writer.WritePropertyName("examples");
                        writer.WriteStartObject();
                        writer.WritePropertyName(mediatype.Key);
                        writer.WriteValue(mediatype.Value.Example);
                        writer.WriteEndObject();
                    }
                    
                }

            }
            writer.WriteMap("headers", response.Headers, WriteHeaderOrReference);
 
            writer.WriteEndObject();
        }


        public static void WriteSchemaOrReference(IOpenApiWriter writer, OpenApiSchema schema)
        {
            if (schema.IsReference())
            {
                schema.WriteRef(writer);
            }
            else
            {
                WriteSchema(writer, schema);
            }
        }

        public static void WriteSchema(IOpenApiWriter writer, OpenApiSchema schema)
        {
            writer.WriteStartObject();

            WriteSchemaProperties(writer, schema);

            writer.WriteEndObject();
        }

        private static void WriteSchemaProperties(IOpenApiWriter writer, OpenApiSchema schema)
        {
            writer.WriteStringProperty("title", schema.Title);
            writer.WriteStringProperty("type", schema.Type);
            writer.WriteStringProperty("format", schema.Format);
            writer.WriteStringProperty("description", schema.Description);

            writer.WriteNumberProperty("maxLength", schema.MaxLength);
            writer.WriteNumberProperty("minLength", schema.MinLength);
            writer.WriteStringProperty("pattern", schema.Pattern);
            writer.WriteStringProperty("default", schema.Default);

            writer.WriteList("required", schema.Required, (nodeWriter, s) => nodeWriter.WriteValue(s));

            writer.WriteNumberProperty("maximum", schema.Maximum);
            writer.WriteBoolProperty("exclusiveMaximum", schema.ExclusiveMaximum, false);
            writer.WriteNumberProperty("minimum", schema.Minimum);
            writer.WriteBoolProperty("exclusiveMinimum", schema.ExclusiveMinimum, false);

            if (schema.AdditionalProperties != null)
            {
                writer.WritePropertyName("additionalProperties");
                WriteSchemaOrReference(writer, schema.AdditionalProperties);
            }

            if (schema.Items != null)
            {
                writer.WritePropertyName("items");
                WriteSchemaOrReference(writer, schema.Items);
            }
            writer.WriteNumberProperty("maxItems", schema.MaxItems);
            writer.WriteNumberProperty("minItems", schema.MinItems);

            if (schema.Properties != null)
            {
                writer.WritePropertyName("properties");
                writer.WriteStartObject();
                foreach (var prop in schema.Properties)
                {
                    writer.WritePropertyName(prop.Key);
                    if (prop.Value != null)
                    {
                        WriteSchemaOrReference(writer, prop.Value);
                    }
                    else
                    {
                        writer.WriteValue("null");
                    }
                }
                writer.WriteEndObject();
            }
            writer.WriteNumberProperty("maxProperties", schema.MaxProperties);
            writer.WriteNumberProperty("minProperties", schema.MinProperties);

            writer.WriteList("enum", schema.Enum, (nodeWriter, s) => nodeWriter.WriteValue(s));
        }

        public static void WriteHeaderOrReference(IOpenApiWriter writer, OpenApiHeader header)
        {
            if (header.IsReference())
            {
                header.WriteRef(writer);
            }
            else
            {
                WriteHeader(writer, header);
            }
        }

        public static void WriteHeader(IOpenApiWriter writer, OpenApiHeader header)
        {
            writer.WriteStartObject();

            writer.WriteStringProperty("description", header.Description);
            writer.WriteBoolProperty("required", header.Required, false);
            writer.WriteBoolProperty("deprecated", header.Deprecated, false);
            writer.WriteBoolProperty("allowEmptyValue", header.AllowEmptyValue, false);
            writer.WriteStringProperty("style", header.Style);
            writer.WriteBoolProperty("explode", header.Explode, false);
            writer.WriteBoolProperty("allowReserved", header.AllowReserved, false);
            writer.WriteObject("schema", header.Schema, WriteSchema);
            writer.WriteStringProperty("example", header.Example);

            writer.WriteEndObject();
        }

        public static void WriteSecurityScheme(IOpenApiWriter writer, OpenApiSecurityScheme securityScheme)
        {
            writer.WriteStartObject();
            if (securityScheme.Type == SecuritySchemeTypeKind.http)
            {
                if (securityScheme.Scheme == "basic")
                {
                    writer.WriteStringProperty("type", "basic");
                }
            }
            else
            {
                writer.WriteStringProperty("type", securityScheme.Type.ToString());
            }
            switch (securityScheme.Type)
            {
                case SecuritySchemeTypeKind.oauth2:
                //writer.WriteStringProperty("scheme", this.Scheme);
                //TODO:
                case SecuritySchemeTypeKind.apiKey:
                    writer.WriteStringProperty("in", securityScheme.In.ToString());
                    writer.WriteStringProperty("name", securityScheme.Name);

                    break;
            }

            writer.WriteObject("flows", securityScheme.Flows, WriteOAuthFlows);

            writer.WriteEndObject();
        }

        public static void WriteOAuthFlows(IOpenApiWriter writer, OpenApiOAuthFlows oAuthFlows)
        {
            writer.WriteStartObject();

            writer.WriteObject("implicit", oAuthFlows.Implicit, WriteOAuthFlow);
            writer.WriteObject("password", oAuthFlows.Password, WriteOAuthFlow);
            writer.WriteObject("clientCredentials", oAuthFlows.ClientCredentials, WriteOAuthFlow);
            writer.WriteObject("authorizationCode", oAuthFlows.AuthorizationCode, WriteOAuthFlow);

            writer.WriteEndObject();
        }

        public static void WriteOAuthFlow(IOpenApiWriter writer, OpenApiOAuthFlow oAuthFlow)
        {
            writer.WriteStartObject();

            writer.WriteStringProperty("authorizationUrl", oAuthFlow.AuthorizationUrl?.ToString());
            writer.WriteStringProperty("tokenUrl", oAuthFlow.TokenUrl?.ToString());
            writer.WriteStringProperty("refreshUrl", oAuthFlow.RefreshUrl?.ToString());
            writer.WriteMap("scopes", oAuthFlow.Scopes, WriteValue);
            writer.WriteEndObject();
        }

        private static void WriteValue(IOpenApiWriter writer, string value)
        {
            writer.WriteValue(value);
        }
    }

    internal class BodyParameter : OpenApiParameter
    {

    }
}
