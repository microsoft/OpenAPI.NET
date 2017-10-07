using System;

namespace Microsoft.OpenApi
{
    public class DomainParseException : Exception
    {
        public string Pointer { get; set; }
        public DomainParseException(string message) : base(message)
        {

        }

    }


}
