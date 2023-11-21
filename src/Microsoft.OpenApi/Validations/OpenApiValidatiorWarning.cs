using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Validations
{
    /// <summary>
    /// Warnings detected when validating an OpenAPI Element
    /// </summary>
    public class OpenApiValidatorWarning : OpenApiError
    {
        /// <summary>
        /// Initializes the <see cref="OpenApiError"/> class.
        /// </summary>
        public OpenApiValidatorWarning(string ruleName, string pointer, string message) : base(pointer, message)
        {
            RuleName = ruleName;
        }

        /// <summary>
        /// Name of rule that detected the error.
        /// </summary>
        public string RuleName { get; set; }
    }

}
