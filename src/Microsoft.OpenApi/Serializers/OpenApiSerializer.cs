using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Serializers.V2;
using Microsoft.OpenApi.Serializers.V3;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Serializers
{
    public class OpenApiSerializer 
    {
        private readonly IServiceProvider _serviceProvider;

        public OpenApiSerializer(OpenApiSpecVersion specVersion)
        {
            var serviceContainer = new ServiceCollection();
            RegisterGeneralSerializers(serviceContainer);
            switch (specVersion)
            {
                case OpenApiSpecVersion.OpenApi2_0:
                    RegisterV2Serializers(serviceContainer);
                    break;
                case OpenApiSpecVersion.OpenApi3_0:
                    RegisterV3Serializers(serviceContainer);
                    break;
                default:
                    throw new NotImplementedException($"Open API Spec Version not implemented in Open API Serializer");
            }
            _serviceProvider = serviceContainer.BuildServiceProvider();
        }

        public static OpenApiSerializer V2Serializer => new OpenApiSerializer(OpenApiSpecVersion.OpenApi2_0);

        public static OpenApiSerializer V3Serializer => new OpenApiSerializer(OpenApiSpecVersion.OpenApi3_0);

        public void Serialize<TElement>(TElement element, IOpenApiWriter writer) where TElement : IOpenApiElement
        {
            var serializer = _serviceProvider.GetService<IOpenApiElementSerializer<TElement>>();
            if (serializer != null)
            {
                serializer.Serialize(element, writer);
            }
        }

        public void SerializeWithoutReference<TReferenceElement>(TReferenceElement referenceElement, IOpenApiWriter writer) 
            where TReferenceElement : IOpenApiReferenceable
        {
            var serializer = _serviceProvider.GetService<IOpenApiReferenceElementSerializer<TReferenceElement>>();
            if (serializer != null)
            {
                serializer.SerializeWithoutReference(referenceElement, writer);
            }
        }

        private void RegisterGeneralSerializers(IServiceCollection serviceCollection)
        {
            RegisterElementSerializer<OpenApiContactSerializer, OpenApiContact>(serviceCollection);
            RegisterElementSerializer<OpenApiExternalDocsSerializer, OpenApiExternalDocs>(serviceCollection);
            RegisterElementSerializer<OpenApiInfoSerializer, OpenApiInfo>(serviceCollection);
            RegisterElementSerializer<OpenApiLicenseSerializer, OpenApiLicense>(serviceCollection);
            RegisterElementSerializer<OpenApiPathsExtensibleDictionarySerializer, OpenApiPaths>(serviceCollection);
            RegisterElementSerializer<OpenApiResponsesExtensibleDictionarySerializer, OpenApiResponses>(serviceCollection);
            RegisterElementSerializer<OpenApiSecurityRequirementSerializer, OpenApiSecurityRequirement>(serviceCollection);
            RegisterReferenceElementSerializer<OpenApiTagSerializer, OpenApiTag>(serviceCollection);
            RegisterElementSerializer<OpenApiXmlSerializer, OpenApiXml>(serviceCollection);
        }

        private void RegisterV2Serializers(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IOpenApiSpecProvider>(x => OpenApiSpecProvider.V2Version);
            RegisterElementSerializer<V2OpenApiDocumentSerializer, OpenApiDocument>(serviceCollection);
            RegisterReferenceElementSerializer<V2OpenApiHeaderSerializer, OpenApiHeader>(serviceCollection);
            RegisterElementSerializer<V2OpenApiOperationSerializer, OpenApiOperation>(serviceCollection);
            RegisterReferenceElementSerializer<V2OpenApiParameterSerializer, OpenApiParameter>(serviceCollection);
            RegisterElementSerializer<V2OpenApiPathItemSerializer, OpenApiPathItem>(serviceCollection);
            RegisterElementSerializer<V2OpenApiReferenceSerializer, OpenApiReference>(serviceCollection);
            RegisterReferenceElementSerializer<V2OpenApiResponseSerializer, OpenApiResponse>(serviceCollection);
            RegisterReferenceElementSerializer<V2OpenApiSchemaSerializer, OpenApiSchema>(serviceCollection);
            RegisterReferenceElementSerializer<V2OpenApiSecuritySchemeSerializer, OpenApiSecurityScheme>(serviceCollection);
            serviceCollection.AddTransient<V2OpenApiSchemaSerializer>();
        }

        private void RegisterV3Serializers(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IOpenApiSpecProvider>(x => OpenApiSpecProvider.V3Version);
            RegisterElementSerializer<V3OpenApiAuthFlowSerializer, OpenApiOAuthFlow>(serviceCollection);
            RegisterElementSerializer<V3OpenApiAuthFlowSerializer, OpenApiOAuthFlows>(serviceCollection);
            RegisterReferenceElementSerializer<V3OpenApiCallbackSerializer, OpenApiCallback>(serviceCollection);
            RegisterElementSerializer<V3OpenApiComponentsSerializer, OpenApiComponents>(serviceCollection);
            RegisterElementSerializer<V3OpenApiDiscriminatorSerializer, OpenApiDiscriminator>(serviceCollection);
            RegisterElementSerializer<V3OpenApiDocumentSerializer, OpenApiDocument>(serviceCollection);            
            RegisterReferenceElementSerializer<V3OpenApiExampleSerializer, OpenApiExample>(serviceCollection);
            RegisterReferenceElementSerializer<V3OpenApiHeaderSerializer, OpenApiHeader>(serviceCollection);
            RegisterReferenceElementSerializer<V3OpenApiLinkSerializer, OpenApiLink>(serviceCollection);
            RegisterElementSerializer<V3OpenApiMediaTypeSerializer, OpenApiMediaType>(serviceCollection);
            RegisterReferenceElementSerializer<V3OpenApiParameterSerializer, OpenApiParameter>(serviceCollection);
            RegisterElementSerializer<V3OpenApiPathItemSerializer, OpenApiPathItem>(serviceCollection);
            RegisterElementSerializer<V3OpenApiReferenceSerializer, OpenApiReference>(serviceCollection);
            RegisterReferenceElementSerializer<V3OpenApiRequestBodySerializer, OpenApiRequestBody>(serviceCollection);
            RegisterReferenceElementSerializer<V3OpenApiResponseSerializer, OpenApiResponse>(serviceCollection);
            RegisterReferenceElementSerializer<V3OpenApiSchemaSerializer, OpenApiSchema>(serviceCollection);
            RegisterReferenceElementSerializer<V3OpenApiSecuritySchemeSerializer, OpenApiSecurityScheme>(serviceCollection);
            RegisterElementSerializer<V3OpenApiServerSerializer, OpenApiServer>(serviceCollection);
            RegisterElementSerializer<V3OpenApiServerVariableSerializer, OpenApiServerVariable>(serviceCollection);
            serviceCollection.AddTransient<V3OpenApiEncodingSerializer>();
            serviceCollection.AddTransient<V3OpenApiOperationSerializer>();
            serviceCollection.AddTransient<IOpenApiElementSerializer<OpenApiOperation>, LazyOpenApiElementSerializer<V3OpenApiOperationSerializer, OpenApiOperation>>(); 
            serviceCollection.AddTransient<IOpenApiElementSerializer<OpenApiEncoding>, LazyOpenApiElementSerializer<V3OpenApiEncodingSerializer, OpenApiEncoding>>();
        }

        private void RegisterReferenceElementSerializer<TReferenceElementSerialier, TReferenceElement>(
            IServiceCollection serviceCollection) 
            where TReferenceElementSerialier : class, IOpenApiReferenceElementSerializer<TReferenceElement>
            where TReferenceElement : IOpenApiReferenceable
        {
            serviceCollection.AddTransient<IOpenApiReferenceElementSerializer<TReferenceElement>, TReferenceElementSerialier>();
            serviceCollection.AddTransient<IOpenApiElementSerializer<TReferenceElement>, TReferenceElementSerialier>();
        }

        private void RegisterElementSerializer<TElementSerializer, TElement>(
            IServiceCollection serviceCollection)
            where TElementSerializer : class, IOpenApiElementSerializer<TElement>
            where TElement : IOpenApiElement
        {
            serviceCollection.AddTransient<IOpenApiElementSerializer<TElement>, TElementSerializer>();
        }
    }
}
