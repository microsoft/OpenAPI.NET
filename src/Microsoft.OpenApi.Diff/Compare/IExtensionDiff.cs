using Microsoft.OpenApi.Diff.BusinessObjects;
using Microsoft.OpenApi.Diff.Enums;

namespace Microsoft.OpenApi.Diff.Compare
{
    public interface IExtensionDiff
    {
        ExtensionDiff SetOpenApiDiff(OpenApiDiff openApiDiff);

        string GetName();

        ChangedBO Diff<T>(ChangeBO<T> extension, DiffContextBO context)
            where T : class;

        bool IsParentApplicable(TypeEnum type, object objectElement, object extension, DiffContextBO context);
    }
}
