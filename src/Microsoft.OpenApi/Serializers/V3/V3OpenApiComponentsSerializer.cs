using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Serializers.V3
{
    public class V3OpenApiComponentsSerializer : IOpenApiElementSerializer<OpenApiComponents>
    {
        private readonly IOpenApiReferenceElementSerializer<OpenApiSchema> _schemaReferenceElementSerializer;

        private readonly IOpenApiReferenceElementSerializer<OpenApiResponse> _responseReferenceElementSerializer;

        private readonly IOpenApiReferenceElementSerializer<OpenApiParameter> _parameterReferenceElementSerializer;

        private readonly IOpenApiReferenceElementSerializer<OpenApiExample> _exampleReferenceElementSerializer;

        private readonly IOpenApiReferenceElementSerializer<OpenApiRequestBody> _requestBodyReferenceElementSerializer;

        private readonly IOpenApiReferenceElementSerializer<OpenApiHeader> _headerReferenceElementSerializer;

        private readonly IOpenApiReferenceElementSerializer<OpenApiSecurityScheme> _securitySchemeReferenceElementSerializer;

        private readonly IOpenApiReferenceElementSerializer<OpenApiLink> _linkReferenceElementSerializer;

        private readonly IOpenApiReferenceElementSerializer<OpenApiCallback> _callbackReferenceElementSerializer;

        public V3OpenApiComponentsSerializer(
            IOpenApiReferenceElementSerializer<OpenApiSchema> schemaReferenceElementSerializer,
            IOpenApiReferenceElementSerializer<OpenApiResponse> responseReferenceElementSerializer,
            IOpenApiReferenceElementSerializer<OpenApiParameter> parameterReferenceElementSerializer,
            IOpenApiReferenceElementSerializer<OpenApiExample> exampleReferenceElementSerializer,
            IOpenApiReferenceElementSerializer<OpenApiRequestBody> requestBodyReferenceElementSerializer,
            IOpenApiReferenceElementSerializer<OpenApiHeader> headerReferenceElementSerializer,
            IOpenApiReferenceElementSerializer<OpenApiSecurityScheme> securitySchemeReferenceElementSerializer,
            IOpenApiReferenceElementSerializer<OpenApiLink> linkReferenceElementSerializer,
            IOpenApiReferenceElementSerializer<OpenApiCallback> callbackReferenceElementSerializer)
        {
            _schemaReferenceElementSerializer = schemaReferenceElementSerializer;
            _responseReferenceElementSerializer = responseReferenceElementSerializer;
            _parameterReferenceElementSerializer = parameterReferenceElementSerializer;
            _exampleReferenceElementSerializer = exampleReferenceElementSerializer;
            _requestBodyReferenceElementSerializer = requestBodyReferenceElementSerializer;
            _headerReferenceElementSerializer = headerReferenceElementSerializer;
            _securitySchemeReferenceElementSerializer = securitySchemeReferenceElementSerializer;
            _linkReferenceElementSerializer = linkReferenceElementSerializer;
            _callbackReferenceElementSerializer = callbackReferenceElementSerializer;
        }

        public void Serialize(OpenApiComponents element, IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            // If references have been inlined we don't need the to render the components section
            // however if they have cycles, then we will need a component rendered
            if (writer.GetSettings().ReferenceInline != ReferenceInlineSetting.DoNotInlineReferences)
            {
                var loops = writer.GetSettings().LoopDetector.Loops;
                writer.WriteStartObject();
                if (loops.TryGetValue(typeof(OpenApiSchema), out List<object> schemas))
                {
                    var openApiSchemas = schemas.Cast<OpenApiSchema>().Distinct().ToList()
                        .ToDictionary<OpenApiSchema, string>(k => k.Reference.Id);

                    writer.WriteOptionalMap(
                       OpenApiConstants.Schemas,
                       element.Schemas,
                       (w, key, component) => {
                           _schemaReferenceElementSerializer.SerializeWithoutReference(component, w);
                       });
                }
                writer.WriteEndObject();
                return;
            }

            writer.WriteStartObject();

            // Serialize each referenceable object as full object without reference if the reference in the object points to itself.
            // If the reference exists but points to other objects, the object is serialized to just that reference.

            // schemas
            writer.WriteOptionalMap(
                OpenApiConstants.Schemas,
                element.Schemas,
                (w, key, component) =>
                {
                    if (component.Reference != null &&
                        component.Reference.Type == ReferenceType.Schema &&
                        component.Reference.Id == key)
                    {
                        _schemaReferenceElementSerializer.SerializeWithoutReference(component, w);
                    }
                    else
                    {
                        _schemaReferenceElementSerializer.Serialize(component, w);
                    }
                });

            // responses
            writer.WriteOptionalMap(
                OpenApiConstants.Responses,
                element.Responses,
                (w, key, component) =>
                {
                    if (component.Reference != null &&
                        component.Reference.Type == ReferenceType.Response &&
                        component.Reference.Id == key)
                    {
                        _responseReferenceElementSerializer.SerializeWithoutReference(component, w);
                    }
                    else
                    {
                        _responseReferenceElementSerializer.Serialize(component, w);
                    }
                });

            // parameters
            writer.WriteOptionalMap(
                OpenApiConstants.Parameters,
                element.Parameters,
                (w, key, component) =>
                {
                    if (component.Reference != null &&
                        component.Reference.Type == ReferenceType.Parameter &&
                        component.Reference.Id == key)
                    {
                        _parameterReferenceElementSerializer.SerializeWithoutReference(component, w);
                    }
                    else
                    {
                        _parameterReferenceElementSerializer.Serialize(component, w);
                    }
                });

            // examples
            writer.WriteOptionalMap(
                OpenApiConstants.Examples,
                element.Examples,
                (w, key, component) =>
                {
                    if (component.Reference != null &&
                        component.Reference.Type == ReferenceType.Example &&
                        component.Reference.Id == key)
                    {
                        _exampleReferenceElementSerializer.SerializeWithoutReference(component, w);
                    }
                    else
                    {
                        _exampleReferenceElementSerializer.Serialize(component, w);
                    }
                });

            // requestBodies
            writer.WriteOptionalMap(
                OpenApiConstants.RequestBodies,
                element.RequestBodies,
                (w, key, component) =>
                {
                    if (component.Reference != null &&
                        component.Reference.Type == ReferenceType.RequestBody &&
                        component.Reference.Id == key)
                    {
                        _requestBodyReferenceElementSerializer.SerializeWithoutReference(component, w);
                    }
                    else
                    {
                        _requestBodyReferenceElementSerializer.Serialize(component, w);
                    }
                });

            // headers
            writer.WriteOptionalMap(
                OpenApiConstants.Headers,
                element.Headers,
                (w, key, component) =>
                {
                    if (component.Reference != null &&
                        component.Reference.Type == ReferenceType.Header &&
                        component.Reference.Id == key)
                    {
                        _headerReferenceElementSerializer.SerializeWithoutReference(component, w);
                    }
                    else
                    {
                        _headerReferenceElementSerializer.Serialize(component, w);
                    }
                });

            // securitySchemes
            writer.WriteOptionalMap(
                OpenApiConstants.SecuritySchemes,
                element.SecuritySchemes,
                (w, key, component) =>
                {
                    if (component.Reference != null &&
                        component.Reference.Type == ReferenceType.SecurityScheme &&
                        component.Reference.Id == key)
                    {
                        _securitySchemeReferenceElementSerializer.SerializeWithoutReference(component, w);
                    }
                    else
                    {
                        _securitySchemeReferenceElementSerializer.Serialize(component, w);
                    }
                });

            // links
            writer.WriteOptionalMap(
                OpenApiConstants.Links,
                element.Links,
                (w, key, component) =>
                {
                    if (component.Reference != null &&
                        component.Reference.Type == ReferenceType.Link &&
                        component.Reference.Id == key)
                    {
                        _linkReferenceElementSerializer.SerializeWithoutReference(component, w);
                    }
                    else
                    {
                        _linkReferenceElementSerializer.Serialize(component, w);
                    }
                });

            // callbacks
            writer.WriteOptionalMap(
                OpenApiConstants.Callbacks,
                element.Callbacks,
                (w, key, component) =>
                {
                    if (component.Reference != null &&
                        component.Reference.Type == ReferenceType.Callback &&
                        component.Reference.Id == key)
                    {
                        _callbackReferenceElementSerializer.SerializeWithoutReference(component, w);
                    }
                    else
                    {
                        _callbackReferenceElementSerializer.Serialize(component, w);
                    }
                });

            // extensions
            writer.WriteExtensions(element.Extensions, OpenApiSpecVersion.OpenApi3_0);

            writer.WriteEndObject();
        }
    }
}
