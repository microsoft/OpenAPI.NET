using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Diff.BusinessObjects;
using Microsoft.OpenApi.Diff.Enums;
using Xunit;

namespace Microsoft.OpenApi.Diff.Tests
{
    public class TestUtils : ITestUtils
    {
        private readonly IOpenAPICompare _openAPICompare;

        public TestUtils(IOpenAPICompare openAPICompare)
        {
            _openAPICompare = openAPICompare;
        }

        public void AssertOpenAPIAreEquals(string oldSpec, string newSpec)
        {
            var changedOpenAPI = _openAPICompare.FromLocations(oldSpec, newSpec);
            Assert.Empty(changedOpenAPI.NewEndpoints);
            Assert.Empty(changedOpenAPI.MissingEndpoints);
            Assert.Empty(changedOpenAPI.ChangedOperations);
        }

        public void AssertOpenAPIChangedEndpoints(string oldSpec, string newSpec)
        {
            var changedOpenAPI = _openAPICompare.FromLocations(oldSpec, newSpec);
            Assert.Empty(changedOpenAPI.NewEndpoints);
            Assert.Empty(changedOpenAPI.MissingEndpoints);
            Assert.NotEmpty(changedOpenAPI.ChangedOperations);
        }

        public void AssertOpenAPIBackwardCompatible(string oldSpec, string newSpec, bool isDiff)
        {
            var changedOpenAPI = _openAPICompare.FromLocations(oldSpec, newSpec);
            Assert.True(changedOpenAPI.IsCompatible());
        }

        public void AssertOpenAPIBackwardIncompatible(string oldSpec, string newSpec)
        {
            var changedOpenAPI = _openAPICompare.FromLocations(oldSpec, newSpec);
            Assert.True(changedOpenAPI.IsIncompatible());
            Assert.NotEmpty(GetChangesOfType(changedOpenAPI, DiffResultEnum.Incompatible));
        }

        public IEnumerable<ChangedInfosBO> GetChangesOfType(ChangedOpenApiBO changedOpenAPI, DiffResultEnum changeType)
        {
            return changedOpenAPI.GetChangedElements()
                .SelectMany(x => x.Change.GetAllChangeInfoFlat(null))
                .Where(y => y.ChangeType.DiffResult == changeType)
                .ToList();
        }

        public IOpenAPICompare GetOpenAPICompare()
        {
            return _openAPICompare;
        }
    }
}
