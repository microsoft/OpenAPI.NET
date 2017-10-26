// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Writers
{
    /// <summary>
    /// Class to serialize Open API v3.0 document.
    /// </summary>
    internal static class OpenApiV3Serializer
    {
        /// <summary>
        /// Write <see cref="OpenApiDocument"/>.
        /// </summary>
        public static void Write(this IOpenApiWriter writer, OpenApiDocument document)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();
            if (document != null)
            {
                // openapi:3.0.0
                writer.WriteRequiredProperty(OpenApiConstants.OpenApiDocOpenApi, document.Version?.ToString());

                // info
                writer.WriteRequiredObject(OpenApiConstants.OpenApiDocInfo, document.Info, Write);

                // servers
                writer.WriteOptionalCollection(OpenApiConstants.OpenApiDocServers, document.Servers, Write);

                // paths
                writer.WriteRequiredObject(OpenApiConstants.OpenApiDocPaths, document.Paths, Write);

                // components
                writer.WriteOptionalObject(OpenApiConstants.OpenApiDocComponents, document.Components, Write);

                // security
                writer.WriteOptionalCollection(OpenApiConstants.OpenApiDocSecurity, document.Security, Write);

                // tags
                writer.WriteOptionalCollection(OpenApiConstants.OpenApiDocTags, document.Tags, Write);

                // external docs
                writer.WriteOptionalObject(OpenApiConstants.OpenApiDocExternalDocs, document.ExternalDocs, Write);

                // Specification Extensions.
                writer.WriteExtensions(document.Extensions);
            }
            writer.WriteEndObject();
        }

        /// <summary>
        /// Write <see cref="OpenApiInfo"/>
        /// </summary>
        public static void Write(this IOpenApiWriter writer, OpenApiInfo info)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();
            if (info != null)
            {
                // title
                writer.WriteRequiredProperty(OpenApiConstants.OpenApiDocTitle, info.Title);

                // description
                writer.WriteOptionalProperty(OpenApiConstants.OpenApiDocDescription, info.Description);

                // termsOfService
                writer.WriteOptionalProperty(OpenApiConstants.OpenApiDocTermsOfService, info.TermsOfService?.OriginalString);

                // contact object
                writer.WriteOptionalObject(OpenApiConstants.OpenApiDocContact, info.Contact, Write);

                // license object
                writer.WriteOptionalObject(OpenApiConstants.OpenApiDocLicense, info.License, Write);

                // version
                writer.WriteRequiredProperty(OpenApiConstants.OpenApiDocVersion, info.Version?.ToString());

                // Specification Extensions.
                writer.WriteExtensions(info.Extensions);
            }
            writer.WriteEndObject();
        }

        /// <summary>
        /// Write <see cref="OpenApiPaths"/>
        /// </summary>
        public static void Write(this IOpenApiWriter writer, OpenApiPaths paths)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();
            if (paths != null)
            {
                foreach (var pathItem in paths)
                {
                    writer.WritePropertyName(pathItem.Key);
                    writer.Write(pathItem.Value);
                }
            }
            writer.WriteEndObject();
        }

        /// <summary>
        /// Write <see cref="OpenApiPathItem"/>
        /// </summary>
        public static void Write(this IOpenApiWriter writer, OpenApiPathItem pathItem)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }
            writer.WriteStartObject();
            if (pathItem != null)
            {
                /*
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
                }*/
            }
            writer.WriteEndObject();
        }

        /// <summary>
        /// Write <see cref="OpenApiComponents"/>
        /// </summary>
        public static void Write(this IOpenApiWriter writer, OpenApiComponents components)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();
            if (components != null)
            {
                // schemas
                writer.WriteOptionalMap(OpenApiConstants.OpenApiDocSchemas, components.Schemas, Write);

                // responses
                writer.WriteOptionalMap(OpenApiConstants.OpenApiDocResponses, components.Responses, Write);

                // parameters
                writer.WriteOptionalMap(OpenApiConstants.OpenApiDocParameters, components.Parameters, Write);

                // examples
                writer.WriteOptionalMap(OpenApiConstants.OpenApiDocExamples, components.Examples, Write);

                // requestBodies
                writer.WriteOptionalMap(OpenApiConstants.OpenApiDocRequestBodies, components.RequestBodies, Write);

                // headers
                writer.WriteOptionalMap(OpenApiConstants.OpenApiDocHeaders, components.Headers, Write);

                // securitySchemes
                writer.WriteOptionalMap(OpenApiConstants.OpenApiDocSecuritySchemes, components.SecuritySchemes, Write);

                // links
                writer.WriteOptionalMap(OpenApiConstants.OpenApiDocLinks, components.Links, Write);

                // callbacks
                writer.WriteOptionalMap(OpenApiConstants.OpenApiDocCallbacks, components.Callbacks, Write);

                // Specification Extensions.
                writer.WriteExtensions(components.Extensions);
            }
            writer.WriteEndObject();
        }

        /// <summary>
        /// Write <see cref="OpenApiSecurityRequirement"/>
        /// </summary>
        public static void Write(this IOpenApiWriter writer, OpenApiSecurityRequirement security)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();
            if (security != null)
            {
            }
            writer.WriteEndObject();
        }

        /// <summary>
        /// Write <see cref="OpenApiTag"/>.
        /// </summary>
        public static void Write(this IOpenApiWriter writer, OpenApiTag tag)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull("writer");
            }

            writer.WriteStartObject();
            if (tag != null)
            {
                // name
                writer.WriteRequiredProperty(OpenApiConstants.OpenApiDocName, tag.Name);

                // description
                writer.WriteOptionalProperty(OpenApiConstants.OpenApiDocDescription, tag.Description);

                // External Docs
                writer.WriteOptionalObject(OpenApiConstants.OpenApiDocExternalDocs, tag.ExternalDocs, Write);

                // Specification Extensions.
                writer.WriteExtensions(tag.Extensions);
            }
            writer.WriteEndObject();
        }

        /// <summary>
        /// Write <see cref="OpenApiExternalDocs"/>.
        /// </summary>
        public static void Write(this IOpenApiWriter writer, OpenApiExternalDocs externalDocs)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull("writer");
            }

            writer.WriteStartObject();
            if (externalDocs != null)
            {
                // description
                writer.WriteOptionalProperty(OpenApiConstants.OpenApiDocDescription, externalDocs.Description);

                // url
                writer.WriteRequiredProperty(OpenApiConstants.OpenApiDocUrl, externalDocs.Url?.OriginalString);

                // Specification Extensions.
                writer.WriteExtensions(externalDocs.Extensions);
            }
            writer.WriteEndObject();
        }

        /// <summary>
        /// Write <see cref="OpenApiServer"/>.
        /// </summary>
        public static void Write(this IOpenApiWriter writer, OpenApiServer server)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull("writer");
            }

            writer.WriteStartObject();
            if (server != null)
            {
                // Url
                writer.WriteRequiredProperty(OpenApiConstants.OpenApiDocUrl, server.Url?.OriginalString);

                // description
                writer.WriteOptionalProperty(OpenApiConstants.OpenApiDocDescription, server.Description);

                // variables
                writer.WriteOptionalMap(OpenApiConstants.OpenApiDocVariables, server.Variables, Write);

                // Specification Extensions.
                writer.WriteExtensions(server.Extensions);
            }

            writer.WriteEndObject();
        }

        /// <summary>
        /// Write <see cref="OpenApiServerVariable"/>.
        /// </summary>
        public static void Write(this IOpenApiWriter writer, OpenApiServerVariable serverVariable)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull("writer");
            }

            writer.WriteStartObject();
            if (serverVariable != null)
            {
                // default
                writer.WriteRequiredProperty(OpenApiConstants.OpenApiDocDefault, serverVariable.Default);

                // description
                writer.WriteOptionalProperty(OpenApiConstants.OpenApiDocDescription, serverVariable.Description);

                // enums
                writer.WriteOptionalCollection(OpenApiConstants.OpenApiDocEnum, serverVariable.Enum, (w, t) => w.WriteValue(t));

                // Specification Extensions.
                writer.WriteExtensions(serverVariable.Extensions);
            }
            writer.WriteEndObject();
        }

        /// <summary>
        /// Write <see cref="OpenApiContact"/>.
        /// </summary>
        public static void Write(this IOpenApiWriter writer, OpenApiContact contact)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull("writer");
            }

            writer.WriteStartObject();
            if (contact != null)
            {
                // name
                writer.WriteOptionalProperty(OpenApiConstants.OpenApiDocName, contact.Name);

                // url
                writer.WriteOptionalProperty(OpenApiConstants.OpenApiDocUrl, contact.Url?.OriginalString);

                // email
                writer.WriteOptionalProperty(OpenApiConstants.OpenApiDocEmail, contact.Email);

                // Specification Extensions.
                writer.WriteExtensions(contact.Extensions);
            }
            writer.WriteEndObject();
        }

        /// <summary>
        /// Write <see cref="OpenApiLicense"/>.
        /// </summary>
        public static void Write(this IOpenApiWriter writer, OpenApiLicense license)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull("writer");
            }

            writer.WriteStartObject();
            if (license != null)
            {
                // name
                writer.WriteRequiredProperty(OpenApiConstants.OpenApiDocName, license.Name);

                // url
                writer.WriteOptionalProperty(OpenApiConstants.OpenApiDocUrl, license.Url?.OriginalString);

                // Specification Extensions.
                writer.WriteExtensions(license.Extensions);
            }
            writer.WriteEndObject();
        }

        /// <summary>
        /// Write <see cref="OpenApiSecurityScheme"/>.
        /// </summary>
        public static void Write(this IOpenApiWriter writer, OpenApiSecurityScheme securityScheme)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull("writer");
            }

            writer.WriteStartObject();
            if (securityScheme != null)
            {
                // type
                writer.WriteRequiredProperty(OpenApiConstants.OpenApiDocType, securityScheme.Type.ToString());

                // Description
                writer.WriteOptionalProperty(OpenApiConstants.OpenApiDocDescription, securityScheme.Description);

                // name
                switch (securityScheme.Type)
                {
                    case SecuritySchemeTypeKind.http:
                        // scheme
                        writer.WriteRequiredProperty(OpenApiConstants.OpenApiDocScheme, securityScheme.Scheme);

                        // bearerFormat
                        writer.WriteOptionalProperty(OpenApiConstants.OpenApiDocBearerFormat, securityScheme.BearerFormat);
                        break;

                    case SecuritySchemeTypeKind.oauth2:
                        // flows
                        writer.WriteRequiredObject(OpenApiConstants.OpenApiDocFlows, securityScheme.Flows, Write);
                        break;

                    case SecuritySchemeTypeKind.apiKey:
                        // name
                        writer.WriteRequiredProperty(OpenApiConstants.OpenApiDocName, securityScheme.Name);

                        // in
                        writer.WriteRequiredProperty(OpenApiConstants.OpenApiDocIn, securityScheme.In.ToString());

                        break;
                }

                // openIdConnectUrl
                writer.WriteRequiredProperty(OpenApiConstants.OpenApiDocOpenIdConnectUrl, securityScheme.OpenIdConnectUrl?.ToString());

                // Specification Extensions.
                writer.WriteExtensions(securityScheme.Extensions);
            }
            writer.WriteEndObject();
        }

        /// <summary>
        /// Write <see cref="OpenApiLink"/>.
        /// </summary>
        public static void Write(this IOpenApiWriter writer, OpenApiLink link)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull("writer");
            }

            writer.WriteStartObject();
            if (link != null)
            {
                if (link.IsReference())
                {
                    link.WriteRef(writer);
                }
                else
                {
                    // operationRef
                    writer.WriteOptionalProperty(OpenApiConstants.OpenApiDocOperationRef, link.OperationRef);

                    // operationId
                    writer.WriteOptionalProperty(OpenApiConstants.OpenApiDocOperationId, link.OperationId);

                    // Parameters
                    // writer.WriteMap("parameters", link.Parameters, (w, x) => { w.WriteValue(x.ToString()); });
                }
            }
            writer.WriteEndObject();
        }

        /// <summary>
        /// Write <see cref="OpenApiHeader"/>.
        /// </summary>
        public static void Write(this IOpenApiWriter writer, OpenApiHeader header)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull("writer");
            }

            writer.WriteStartObject();
            if (header != null)
            {
                if (header.IsReference())
                {
                    header.WriteRef(writer);
                }
                else
                {
                    // description
                    writer.WriteOptionalProperty(OpenApiConstants.OpenApiDocDescription, header.Description);

                    // required
                    writer.WriteOptionalProperty(OpenApiConstants.OpenApiDocRequired, header.Required, false);

                    // deprecated
                    writer.WriteOptionalProperty(OpenApiConstants.OpenApiDocDeprecated, header.Deprecated, false);

                    // allowEmptyValue
                    writer.WriteOptionalProperty(OpenApiConstants.OpenApiDocAllowEmptyValue, header.AllowEmptyValue, false);

                    // style
                    writer.WriteOptionalProperty(OpenApiConstants.OpenApiDocStyle, header.Style);

                    // explode
                    writer.WriteOptionalProperty(OpenApiConstants.OpenApiDocExplode, header.Explode, false);

                    // allowReserved
                    writer.WriteOptionalProperty(OpenApiConstants.OpenApiDocAllowReserved, header.AllowReserved, false);

                    // schema
                    writer.WriteOptionalObject(OpenApiConstants.OpenApiDocSchema, header.Schema, Write);

                    // Examples
                    writer.WriteOptionalCollection(OpenApiConstants.OpenApiDocExamples, header.Examples, Write);

                    // example
                    writer.WriteOptionalObject(OpenApiConstants.OpenApiDocExample, header.Example, (w, a) => w.WriteAny(a));

                    // content
                    writer.WriteOptionalMap(OpenApiConstants.OpenApiDocContent, header.Content, Write);

                    // Specification Extensions.
                    writer.WriteExtensions(header.Extensions);
                }
            }
            writer.WriteEndObject();
        }

        /// <summary>
        /// Write <see cref="OpenApiResponse"/>.
        /// </summary>
        public static void Write(this IOpenApiWriter writer, OpenApiResponse response)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull("writer");
            }

            writer.WriteStartObject();
            if (response != null)
            {
                if (response.IsReference())
                {
                    response.WriteRef(writer);
                }
                else
                {/*
                    writer.WriteStringProperty("description", response.Description);
                    writer.WriteMap("content", response.Content, WriteMediaType);

                    writer.WriteMap("headers", response.Headers, WriteHeaderOrReference);
                    writer.WriteMap("links", response.Links, WriteLinkOrReference);*/
                }
            }

            writer.WriteEndObject();
        }

        /// <summary>
        /// Write <see cref="OpenApiRequestBody"/>.
        /// </summary>
        public static void Write(this IOpenApiWriter writer, OpenApiRequestBody requestBody)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull("writer");
            }

            writer.WriteStartObject();

            if (requestBody != null)
            {
                if (requestBody.IsReference())
                {
                    requestBody.WriteRef(writer);
                }
                else
                {
                    
                }
            }
            writer.WriteEndObject();
        }

        /// <summary>
        /// Write <see cref="OpenApiParameter"/>.
        /// </summary>
        public static void Write(this IOpenApiWriter writer, OpenApiParameter parameter)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull("writer");
            }

            writer.WriteStartObject();
            if (parameter != null)
            {
                if (parameter.IsReference())
                {
                    parameter.WriteRef(writer);
                }
                else
                {/*
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
                    writer.WriteMap("content", parameter.Content, WriteMediaType);*/
                }
            }
            writer.WriteEndObject();
        }

        public static void Write(this IOpenApiWriter writer, OpenApiOperation operation)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull("writer");
            }

            writer.WriteStartObject();
            if (operation != null)
            {/*
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
                writer.WriteList("servers", operation.Servers, WriteServer);*/
            }
            writer.WriteEndObject();
        }

        /// <summary>
        /// Write <see cref="OpenApiSchema"/>.
        /// </summary>
        public static void Write(IOpenApiWriter writer, OpenApiSchema schema)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull("writer");
            }

            if (schema == null)
            {
                writer.WriteStartObject();
                writer.WriteEndObject();
                return;
            }

            if (schema.IsReference())
            {
                writer.WriteStartObject();
                schema.WriteRef(writer);
                writer.WriteEndObject();
                return;
            }

            writer.WriteStartObject();

            // type
            writer.WriteOptionalProperty(OpenApiConstants.OpenApiDocType, schema.Type);

            // title
            writer.WriteOptionalProperty(OpenApiConstants.OpenApiDocTitle, schema.Title);

            // Format
            writer.WriteOptionalProperty(OpenApiConstants.OpenApiDocFormat, schema.Format);

            // Description
            writer.WriteOptionalProperty(OpenApiConstants.OpenApiDocDescription, schema.Description);

            // MaxLength
            writer.WriteOptionalProperty(OpenApiConstants.OpenApiDocMaxLength, schema.MaxLength);

            // MaxLength
            writer.WriteOptionalProperty(OpenApiConstants.OpenApiDocMinLength, schema.MinLength);

            // Pattern
            writer.WriteOptionalProperty(OpenApiConstants.OpenApiDocPattern, schema.Pattern);

            // Default
            writer.WriteOptionalProperty(OpenApiConstants.OpenApiDocDefault, schema.Default);

            // Required
            writer.WriteOptionalCollection(OpenApiConstants.OpenApiDocRequired, schema.Required, (w, s) => w.WriteValue(s));

            // Maximum
            writer.WriteOptionalProperty(OpenApiConstants.OpenApiDocMaximum, schema.Maximum);

            // exclusiveMaximum
            writer.WriteOptionalProperty(OpenApiConstants.OpenApiDocExclusiveMaximum, schema.ExclusiveMaximum, false);

            // Minimum
            writer.WriteOptionalProperty(OpenApiConstants.OpenApiDocMinimum, schema.Minimum);

            // exclusiveMinimum
            writer.WriteOptionalProperty(OpenApiConstants.OpenApiDocExclusiveMinimum, schema.ExclusiveMinimum, false);

            // AdditionalProperties
            writer.WriteOptionalObject(OpenApiConstants.OpenApiDocAdditionalProperties, schema.AdditionalProperties, Write);

            // Items
            writer.WriteOptionalObject(OpenApiConstants.OpenApiDocItems, schema.Items, Write);

            // MaxItems
            writer.WriteOptionalProperty(OpenApiConstants.OpenApiDocMaxItems, schema.MaxItems);

            // MinItems
            writer.WriteOptionalProperty(OpenApiConstants.OpenApiDocMinItems, schema.MinItems);

            // Properties
            writer.WriteOptionalMap(OpenApiConstants.OpenApiDocProperties, schema.Properties, Write);


            // MaxProperties
            writer.WriteOptionalProperty(OpenApiConstants.OpenApiDocMaxProperties, schema.MaxProperties);

            // MinProperties
            writer.WriteOptionalProperty(OpenApiConstants.OpenApiDocMinProperties, schema.MinProperties);

            // enum
            writer.WriteOptionalCollection(OpenApiConstants.OpenApiDocEnum, schema.Enum, (w, s) => w.WriteValue(s));

            // Specification Extensions.
            writer.WriteExtensions(schema.Extensions);

            writer.WriteEndObject();
        }

        /// <summary>
        /// Write <see cref="OpenApiMediaType"/>.
        /// </summary>
        public static void Write(IOpenApiWriter writer, OpenApiMediaType mediaType)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull("writer");
            }

            writer.WriteStartObject();

            if (mediaType != null)
            {
                // schema
                writer.WriteOptionalObject(OpenApiConstants.OpenApiDocSchema, mediaType.Schema, Write);

                // example
                writer.WriteOptionalObject(OpenApiConstants.OpenApiDocExample, mediaType.Example, (w,a) => w.WriteAny(a));

                // examples
                writer.WriteOptionalMap(OpenApiConstants.OpenApiDocExamples, mediaType.Examples, Write);

                // encoding
                writer.WriteOptionalMap(OpenApiConstants.OpenApiDocEncoding, mediaType.Encoding, Write);
            }

            writer.WriteEndObject();
        }

        /// <summary>
        /// Write <see cref="OpenApiEncoding"/>.
        /// </summary>
        public static void Write(IOpenApiWriter writer, OpenApiEncoding encoding)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull("writer");
            }

            writer.WriteStartObject();
            if (encoding != null)
            {

            }
            writer.WriteEndObject();
        }

        /// <summary>
        /// Write <see cref="OpenApiCallback"/>.
        /// </summary>
        public static void Write(IOpenApiWriter writer, OpenApiCallback callback)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull("writer");
            }

            writer.WriteStartObject();
            if (callback != null)
            {
                if(callback.IsReference())
                {
                    callback.WriteRef(writer);
                }
                else
                {

                }
            }
            writer.WriteEndObject();
        }

        /// <summary>
        /// Write <see cref="OpenApiExample"/>.
        /// </summary>
        public static void Write(this IOpenApiWriter writer, OpenApiExample example)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull("writer");
            }
            writer.WriteStartObject();

            if (example != null)
            {
                if (example.IsReference())
                {
                    example.WriteRef(writer);
                }
                else
                {
                    // summary
                    writer.WriteOptionalProperty(OpenApiConstants.OpenApiDocSummary, example.Summary);

                    // description
                    writer.WriteOptionalProperty(OpenApiConstants.OpenApiDocDescription, example.Description);

                    // value
                    writer.WriteOptionalObject(OpenApiConstants.OpenApiDocValue, example.Value, (w, a) => w.WriteAny(a));

                    // externalValue
                    writer.WriteOptionalProperty(OpenApiConstants.OpenApiDocExternalValue, example.ExternalValue?.OriginalString);

                    // Specification Extensions.
                    writer.WriteExtensions(example.Extensions);
                }
            }
            writer.WriteEndObject();
        }

        /// <summary>
        /// Write <see cref="OpenApiOAuthFlows"/>.
        /// </summary>
        public static void Write(this IOpenApiWriter writer, OpenApiOAuthFlows oAuthFlows)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull("writer");
            }

            writer.WriteStartObject();
            if (oAuthFlows != null)
            {
                // implicit
                writer.WriteOptionalObject(OpenApiConstants.OpenApiDocImplicit, oAuthFlows.Implicit, Write);

                // password
                writer.WriteOptionalObject(OpenApiConstants.OpenApiDocPassword, oAuthFlows.Password, Write);

                // clientCredentials
                writer.WriteOptionalObject(OpenApiConstants.OpenApiDocClientCredentials, oAuthFlows.ClientCredentials, Write);

                // authorizationCode
                writer.WriteOptionalObject(OpenApiConstants.OpenApiDocAuthorizationCode, oAuthFlows.AuthorizationCode, Write);

                // Specification Extensions.
                writer.WriteExtensions(oAuthFlows.Extensions);
            }
            writer.WriteEndObject();
        }

        /// <summary>
        /// Write <see cref="OpenApiOAuthFlow"/>.
        /// </summary>
        public static void Write(IOpenApiWriter writer, OpenApiOAuthFlow oAuthFlow)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull("writer");
            }

            writer.WriteStartObject();
            if (oAuthFlow != null)
            {
                // authorizationUrl
                writer.WriteRequiredProperty(OpenApiConstants.OpenApiDocAuthorizationUrl, oAuthFlow.AuthorizationUrl?.ToString());

                // tokenUrl
                writer.WriteRequiredProperty(OpenApiConstants.OpenApiDocTokenUrl, oAuthFlow.TokenUrl?.ToString());

                // refreshUrl
                writer.WriteOptionalProperty(OpenApiConstants.OpenApiDocRefreshUrl, oAuthFlow.RefreshUrl?.ToString());

                // scopes
                writer.WriteRequiredMap(OpenApiConstants.OpenApiDocScopes, oAuthFlow.Scopes, (w, s) => w.WriteValue(s));

                // Specification Extensions.
                writer.WriteExtensions(oAuthFlow.Extensions);
            }
            writer.WriteEndObject();
        }
    }
}
