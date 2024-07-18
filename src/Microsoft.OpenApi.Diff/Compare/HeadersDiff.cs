using System.Collections.Generic;
using Microsoft.OpenApi.Diff.BusinessObjects;
using Microsoft.OpenApi.Diff.Utils;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Diff.Compare
{
    public class HeadersDiff
    {
        private readonly OpenApiDiff _openApiDiff;

        public HeadersDiff(OpenApiDiff openApiDiff)
        {
            _openApiDiff = openApiDiff;
        }

        public ChangedHeadersBO Diff(IDictionary<string, OpenApiHeader> left, IDictionary<string, OpenApiHeader> right, DiffContextBO context)
        {
            var headerMapDiff = MapKeyDiff<string, OpenApiHeader>.Diff(left, right);
            var sharedHeaderKeys = headerMapDiff.SharedKey;

            var changed = new Dictionary<string, ChangedHeaderBO>();
            foreach (var headerKey in sharedHeaderKeys)
            {
                var oldHeader = left[headerKey];
                var newHeader = right[headerKey];
                var changedHeaders = _openApiDiff
                    .HeaderDiff
                    .Diff(oldHeader, newHeader, context);
                if (changedHeaders != null)
                    changed.Add(headerKey, changedHeaders);
            }

            return ChangedUtils.IsChanged(
                new ChangedHeadersBO(left, right, context)
                {
                    Increased = headerMapDiff.Increased,
                    Missing = headerMapDiff.Missing,
                    Changed = changed
                });
        }
    }
}
