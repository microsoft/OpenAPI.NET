
namespace Tavis.OpenApi
{
    using System.IO;
    using Tavis.OpenApi.Model;
    public interface IOpenApiWriter {
        void Write(Stream stream, OpenApiDocument document);
    }
}
