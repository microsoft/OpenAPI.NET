using System;

namespace Microsoft.OpenApi
{
    public class OpenApiException : Exception
    {
        public string Pointer { get; set; }
        public OpenApiException(string message) : base(message)
        {

        }

    }


}
