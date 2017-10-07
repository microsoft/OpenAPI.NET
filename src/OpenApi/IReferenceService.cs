
namespace Microsoft.OpenApi
{
    public interface IReferenceService
    {
        IReference LoadReference(OpenApiReference reference);
        OpenApiReference ParseReference(string pointer);
    }


}
