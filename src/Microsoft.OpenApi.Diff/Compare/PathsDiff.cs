using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Diff.BusinessObjects;
using Microsoft.OpenApi.Diff.Extensions;
using Microsoft.OpenApi.Diff.Utils;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Diff.Compare
{
    public class PathsDiff
    {
        private readonly OpenApiDiff _openApiDiff;

        public PathsDiff(OpenApiDiff openApiDiff)
        {
            _openApiDiff = openApiDiff;
        }

        public ChangedPathsBO Diff(Dictionary<string, OpenApiPathItem> left, Dictionary<string, OpenApiPathItem> right)
        {
            var changedPaths = new ChangedPathsBO(left, right);

            foreach (var (key, value) in right)
            {
                changedPaths.Increased.Add(key, value);
            }

            foreach (var (key, value) in left)
            {
                var template = key.NormalizePath();
                var result = right.Keys.FirstOrDefault(x => x.NormalizePath() == template);

                if (result != null)
                {
                    if (!changedPaths.Increased.ContainsKey(result))
                        throw new ArgumentException($"Two path items have the same signature: {template}");
                    var rightPath = changedPaths.Increased[result];
                    changedPaths.Increased.Remove(result);
                    var paramsDict = new Dictionary<string, string>();
                    if (key != result)
                    {
                        var oldParams = key.ExtractParametersFromPath();
                        var newParams = result.ExtractParametersFromPath();
                        for (var i = oldParams.Count - 1; i >= 0; i--)
                        {
                            paramsDict.Add(oldParams[i], newParams[i]);
                        }
                    }
                    var context = new DiffContextBO()
                    {
                        URL = key,
                        Parameters = paramsDict
                    };

                    var diff = _openApiDiff
                        .PathDiff
                        .Diff(value, rightPath, context);

                    if (diff != null)
                        changedPaths.Changed.Add(result, diff);
                }
                else
                {
                    changedPaths.Missing.Add(key, value);
                }
            }

            return ChangedUtils.IsChanged(changedPaths);
        }

        public static OpenApiPaths ValOrEmpty(OpenApiPaths path)
        {
            return path ?? new OpenApiPaths();
        }
    }
}
