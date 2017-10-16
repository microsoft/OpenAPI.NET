
namespace Microsoft.OpenApi
{
    using System;

    public class Tag : IReference
    {
        public string Name { get; set; }
        public string Description { get; set; }

        OpenApiReference IReference.Pointer
        {
            get; set;
        }

        
    }
}