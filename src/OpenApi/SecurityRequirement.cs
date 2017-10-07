
namespace Microsoft.OpenApi
{
    using System.Collections.Generic;
    using System.Linq;

    public class SecurityRequirement
    {
        public Dictionary<SecurityScheme, List<string>> Schemes { get; set; } = new Dictionary<SecurityScheme, List<string>>();

        
    }
}