using System;
using System.CommandLine.Invocation;
using System.Threading.Tasks;

namespace resultsComparer.Handlers;

internal abstract class AsyncCommandHandler : ICommandHandler
{
    public int Invoke(InvocationContext context)
    {
        throw new InvalidOperationException("This method should not be called");
    }
    public abstract Task<int> InvokeAsync(InvocationContext context);
}
