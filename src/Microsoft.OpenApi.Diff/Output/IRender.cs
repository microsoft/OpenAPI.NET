using System.Threading.Tasks;
using Microsoft.OpenApi.Diff.BusinessObjects;

namespace Microsoft.OpenApi.Diff.Output
{
    public interface IRender
    {
        Task<string> Render(ChangedOpenApiBO diff);
    }
}
