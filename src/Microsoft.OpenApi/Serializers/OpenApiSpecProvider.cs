using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OpenApi.Interfaces;

namespace Microsoft.OpenApi.Serializers
{
    public class OpenApiSpecProvider : IOpenApiSpecProvider
    {
        private readonly OpenApiSpecVersion _specVersion;
        private OpenApiSpecProvider(OpenApiSpecVersion specVersion)
        {
            _specVersion = specVersion;
        }

        public static OpenApiSpecProvider V2Version { get; } = new OpenApiSpecProvider(OpenApiSpecVersion.OpenApi2_0);

        public static OpenApiSpecProvider V3Version { get; } = new OpenApiSpecProvider(OpenApiSpecVersion.OpenApi3_0);

        public OpenApiSpecVersion OpenApiSpec => _specVersion;
    }
}
