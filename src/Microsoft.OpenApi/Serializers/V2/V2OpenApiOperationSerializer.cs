using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Serializers.V2
{
    public class V2OpenApiOperationSerializer : IOpenApiElementSerializer<OpenApiOperation>
    {
        private readonly IOpenApiElementSerializer<OpenApiExternalDocs> _externalDocsSerializer;

        private readonly IOpenApiElementSerializer<OpenApiParameter> _parameterSerializer;

        private readonly IOpenApiElementSerializer<OpenApiResponses> _responsesSerializer;

        private readonly IOpenApiElementSerializer<OpenApiSecurityRequirement> _securityRequirementSerializer;

        private readonly IOpenApiElementSerializer<OpenApiTag> _tagSerializer;
        public V2OpenApiOperationSerializer(
            IOpenApiElementSerializer<OpenApiExternalDocs> externalDocsSerializer,
            IOpenApiElementSerializer<OpenApiParameter> parameterSerializer,
            IOpenApiElementSerializer<OpenApiResponses> responsesSerializer,
            IOpenApiElementSerializer<OpenApiSecurityRequirement> securityRequirementSerializer,
            IOpenApiElementSerializer<OpenApiTag> tagSerializer)
        {
            _externalDocsSerializer = externalDocsSerializer;
            _parameterSerializer = parameterSerializer;
            _responsesSerializer = responsesSerializer;
            _securityRequirementSerializer = securityRequirementSerializer;
            _tagSerializer = tagSerializer;
        }
        public void Serialize(OpenApiOperation element, IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            writer.WriteStartObject();

            // tags
            writer.WriteOptionalCollection(
                OpenApiConstants.Tags,
                element.Tags,
                (w, t) =>
                {
                    _tagSerializer.Serialize(t, w);
                });

            // summary
            writer.WriteProperty(OpenApiConstants.Summary, element.Summary);

            // description
            writer.WriteProperty(OpenApiConstants.Description, element.Description);

            // externalDocs
            writer.WriteOptionalObject(OpenApiConstants.ExternalDocs, element.ExternalDocs, (w, e) => _externalDocsSerializer.Serialize(e, w));

            // operationId
            writer.WriteProperty(OpenApiConstants.OperationId, element.OperationId);

            IList<OpenApiParameter> parameters;
            if (element.Parameters == null)
            {
                parameters = new List<OpenApiParameter>();
            }
            else
            {
                parameters = new List<OpenApiParameter>(element.Parameters);
            }

            if (element.RequestBody != null)
            {
                // consumes
                writer.WritePropertyName(OpenApiConstants.Consumes);
                writer.WriteStartArray();
                var consumes = element.RequestBody.Content.Keys.Distinct().ToList();
                foreach (var mediaType in consumes)
                {
                    writer.WriteValue(mediaType);
                }

                writer.WriteEndArray();

                // This is form data. We need to split the request body into multiple parameters.
                if (consumes.Contains("application/x-www-form-urlencoded") ||
                    consumes.Contains("multipart/form-data"))
                {
                    foreach (var property in element.RequestBody.Content.First().Value.Schema.Properties)
                    {
                        var paramName = property.Key;
                        var paramSchema = property.Value;
                        if (paramSchema.Type == "string" && paramSchema.Format == "binary")
                        {
                            paramSchema.Type = "file";
                            paramSchema.Format = null;
                        }
                        parameters.Add(
                            new OpenApiFormDataParameter
                            {
                                Description = property.Value.Description,
                                Name = property.Key,
                                Schema = property.Value,
                                Required = element.RequestBody.Content.First().Value.Schema.Required.Contains(property.Key)

                            });
                    }
                }
                else
                {
                    var content = element.RequestBody.Content.Values.FirstOrDefault();

                    var bodyParameter = new OpenApiBodyParameter
                    {
                        Description = element.RequestBody.Description,
                        // V2 spec actually allows the body to have custom name.
                        // To allow round-tripping we use an extension to hold the name
                        Name = "body",
                        Schema = content?.Schema ?? new OpenApiSchema(),
                        Required = element.RequestBody.Required,
                        Extensions = element.RequestBody.Extensions.ToDictionary(k => k.Key, v => v.Value)  // Clone extensions so we can remove the x-bodyName extensions from the output V2 model.
                    };

                    if (bodyParameter.Extensions.ContainsKey(OpenApiConstants.BodyName))
                    {
                        bodyParameter.Name = (element.RequestBody.Extensions[OpenApiConstants.BodyName] as OpenApiString)?.Value ?? "body";
                        bodyParameter.Extensions.Remove(OpenApiConstants.BodyName);
                    }

                    parameters.Add(bodyParameter);
                }
            }

            if (element.Responses != null)
            {
                var produces = element.Responses.Where(r => r.Value.Content != null)
                    .SelectMany(r => r.Value.Content?.Keys)
                    .Distinct()
                    .ToList();

                if (produces.Any())
                {
                    // produces
                    writer.WritePropertyName(OpenApiConstants.Produces);
                    writer.WriteStartArray();
                    foreach (var mediaType in produces)
                    {
                        writer.WriteValue(mediaType);
                    }

                    writer.WriteEndArray();
                }
            }

            // parameters
            // Use the parameters created locally to include request body if exists.
            writer.WriteOptionalCollection(OpenApiConstants.Parameters, parameters, (w, p) => _parameterSerializer.Serialize(p, w));

            // responses
            writer.WriteRequiredObject(OpenApiConstants.Responses, element.Responses, (w, r) => _responsesSerializer.Serialize(r, w));

            // schemes
            // All schemes in the Servers are extracted, regardless of whether the host matches
            // the host defined in the outermost Swagger object. This is due to the 
            // inaccessibility of information for that host in the context of an inner object like this Operation.
            if (element.Servers != null)
            {
                var schemes = element.Servers.Select(
                        s =>
                        {
                            Uri.TryCreate(s.Url, UriKind.RelativeOrAbsolute, out var url);
                            return url?.Scheme;
                        })
                    .Where(s => s != null)
                    .Distinct()
                    .ToList();

                writer.WriteOptionalCollection(OpenApiConstants.Schemes, schemes, (w, s) => w.WriteValue(s));
            }

            // deprecated
            writer.WriteProperty(OpenApiConstants.Deprecated, element.Deprecated, false);

            // security
            writer.WriteOptionalCollection(OpenApiConstants.Security, element.Security, (w, s) => _securityRequirementSerializer.Serialize(s, w));

            // specification extensions
            writer.WriteExtensions(element.Extensions, OpenApiSpecVersion.OpenApi2_0);

            writer.WriteEndObject();
        }
    }
}
