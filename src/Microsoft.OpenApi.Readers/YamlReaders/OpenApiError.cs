namespace Microsoft.OpenApi.Readers.YamlReaders
{
    public class OpenApiError
    {
        string pointer;
        string message;

        public OpenApiError(OpenApiException ex)
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
