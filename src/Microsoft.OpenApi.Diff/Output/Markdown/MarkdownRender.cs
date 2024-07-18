using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Diff.BusinessObjects;
using Microsoft.OpenApi.Diff.Enums;
using Microsoft.OpenApi.Diff.Extensions;

namespace Microsoft.OpenApi.Diff.Output.Markdown
{
    public class MarkdownRender : BaseRenderer, IMarkdownRender
    {
        private readonly ILogger<MarkdownRender> _logger;
        private readonly string _title;

        public MarkdownRender(ILogger<MarkdownRender> logger)
        {
            _logger = logger;
        }

        public MarkdownRender(string title, ILogger<MarkdownRender> logger) : this(logger)
        {
            _title = title;
        }

        public Task<string> Render(ChangedOpenApiBO diff)
        {
            var model = !_title.IsNullOrEmpty() ? GetRenderModel(diff, _title) : GetRenderModel(diff);
            return Task.FromResult(GetIndex(model));
        }


        private string GetIndex(RenderViewModel model)
        {
            return $"# {model.Name}\n" +
                   $"Compared Specs: **{model.OldSpecIdentifier}** - **{model.NewSpecIdentifier}**\n" +
                   $"Report Result: " + 
                   $"<img src=\"https://img.shields.io/static/v1?label=&amp;message={model.ChangeType.DiffResult}&amp;color={GetColorForDiffResult(model.ChangeType.DiffResult)}&amp;\" alt=\"{model.ChangeType.DiffResult}\">\n" +
                   $"## Added Endpoints\n" +
                   $"{GetOperationOverview(model.NewEndpoints)}\n" +
                   $"## Removed Endpoints\n" +
                   $"{GetOperationOverview(model.MissingEndpoints)}\n" +
                   $"## Deprecated Endpoints\n" +
                   $"{GetOperationOverview(model.DeprecatedEndpoints)}\n" +
                   $"## Changed Endpoints\n" +
                   $"{GetChangedOperationOverview(model.ChangedEndpoints)}";
        }

        private string GetOperationOverview(IEnumerable<EndpointBO> endpoints)
        {
            var returnString = string.Empty;
            foreach (var endpoint in endpoints)
            {
                returnString += $"<img src=\"https://img.shields.io/static/v1?label=&amp;message={endpoint.Method}&amp;color=grey&amp;\" alt=\"{endpoint.Method}\"> **{endpoint.PathUrl}**\n";
            }
            return returnString;
        }

        private string GetColorForDiffResult(DiffResultEnum diffResult)
        {
            switch (diffResult)
            {
                case DiffResultEnum.NoChanges:
                    return "grey";
                case DiffResultEnum.Metadata:
                    return "blue";
                case DiffResultEnum.Compatible:
                    return "green";
                case DiffResultEnum.Unknown:
                    return "orange";
                case DiffResultEnum.Incompatible:
                    return "red";
                default:
                    throw new ArgumentOutOfRangeException(nameof(diffResult), diffResult, null);
            }
        }

        private string GetChangedOperationOverview(IEnumerable<ChangedEndpointViewModel> endpoints)
        {
            var returnString = string.Empty;
            foreach (var endpoint in endpoints)
            {
                returnString += $"<details>\n" +
                                $"  <summary>" +
                                $"<img src=\"https://img.shields.io/static/v1?label=&amp;message={endpoint.Method}&amp;color=grey&amp;\" alt=\"{endpoint.Method}\"> " +
                                $"{endpoint.PathUrl} " +
                                $"<img src=\"https://img.shields.io/static/v1?label=&amp;message={endpoint.ChangeType.DiffResult}&amp;color={GetColorForDiffResult(endpoint.ChangeType.DiffResult)}&amp;\" alt=\"{endpoint.ChangeType.DiffResult}\">" +
                                $"</summary>\n" +
                                $"  \n";

                if (endpoint.ChangeType.IsIncompatible())
                {
                    returnString += $"> <details>\n" +
                                    $">   <summary>Breaking Changes</summary>\n" +
                                    $">   \n" +
                                    $"{GetChangeDetails(endpoint.ChangesByType.Where(x => x.ChangeType.IsIncompatible()))}" +
                                    $"> </details>\n" +
                                    $"> \n";
                }

                if (endpoint.ChangesByType.Any(x => x.ChangeType.IsCompatible()))
                {
                    returnString += $"> <details>\n" +
                                    $">   <summary>Compatible Changes</summary>\n" +
                                    $">   \n" +
                                    $"{GetChangeDetails(endpoint.ChangesByType.Where(x => x.ChangeType.IsCompatible()))}" +
                                    $"> </details>\n" +
                                    $"> \n";
                }

                returnString += $"</details>\n\n";
            }
            return returnString;
        }

        private string GetChangeDetails(IEnumerable<ChangeViewModel> changes)
        {
            var returnString = string.Empty;
            foreach (var change in changes)
            {
                returnString += $">   - **{string.Join(" - ", change.Path)}**\n";

                foreach (var singleChange in change.Changes)
                {
                    returnString += $">     - {singleChange.ElementType} Modification\n";

                    if (singleChange.ChangeType == TypeEnum.Changed)
                    {
                        returnString += $">       - `{singleChange.ElementType}` changed from " +
                                        $"`{(!singleChange.OldValue.IsNullOrEmpty() ? singleChange.OldValue : " ")}` to " +
                                        $"`{(!singleChange.NewValue.IsNullOrEmpty() ? singleChange.NewValue : " ")}`\n";
                    }
                    else
                    {
                        returnString += $">       - {singleChange.ChangeType} `{singleChange.FieldName}`\n";
                    }
                }
            }
            returnString += $">   \n";
            return returnString;
        }
    }
}
