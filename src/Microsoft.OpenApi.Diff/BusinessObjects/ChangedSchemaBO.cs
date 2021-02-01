using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Diff.Enums;
using Microsoft.OpenApi.Diff.Extensions;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Diff.BusinessObjects
{
    public class ChangedSchemaBO : ComposedChangedBO
    {
        protected override ChangedElementTypeEnum GetElementType() => ChangedElementTypeEnum.Schema;

        public DiffContextBO Context { get; set; }
        public OpenApiSchema OldSchema { get; set; }
        public OpenApiSchema NewSchema { get; set; }
        public string Type { get; set; }
        public Dictionary<string, ChangedSchemaBO> ChangedProperties { get; set; }
        public Dictionary<string, OpenApiSchema> IncreasedProperties { get; set; }
        public Dictionary<string, OpenApiSchema> MissingProperties { get; set; }
        public bool IsChangeDeprecated { get; set; }
        public ChangedMetadataBO Description { get; set; }
        public bool IsChangeTitle { get; set; }
        public ChangedRequiredBO Required { get; set; }
        public bool IsChangeDefault { get; set; }
        public ChangedEnumBO Enumeration { get; set; }
        public bool IsChangeFormat { get; set; }
        public ChangedReadOnlyBO ReadOnly { get; set; }
        public ChangedWriteOnlyBO WriteOnly { get; set; }
        public bool IsChangedType { get; set; }
        public ChangedMaxLengthBO MaxLength { get; set; }
        public bool DiscriminatorPropertyChanged { get; set; }
        public ChangedSchemaBO Items { get; set; }
        public ChangedOneOfSchemaBO OneOfSchema { get; set; }
        public ChangedSchemaBO AddProp { get; set; }
        public ChangedExtensionsBO Extensions { get; set; }

        public ChangedSchemaBO() 
        {
            IncreasedProperties = new Dictionary<string, OpenApiSchema>();
            MissingProperties = new Dictionary<string, OpenApiSchema>();
            ChangedProperties = new Dictionary<string, ChangedSchemaBO>();
        }

        public override List<(string Identifier, ChangedBO Change)> GetChangedElements()
        {
            return new List<(string Identifier, ChangedBO Change)>(
                        ChangedProperties.Select(x => (x.Key, (ChangedBO)x.Value))
                    )
                {
                    ("Description", Description),
                    ("ReadOnly", ReadOnly),
                    ("WriteOnly", WriteOnly),
                    ("Items", Items),
                    ("OneOfSchema", OneOfSchema),
                    ("AddProp", AddProp),
                    ("Enumeration", Enumeration),
                    ("Required", Required),
                    ("MaxLength", MaxLength),
                    (null, Extensions)
                }
                .Where(x => x.Change != null).ToList();
        }

        public override DiffResultBO IsCoreChanged()
        {
            if (
                !IsChangedType
                && (OldSchema == null && NewSchema == null || OldSchema != null && NewSchema != null)
                && !IsChangeFormat
                && IncreasedProperties.Count == 0
                && MissingProperties.Count == 0
                && ChangedProperties.Values.Count == 0
                && !IsChangeDeprecated
                && ! DiscriminatorPropertyChanged
            )
                return new DiffResultBO(DiffResultEnum.NoChanges);

            var compatibleForRequest = OldSchema != null || NewSchema == null;
            var compatibleForResponse =
                MissingProperties.IsNullOrEmpty() && (OldSchema == null || NewSchema != null);

            if ((Context.IsRequest && compatibleForRequest
                 || Context.IsResponse && compatibleForResponse)
                && !IsChangedType
                && !DiscriminatorPropertyChanged)
            {
                return new DiffResultBO(DiffResultEnum.Compatible);
            }
            return new DiffResultBO(DiffResultEnum.Incompatible);
        }

        protected override List<ChangedInfoBO> GetCoreChanges()
        {
            var returnList = GetCoreChangeInfosOfComposed(IncreasedProperties.Keys.ToList(), MissingProperties.Keys.ToList(), x => x);
            var elementType = GetElementType();
            const TypeEnum changeType = TypeEnum.Changed;

            if (IsChangedType)
                returnList.Add(new ChangedInfoBO(elementType, changeType, "Type", OldSchema?.Type, NewSchema?.Type));

            if (IsChangeDefault)
                returnList.Add(new ChangedInfoBO(elementType, changeType, "Default", OldSchema?.Default.ToString(), NewSchema?.Default.ToString()));

            if (IsChangeDeprecated)
                returnList.Add(new ChangedInfoBO(elementType, changeType, "Deprecation", OldSchema?.Deprecated.ToString(), NewSchema?.Deprecated.ToString()));

            if (IsChangeFormat)
                returnList.Add(new ChangedInfoBO(elementType, changeType, "Format", OldSchema?.Format, NewSchema?.Format));

            if (IsChangeTitle)
                returnList.Add(new ChangedInfoBO(elementType, changeType, "Title", OldSchema?.Title, NewSchema?.Title));

            if (DiscriminatorPropertyChanged)
                returnList.Add(new ChangedInfoBO(elementType, changeType, "Discriminator Property", OldSchema?.Discriminator?.PropertyName, NewSchema?.Discriminator?.PropertyName));

            return returnList;
        }
    }
}
