using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OpenApi
{
 
    public class Schema : IReference

    {
        public string Title { get; set; }
        public string Type { get; set; }
        public string Format { get; set; }
        public string Description { get; set; }
        public decimal? Maximum { get; set; }
        public bool ExclusiveMaximum { get; set; } = false;
        public decimal? Minimum { get; set; }
        public bool ExclusiveMinimum { get; set; } = false;
        public int? MaxLength { get; set; }
        public int? MinLength { get; set; }
        public string Pattern { get; set; }
        public decimal MultipleOf { get; set; }
        public string Default { get; set; }
        public bool ReadOnly { get; set; }
        public bool WriteOnly { get; set; }
        public List<Schema> AllOf { get; set; }
        public List<Schema> OneOf { get; set; }
        public List<Schema> AnyOf { get; set; }
        public Schema Not { get; set; }
        public string[] Required { get; set; }
        public Schema Items { get; set; }
        public int? MaxItems { get; set; }
        public int? MinItems { get; set; }
        public bool UniqueItems { get; set; }
        public Dictionary<string,Schema> Properties { get; set; }
        public int? MaxProperties { get; set; }
        public int? MinProperties { get; set; }
        public bool AdditionalPropertiesAllowed { get; set; }
        public Schema AdditionalProperties { get; set; }

        public string Example { get; set; }
        public List<string> Enum { get; set; } = new List<string>();
        public bool Nullable { get; set; }
        public ExternalDocs ExternalDocs { get; set; }
        public bool Deprecated { get; set; }

        public Dictionary<string, IOpenApiExtension> Extensions { get; set; } = new Dictionary<string, IOpenApiExtension>();

        public OpenApiReference Pointer
        {
            get;
            set;
        }


        
    }
}