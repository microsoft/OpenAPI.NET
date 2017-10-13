using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OpenApi.Sevices
{
    public class OpenApiValidator : OpenApiVisitorBase
    {
        private List<OpenApiException> openApiException;

        public List<OpenApiException> Exceptions { get { return this.openApiException; } }
        public OpenApiValidator()
        {
            this.openApiException = new List<OpenApiException>();
        }
        public override void Visit(Response response)
        {
            if (string.IsNullOrEmpty(response.Description))
            {
                this.openApiException.Add(new OpenApiException("Response must have a description"));
            }
        }
    }
}
