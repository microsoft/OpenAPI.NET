using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Diff.BusinessObjects
{
    public class EndpointBO
    {
        public string PathUrl { get; set; }
        public OperationType Method { get; set; }
        public string Summary { get; set; }
        public OpenApiPathItem Path { get; set; }
        public OpenApiOperation Operation { get; set; }

        public override string ToString()
        {
            return Method + " " + PathUrl;
        }
    }
}
