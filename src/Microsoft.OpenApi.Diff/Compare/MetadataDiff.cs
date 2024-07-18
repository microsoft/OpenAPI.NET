using Microsoft.OpenApi.Diff.BusinessObjects;
using Microsoft.OpenApi.Diff.Utils;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Diff.Compare
{
    public class MetadataDiff
    {
        private OpenApiComponents _leftComponents;
        private OpenApiComponents _rightComponents;
        private OpenApiDiff _openApiDiff;

        public MetadataDiff(OpenApiDiff openApiDiff)
        {
            _openApiDiff = openApiDiff;
            _leftComponents = openApiDiff.OldSpecOpenApi?.Components;
            _rightComponents = openApiDiff.NewSpecOpenApi?.Components;
        }

        public ChangedMetadataBO Diff(string left, string right, DiffContextBO context)
        {
            return ChangedUtils.IsChanged(new ChangedMetadataBO(left, right));
        }
    }
}
