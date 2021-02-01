using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Diff.BusinessObjects;
using Microsoft.OpenApi.Diff.Enums;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Diff.Output
{
    public class RenderViewModel
    {
        public DiffResultBO ChangeType { get; set; }
        public DateTime CreatedDate => DateTime.Now;
        public string Author { get; set; }
        public string Description { get; set; }
        public string LogoUrl { get; set; }
        public string Name { get; set; }
        public string PageTitle { get; set; }
        public IReadOnlyCollection<EndpointBO> NewEndpoints { get; set; }
        public IReadOnlyCollection<EndpointBO> MissingEndpoints { get; set; }
        public IReadOnlyCollection<EndpointBO> DeprecatedEndpoints { get; set; }
        public IReadOnlyCollection<ChangedEndpointViewModel> ChangedEndpoints { get; set; }
        public string OldSpecIdentifier { get; set; }
        public string NewSpecIdentifier { get; set; }
    }

    public class ChangedEndpointViewModel
    {
        public string PathUrl { get; set; }
        public OperationType Method { get; set; }
        public string Summary { get; set; }
        public DiffResultBO ChangeType { get; set; }
        public List<ChangeViewModel> ChangesByType { get; set; }
    }

    public class ChangeViewModel
    {
        public List<string> Path { get; set; }
        public DiffResultBO ChangeType { get; set; }
        public List<SingleChangeViewModel> Changes { get; set; }
    }

    public class SingleChangeViewModel
    {
        public ChangedElementTypeEnum ElementType { get; set; }
        public TypeEnum ChangeType { get; set; }
        public string FieldName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
    }
}
