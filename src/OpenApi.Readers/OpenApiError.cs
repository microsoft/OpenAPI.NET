

namespace Microsoft.OpenApi
{

    public class OpenApiError
    {
        string pointer;
        string message;

        public OpenApiError(DomainParseException ex)
        {
            this.message = ex.Message;
            this.pointer = ex.Pointer;
        }
        public OpenApiError(string pointer, string message)
        {
            this.pointer = pointer;
            this.message = message;
        }

        public override string ToString()
        {
            return this.message + (!string.IsNullOrEmpty(this.pointer) ? " at " + this.pointer : "");
        }
    }


}
