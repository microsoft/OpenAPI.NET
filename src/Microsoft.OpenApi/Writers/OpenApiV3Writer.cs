//---------------------------------------------------------------------
// <copyright file="OpenApiV3Writer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OpenApi.Writers
{
    using System;
    using System.IO;
    using Microsoft.OpenApi;

    public class OpenApiV3Writer : IOpenApiStructureWriter
    {
        Func<Stream, IOpenApiWriter> defaultWriterFactory = s => new OpenApiYamlWriter(new StreamWriter(s));
        Func<Stream, IOpenApiWriter> writerFactory;

        public OpenApiV3Writer(Func<Stream, IOpenApiWriter> writerFactory = null)
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

            writer.WritePropertyName("openapi");
            writer.WriteValue("3.0.0");

            writer.WriteObject("info", doc.Info, WriteInfo);
            writer.WriteList("servers", doc.Servers, WriteServer);
            writer.WritePropertyName("paths");

            writer.WriteStartObject();
            WritePaths(writer, doc.Paths);
            writer.WriteEndObject();

            writer.WriteList("tags", doc.Tags, WriteTag);
            if (!doc.Components.IsEmpty())
            {
                writer.WriteObject("components", doc.Components, WriteComponents);
            }
            if (doc.ExternalDocs.Url != null)
            {
                writer.WriteObject("externalDocs", doc.ExternalDocs, WriteExternalDocs);
            }
            writer.WriteList("security", doc.SecurityRequirements, WriteSecurityRequirement);

            writer.WriteEndObject();

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

        public static void WriteServer(IOpenApiWriter writer, OpenApiServer server)
        {
            writer.WriteStartObject();

            writer.WriteStringProperty("url", server.Url);
            writer.WriteStringProperty("description", server.Description);

            writer.WriteMap("variables", server.Variables, WriteServerVariable);
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
            writer.WriteStartObject();

            writer.WriteMap("schemas", components.Schemas, WriteSchema);
            writer.WriteMap("responses", components.Responses, WriteResponse);
            writer.WriteMap("parameters", components.Parameters, WriteParameter);
            writer.WriteMap("examples", components.Examples, WriteExample);
            writer.WriteMap("requestBodies", components.RequestBodies, WriteRequestBody);
            writer.WriteMap("headers", components.Headers, WriteHeader);
            writer.WriteMap("securitySchemes", components.SecuritySchemes, WriteSecurityScheme);
            writer.WriteMap("links", components.Links, WriteLink);
            writer.WriteMap("callbacks", components.Callbacks, WriteCallback);

            writer.WriteEndObject();
        }

        public static void WriteServerVariable(IOpenApiWriter writer, OpenApiServerVariable variable)
        {
            writer.WriteStartObject();

            writer.WriteList("enum", variable.Enum, (nodeWriter, s) => nodeWriter.WriteValue(s));
            writer.WriteStringProperty("default", variable.Default);
            writer.WriteStringProperty("description", variable.Description);

            writer.WriteEndObject();

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
                if (scheme.Value.Count > 0)
                {
                    writer.WriteStartArray();
                    foreach (var scope in scheme.Value)
                    {
                        writer.WriteValue(scope);
                    }
                    writer.WriteEndArray();
                }
                else
                {
                    writer.WriteValue("[]");
                }
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
            writer.WriteStringProperty("summary", pathItem.Summary);
            writer.WriteStringProperty("description", pathItem.Description);
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
            writer.WriteList("servers", pathItem.Servers, WriteServer);

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
            writer.WriteList<OpenApiParameter>("parameters", operation.Parameters, WriteParameterOrReference);
            writer.WriteObject("requestBody", operation.RequestBody, WriteRequestBodyOrReference);
            writer.WriteMap<OpenApiResponse>("responses", operation.Responses, WriteResponseOrReference);
            writer.WriteMap<OpenApiCallback>("callbacks", operation.Callbacks, WriteCallbackOrReference);
            writer.WriteBoolProperty("deprecated", operation.Deprecated, OpenApiOperation.DeprecatedDefault);
            writer.WriteList("security", operation.Security, WriteSecurityRequirement);
            writer.WriteList("servers", operation.Servers, WriteServer);

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
            writer.WriteStringProperty("in", parameter.In.ToString());
            writer.WriteStringProperty("description", parameter.Description);
            writer.WriteBoolProperty("required", parameter.Required, false);
            writer.WriteBoolProperty("deprecated", parameter.Deprecated, false);
            writer.WriteBoolProperty("allowEmptyValue", parameter.AllowEmptyValue, false);
            writer.WriteStringProperty("style", parameter.Style);
            writer.WriteBoolProperty("explode", parameter.Explode, false);
            writer.WriteBoolProperty("allowReserved", parameter.AllowReserved, false);
            writer.WriteObject("schema", parameter.Schema, WriteSchemaOrReference);
            writer.WriteList("examples", parameter.Examples, WriteExample);
            writer.WriteObject("example", parameter.Example, (w, s) => w.WriteRaw(s));
            writer.WriteMap("content", parameter.Content, WriteMediaType);
            writer.WriteEndObject();
        }

        public static void WriteRequestBodyOrReference(IOpenApiWriter writer, OpenApiRequestBody requestBody)
        {
            if (requestBody.IsReference())
            {
                requestBody.WriteRef(writer);
            }
            else
            {
                WriteRequestBody(writer, requestBody);
            }
        }

        public static void WriteRequestBody(IOpenApiWriter writer, OpenApiRequestBody requestBody)
        {
            writer.WriteStartObject();

            writer.WriteStringProperty("description", requestBody.Description);
            writer.WriteBoolProperty("required", requestBody.Required, false);
            writer.WriteMap("content", requestBody.Content, WriteMediaType);

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
            writer.WriteMap("content", response.Content, WriteMediaType);

            writer.WriteMap("headers", response.Headers, WriteHeaderOrReference);
            writer.WriteMap("links", response.Links, WriteLinkOrReference);

            //Links
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

            writer.WriteEndObject();
        }

        public static void WriteLinkOrReference(IOpenApiWriter writer, OpenApiLink link)
        {
            if (link.IsReference())
            {
                link.WriteRef(writer);
            }
            else
            {
                WriteLink(writer, link);
            }
        }

        public static void WriteLink(IOpenApiWriter writer, OpenApiLink link)
        {
            writer.WriteStartObject();

            writer.WriteStringProperty("href", link.Href);
            writer.WriteStringProperty("operationId", link.OperationId);
            writer.WriteMap("parameters", link.Parameters, (w, x) => { w.WriteValue(x.ToString()); });

            writer.WriteEndObject();
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
            writer.WriteList("examples", header.Examples, WriteExampleOrReference);
            writer.WriteObject("example", header.Example, (w, s) => w.WriteRaw(s));
            writer.WriteMap("content", header.Content, WriteMediaType);

            writer.WriteEndObject();
        }

        public static void WriteMediaType(IOpenApiWriter writer, OpenApiMediaType mediaType)
        {
            writer.WriteStartObject();

            writer.WriteObject("schema", mediaType.Schema, WriteSchemaOrReference);
            writer.WriteObject("example", mediaType.Example, (w, s) => w.WriteRaw(s));
            writer.WriteMap("examples", mediaType.Examples, WriteExampleOrReference);

            writer.WriteEndObject();
        }

        public static void WriteCallbackOrReference(IOpenApiWriter writer, OpenApiCallback callback)
        {
            if (callback.IsReference())
            {
                callback.WriteRef(writer);
            }
            else
            {
                WriteCallback(writer, callback);
            }
        }

        public static void WriteCallback(IOpenApiWriter writer, OpenApiCallback callback)
        {
            writer.WriteStartObject();
            foreach (var item in callback.PathItems)
            {
                writer.WriteObject<OpenApiPathItem>(item.Key.Expression, item.Value, WritePathItem);
            }
            writer.WriteEndObject();
        }

        public static void WriteExampleOrReference(IOpenApiWriter writer, OpenApiExample example)
        {
            if (example.IsReference())
            {
                example.WriteRef(writer);
            }
            else
            {
                WriteExample(writer, example);
            }
        }

        public static void WriteExample(IOpenApiWriter writer, OpenApiExample example)
        {
            writer.WriteStartObject();
            writer.WriteStringProperty("summary", example.Summary);
            writer.WriteStringProperty("description", example.Description);
            if (example.Value != null)
            {
                writer.WritePropertyName("value");
                writer.WriteRaw(example.Value);
            }
            writer.WriteEndObject();
        }

        public static void WriteSecurityScheme(IOpenApiWriter writer, OpenApiSecurityScheme securityScheme)
        {
            writer.WriteStartObject();
            writer.WriteStringProperty("type", securityScheme.Type.ToString());
            switch (securityScheme.Type)
            {
                case SecuritySchemeTypeKind.http:
                    writer.WriteStringProperty("scheme", securityScheme.Scheme);
                    writer.WriteStringProperty("bearerFormat", securityScheme.BearerFormat);
                    break;
                case SecuritySchemeTypeKind.oauth2:
                //writer.WriteStringProperty("scheme", this.Scheme);
                //TODO:
                case SecuritySchemeTypeKind.apiKey:
                    writer.WriteStringProperty("in", securityScheme.In.ToString());
                    writer.WriteStringProperty("name", securityScheme.Name);

                    break;
            }

            writer.WriteObject("flows", securityScheme.Flows, WriteOAuthFlows);

            writer.WriteStringProperty("openIdConnectUrl", securityScheme.OpenIdConnectUrl?.ToString());

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
            writer.WriteMap("scopes", oAuthFlow.Scopes, (w, s) => w.WriteValue(s));
            writer.WriteEndObject();
        }
    }
}
