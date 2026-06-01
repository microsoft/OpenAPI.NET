using System;
using Xunit;

namespace Microsoft.OpenApi.Tests.Exceptions;

public class OpenApiWriterExceptionTests
{
    [Fact]
    public void DefaultConstructorUsesTheGenericWriterMessage()
    {
        var exception = new OpenApiWriterException();

        Assert.Equal(SRResource.OpenApiWriterExceptionGenericError, exception.Message);
        Assert.Null(exception.InnerException);
    }

    [Fact]
    public void ConstructorPreservesMessageAndInnerException()
    {
        var innerException = new InvalidOperationException("boom");

        var exception = new OpenApiWriterException("writer failed", innerException);

        Assert.Equal("writer failed", exception.Message);
        Assert.Same(innerException, exception.InnerException);
    }
}
