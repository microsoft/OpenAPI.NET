using System;
using System.CommandLine.Invocation;
using System.Threading.Tasks;

namespace Microsoft.OpenApi.Hidi.Handlers;

internal abstract class AsyncCommandHandler : ICommandHandler
{
    public int Invoke(InvocationContext context)
    {
        throw new InvalidOperationException("This method should not be called");
    }
    public abstract Task<int> InvokeAsync(InvocationContext context);
}
