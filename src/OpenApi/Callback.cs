
namespace Microsoft.OpenApi
{
    using System;
    using System.Collections.Generic;

    public class Callback : IReference
    {
        public Dictionary<RuntimeExpression, PathItem> PathItems { get; set; } = new Dictionary<RuntimeExpression, PathItem>();

        public OpenApiReference Pointer { get; set; }

        public Dictionary<string, string> Extensions { get; set; }

                


    }
}
