
namespace Microsoft.OpenApi.Writers
{
    using System.IO;
    using Microsoft.OpenApi;
    public interface IOpenApiWriter {
        void Write(Stream stream, OpenApiDocument document);
    }
}
