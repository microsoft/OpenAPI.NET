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

    public class OpenApiV3Writer : IOpenApiWriter
    {
        Func<Stream, IParseNodeWriter> defaultWriterFactory = s => new YamlParseNodeWriter(s);
        Func<Stream, IParseNodeWriter> writerFactory;

        public OpenApiV3Writer(Func<Stream, IParseNodeWriter> writerFactory = null)
        {
            this.writerFactory = writerFactory ?? defaultWriterFactory;
        }

        public void Write(Stream stream, OpenApiDocument document)
        {

            var writer = writerFactory(stream);
            writer.WriteStartDocument();
            WriteOpenApiDocument(writer,document);
            writer.WriteEndDocument();
            writer.Flush();
        }
        
        public static void WriteOpenApiDocument(IParseNodeWriter writer, OpenApiDocument doc)
        {
            writer.WriteStartMap();

            writer.WritePropertyName("openapi");
            writer.WriteValue("3.0.0");

            writer.WriteObject("info", doc.Info, WriteInfo);
            writer.WriteList("servers", doc.Servers, WriteServer);
            writer.WritePropertyName("paths");

            writer.WriteStartMap();
            WritePaths(writer, doc.Paths);
            writer.WriteEndMap();

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

            writer.WriteEndMap();

        }

        public static void WriteInfo(IParseNodeWriter writer, OpenApiInfo info)
        {
            writer.WriteStartMap();

            writer.WriteStringProperty("title", info.Title);
            writer.WriteStringProperty("description", info.Description);
            writer.WriteStringProperty("termsOfService", info.TermsOfService);
            writer.WriteObject("contact", info.Contact, WriteContact);
            writer.WriteObject("license", info.License, WriteLicense);
            writer.WriteStringProperty("version", info.Version.ToString());

            writer.WriteEndMap();
        }

        public static void WriteContact(IParseNodeWriter writer, OpenApiContact contact)
        {

            writer.WriteStartMap();

            writer.WriteStringProperty("name", contact.Name);
            writer.WriteStringProperty("url", contact.Url?.OriginalString);
            writer.WriteStringProperty("email", contact.Email);

            writer.WriteEndMap();
        }

        public static void WriteLicense(IParseNodeWriter writer, OpenApiLicense license)
        {
            writer.WriteStartMap();

            writer.WriteStringProperty("name", license.Name);
            writer.WriteStringProperty("url", license.Url?.OriginalString);

            writer.WriteEndMap();
        }

        public static void WriteServer(IParseNodeWriter writer, OpenApiServer server)
        {
            writer.WriteStartMap();

            writer.WriteStringProperty("url", server.Url);
            writer.WriteStringProperty("description", server.Description);

            writer.WriteMap("variables", server.Variables, WriteServerVariable);
            writer.WriteEndMap();
        }

        public static void WriteTag(IParseNodeWriter writer, OpenApiTag tag)
        {
            writer.WriteStartMap();
            writer.WriteStringProperty("name", tag.Name);
            writer.WriteStringProperty("description", tag.Description);
            writer.WriteEndMap();
        }

        public static void WriteTagRef(IParseNodeWriter writer, OpenApiTag tag)
        {
            writer.WriteValue(tag.Name);
        }

        public static void WriteComponents(IParseNodeWriter writer, OpenApiComponents components)
        {
            writer.WriteStartMap();

            writer.WriteMap("schemas", components.Schemas, WriteSchema);
            writer.WriteMap("responses", components.Responses, WriteResponse);
            writer.WriteMap("parameters", components.Parameters, WriteParameter);
            writer.WriteMap("examples", components.Examples, WriteExample);
            writer.WriteMap("requestBodies", components.RequestBodies, WriteRequestBody);
            writer.WriteMap("headers", components.Headers, WriteHeader);
            writer.WriteMap("securitySchemes", components.SecuritySchemes, WriteSecurityScheme);
            writer.WriteMap("links", components.Links, WriteLink);
            writer.WriteMap("callbacks", components.Callbacks, WriteCallback);

            writer.WriteEndMap();
        }

        public static void WriteServerVariable(IParseNodeWriter writer, OpenApiServerVariable variable)
        {
            writer.WriteStartMap();

            writer.WriteList("enum", variable.Enum, (nodeWriter, s) => nodeWriter.WriteValue(s));
            writer.WriteStringProperty("default", variable.Default);
            writer.WriteStringProperty("description", variable.Description);

            writer.WriteEndMap();

        }
        public static void WriteExternalDocs(IParseNodeWriter writer, OpenApiExternalDocs externalDocs)
        {
            writer.WriteStartMap();
            writer.WriteStringProperty("description", externalDocs.Description);
            writer.WriteStringProperty("url", externalDocs.Url?.OriginalString);
            writer.WriteEndMap();
        }


        public static void WriteSecurityRequirement(IParseNodeWriter writer, OpenApiSecurityRequirement securityRequirement)
        {

            writer.WriteStartMap();

            foreach (var scheme in securityRequirement.Schemes)
            {

                writer.WritePropertyName(scheme.Key.Pointer.TypeName);
                if (scheme.Value.Count > 0)
                {
                    writer.WriteStartList();
                    foreach (var scope in scheme.Value)
                    {
                        writer.WriteValue(scope);
                    }
                    writer.WriteEndList();
                }
                else
                {
                    writer.WriteValue("[]");
                }
            }

            writer.WriteEndMap();
        }


        public static void WritePaths(IParseNodeWriter writer, OpenApiPaths paths)
        {

            foreach (var pathItem in paths)
            {
                writer.WritePropertyName(pathItem.Key);
                WritePathItem(writer, pathItem.Value);
            }
        }

        public static void WritePathItem(IParseNodeWriter writer, OpenApiPathItem pathItem)
        {
            writer.WriteStartMap();
            writer.WriteStringProperty("summary", pathItem.Summary);
            writer.WriteStringProperty("description", pathItem.Description);
            if (pathItem.Parameters != null && pathItem.Parameters.Count > 0)
            {
                writer.WritePropertyName("parameters");
                writer.WriteStartList();
                foreach (var parameter in pathItem.Parameters)
                {
                    WriteParameter(writer, parameter);
                }
                writer.WriteEndList();

            }
            writer.WriteList("servers", pathItem.Servers, WriteServer);

            foreach (var operationPair in pathItem.Operations)
            {
                writer.WritePropertyName(operationPair.Key);
                WriteOperation(writer, operationPair.Value);
            }
            writer.WriteEndMap();
        }


        public static void WriteOperation(IParseNodeWriter writer, OpenApiOperation operation)
        {
            writer.WriteStartMap();
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

            writer.WriteEndMap();
        }

        public static void WriteParameterOrReference(IParseNodeWriter writer, OpenApiParameter parameter)
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
        public static void WriteParameter(IParseNodeWriter writer, OpenApiParameter parameter)
        {
            writer.WriteStartMap();
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
            writer.WriteObject("example", parameter.Example, (w,s)=> w.WriteRaw(s));
            writer.WriteMap("content", parameter.Content, WriteMediaType);
            writer.WriteEndMap();
        }

        public static void WriteRequestBodyOrReference(IParseNodeWriter writer, OpenApiRequestBody requestBody)
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

        public static void WriteRequestBody(IParseNodeWriter writer, OpenApiRequestBody requestBody)
        {
            writer.WriteStartMap();

            writer.WriteStringProperty("description", requestBody.Description);
            writer.WriteBoolProperty("required", requestBody.Required, false);
            writer.WriteMap("content", requestBody.Content, WriteMediaType);

            writer.WriteEndMap();
        }

        public static void WriteResponseOrReference(IParseNodeWriter writer, OpenApiResponse response)
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

        public static void WriteResponse(IParseNodeWriter writer, OpenApiResponse response)
        {
            writer.WriteStartMap();

            writer.WriteStringProperty("description", response.Description);
            writer.WriteMap("content", response.Content, WriteMediaType);

            writer.WriteMap("headers", response.Headers, WriteHeaderOrReference);
            writer.WriteMap("links", response.Links, WriteLinkOrReference);

            //Links
            writer.WriteEndMap();
        }


        public static void WriteSchemaOrReference(IParseNodeWriter writer, OpenApiSchema schema)
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

        public static void WriteSchema(IParseNodeWriter writer, OpenApiSchema schema)
        {
            writer.WriteStartMap();

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
                writer.WriteStartMap();
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
                writer.WriteEndMap();
            }
            writer.WriteNumberProperty("maxProperties", schema.MaxProperties);
            writer.WriteNumberProperty("minProperties", schema.MinProperties);

            writer.WriteList("enum", schema.Enum, (nodeWriter, s) => nodeWriter.WriteValue(s));

            writer.WriteEndMap();
        }

        public static void WriteLinkOrReference(IParseNodeWriter writer, OpenApiLink link)
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

        public static void WriteLink(IParseNodeWriter writer, OpenApiLink link)
        {
            writer.WriteStartMap();

            writer.WriteStringProperty("href", link.Href);
            writer.WriteStringProperty("operationId", link.OperationId);
            writer.WriteMap("parameters", link.Parameters, (w, x) => { w.WriteValue(x.ToString()); });

            writer.WriteEndMap();
        }

        public static void WriteHeaderOrReference(IParseNodeWriter writer, OpenApiHeader header)
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

        public static void WriteHeader(IParseNodeWriter writer, OpenApiHeader header)
        {
            writer.WriteStartMap();

            writer.WriteStringProperty("description", header.Description);
            writer.WriteBoolProperty("required", header.Required, false);
            writer.WriteBoolProperty("deprecated", header.Deprecated, false);
            writer.WriteBoolProperty("allowEmptyValue", header.AllowEmptyValue, false);
            writer.WriteStringProperty("style", header.Style);
            writer.WriteBoolProperty("explode", header.Explode, false);
            writer.WriteBoolProperty("allowReserved", header.AllowReserved, false);
            writer.WriteObject("schema", header.Schema, WriteSchema);
            writer.WriteList("examples", header.Examples, WriteExampleOrReference);
            writer.WriteObject("example", header.Example, (w,s)=> w.WriteRaw(s));
            writer.WriteMap("content", header.Content, WriteMediaType);

            writer.WriteEndMap();
        }

        public static void WriteMediaType(IParseNodeWriter writer, OpenApiMediaType mediaType)
        {
            writer.WriteStartMap();

            writer.WriteObject("schema", mediaType.Schema, WriteSchemaOrReference);
            writer.WriteObject("example", mediaType.Example, (w,s) => w.WriteRaw(s));
            writer.WriteMap("examples", mediaType.Examples, WriteExampleOrReference);

            writer.WriteEndMap();
        }

        public static void WriteCallbackOrReference(IParseNodeWriter writer, OpenApiCallback callback)
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

        public static void WriteCallback(IParseNodeWriter writer, OpenApiCallback callback)
        {
            writer.WriteStartMap();
            foreach(var item in callback.PathItems)
            {
                writer.WriteObject<OpenApiPathItem>(item.Key.Expression, item.Value, WritePathItem);
            }
            writer.WriteEndMap();
        }

        public static void WriteExampleOrReference(IParseNodeWriter writer, OpenApiExample example)
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

        public static void WriteExample(IParseNodeWriter writer, OpenApiExample example)
        {
            writer.WriteStartMap();
            writer.WriteStringProperty("summary", example.Summary);
            writer.WriteStringProperty("description", example.Description);
            if (example.Value != null)
            {
                writer.WritePropertyName("value");
                writer.WriteRaw(example.Value);
            }
            writer.WriteEndMap();
        }


        public static void WriteSecurityScheme(IParseNodeWriter writer, OpenApiSecurityScheme securityScheme)
        {
            writer.WriteStartMap();
            writer.WriteStringProperty("type", securityScheme.Type);
            switch (securityScheme.Type)
            {
                case "http":
                    writer.WriteStringProperty("scheme", securityScheme.Scheme);
                    writer.WriteStringProperty("bearerFormat", securityScheme.BearerFormat);
                    break;
                case "oauth2":
                //writer.WriteStringProperty("scheme", this.Scheme);
                //TODO:
                case "apiKey":
                    writer.WriteStringProperty("in", securityScheme.In);
                    writer.WriteStringProperty("name", securityScheme.Name);

                    break;
            }
            writer.WriteEndMap();

        }



    }
}
