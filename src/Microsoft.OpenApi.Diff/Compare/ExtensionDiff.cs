using Microsoft.OpenApi.Diff.BusinessObjects;
using Microsoft.OpenApi.Diff.Enums;

namespace Microsoft.OpenApi.Diff.Compare
{
    public abstract class ExtensionDiff : IExtensionDiff
    {
        public abstract ExtensionDiff SetOpenApiDiff(OpenApiDiff openApiDiff);

        public abstract string GetName();

        public abstract ChangedBO Diff<T>(ChangeBO<T> extension, DiffContextBO context)
            where T : class;

        public virtual bool IsParentApplicable(TypeEnum type, object objectElement, object extension, DiffContextBO context)
        {
            return true;
        }
    }
}
