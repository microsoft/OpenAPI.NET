using Tavis.OpenApi.Model;

namespace Tavis.OpenApi
{
    public interface IReferenceService
    {
        IReference LoadReference(OpenApiReference reference);
        OpenApiReference ParseReference(string pointer);
    }


}
