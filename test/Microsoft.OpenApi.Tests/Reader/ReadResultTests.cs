using Xunit;
using Microsoft.OpenApi.Reader;

namespace Microsoft.OpenApi.Tests.Reader;

public class ReadResultTests
{
    [Fact]
    public void Deconstructs()
    {
        var readResult = new ReadResult()
        {
            Document = new OpenApiDocument(),
            Diagnostic = new OpenApiDiagnostic()
        };
        var (document, diagnostic) = readResult;
        Assert.Equal(readResult.Document, document);
        Assert.Equal(readResult.Diagnostic, diagnostic);
    }
}
