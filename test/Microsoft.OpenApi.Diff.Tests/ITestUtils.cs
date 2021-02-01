using System.Collections.Generic;
using Microsoft.OpenApi.Diff.BusinessObjects;
using Microsoft.OpenApi.Diff.Enums;

namespace Microsoft.OpenApi.Diff.Tests
{
    public interface ITestUtils
    {
        void AssertOpenAPIAreEquals(string oldSpec, string newSpec);
        void AssertOpenAPIChangedEndpoints(string oldSpec, string newSpec);
        void AssertOpenAPIBackwardCompatible(string oldSpec, string newSpec, bool isDiff);
        void AssertOpenAPIBackwardIncompatible(string oldSpec, string newSpec);
        IOpenAPICompare GetOpenAPICompare();
        IEnumerable<ChangedInfosBO> GetChangesOfType(ChangedOpenApiBO changedOpenAPI, DiffResultEnum changeType);
    }
}
