//---------------------------------------------------------------------
// <copyright file="OpenApiV2Writer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace Microsoft.OpenApi.Writers
{
    public class OpenApiV2Writer : IOpenApiWriter
    {
        Func<Stream, IParseNodeWriter> defaultWriterFactory = s => new YamlParseNodeWriter(s);
        Func<Stream, IParseNodeWriter> writerFactory;

        public OpenApiV2Writer(Func<Stream, IParseNodeWriter> writerFactory = null)
        {
            this.writerFactory = writerFactory ?? defaultWriterFactory;
        }

        public void Write(Stream stream, OpenApiDocument document)
        {

            var writer = writerFactory(stream);
            writer.WriteStartDocument();
            WriteOpenApiDocument(writer, document);
            writer.WriteEndDocument();
            writer.Flush();
        }

        public static void WriteOpenApiDocument(IParseNodeWriter writer, OpenApiDocument doc)
        {
            writer.WriteStartMap();

            writer.WritePropertyName("swagger");
            writer.WriteValue("2.0");

            writer.WriteObject("info", doc.Info, WriteInfo);
            WriteHostInfo(writer, doc.Servers);

            writer.WritePropertyName("paths");

            writer.WriteStartMap();
            WritePaths(writer, doc.Paths);
            writer.WriteEndMap();

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

            writer.WriteEndMap();

        }

        private static void WriteHostInfo(IParseNodeWriter writer, IList<OpenApiServer> servers)
        {
            if (servers == null || servers.Count == 0) return;
            var firstServer = servers.First();
            var url = new Uri(firstServer.Url);
            writer.WriteStringProperty("host", url.GetComponents(UriComponents.Host | UriComponents.Port, UriFormat.SafeUnescaped));
            writer.WriteStringProperty("basePath", url.AbsolutePath);
            var schemes = servers.Select(s => new Uri(s.Url).Scheme).Distinct();
            writer.WritePropertyName("schemes");
            writer.WriteStartList();
            foreach(var scheme in schemes)
            {
                writer.WriteListItem(scheme, (w, s) => w.WriteValue(s));
            }
            writer.WriteEndList();
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
            writer.WriteMap("definitions", components.Schemas, WriteSchema);
            writer.WriteMap("responses", components.Responses, WriteResponse);
            writer.WriteMap("parameters", components.Parameters, WriteParameter);
            writer.WriteMap("securityDefinitions", components.SecuritySchemes, WriteSecurityScheme);

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
                writer.WriteStartList();
                    
                foreach (var scope in scheme.Value)
                {
                    writer.WriteValue(scope);
                }

                writer.WriteEndList();
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
            writer.WriteStringProperty("x-summary", pathItem.Summary);
            writer.WriteStringProperty("x-description", pathItem.Description);
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
            //writer.WriteList("x-servers", pathItem.Servers, WriteServer);

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

            var parameters = new List<OpenApiParameter>(operation.Parameters);

            OpenApiParameter bodyParameter = null;
            if (operation.RequestBody != null)
            {
                writer.WritePropertyName("consumes");
                writer.WriteStartList();
                var consumes = operation.RequestBody.Content.Keys.Distinct();
                foreach (var mediaType in consumes)
                {
                    writer.WriteListItem(mediaType, (w, s) => w.WriteValue(s));
                }
                writer.WriteEndList();

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
                writer.WriteStartList();
                foreach (var mediaType in produces)
                {
                    writer.WriteListItem(mediaType, (w, s) => w.WriteValue(s));
                }
                writer.WriteEndList();
            }

            writer.WriteList<OpenApiParameter>("parameters", parameters, WriteParameterOrReference);
            writer.WriteMap<OpenApiResponse>("responses", operation.Responses, WriteResponseOrReference);
            writer.WriteBoolProperty("deprecated", operation.Deprecated, OpenApiOperation.DeprecatedDefault);
            writer.WriteList("security", operation.Security, WriteSecurityRequirement);

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
            if (response.Content != null)
            {
                var mediatype = response.Content.FirstOrDefault();
                if (mediatype.Value != null)
                {

                    writer.WriteObject("schema", mediatype.Value.Schema, WriteSchemaOrReference);

                    if (mediatype.Value.Example != null)
                    {
                        writer.WritePropertyName("examples");
                        writer.WriteStartMap();
                        writer.WritePropertyName(mediatype.Key);
                        writer.WriteValue(mediatype.Value.Example);
                        writer.WriteEndMap();
                    }
                    
                }

            }
            writer.WriteMap("headers", response.Headers, WriteHeaderOrReference);
 
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

            WriteSchemaProperties(writer, schema);

            writer.WriteEndMap();
        }

        private static void WriteSchemaProperties(IParseNodeWriter writer, OpenApiSchema schema)
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
            writer.WriteStringProperty("example", header.Example);

            writer.WriteEndMap();
        }

        

        public static void WriteSecurityScheme(IParseNodeWriter writer, OpenApiSecurityScheme securityScheme)
        {
            writer.WriteStartMap();
            if (securityScheme.Type == "http")
            {
                if (securityScheme.Scheme == "basic")
                {
                    writer.WriteStringProperty("type", "basic");
                }
            }
            else
            {
                writer.WriteStringProperty("type", securityScheme.Type);
            }
            switch (securityScheme.Type)
            {
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

    internal class BodyParameter : OpenApiParameter
    {

    }
}
