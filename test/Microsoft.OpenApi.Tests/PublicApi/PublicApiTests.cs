// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.IO;
using PublicApiGenerator;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.OpenApi.Tests.PublicApi
{
    [Collection("DefaultSettings")]
    public class PublicApiTests
    {
        private readonly ITestOutputHelper _output;

        public PublicApiTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void ReviewPublicApiChanges()
        {
            // This test is a safety check when you modify the public api surface.
            // If it fails, it means you changed something public. This is not always a problem!
            // It takes a human to read the change, determine if it is breaking and update the PublicApi.approved.txt with the new approved API surface

            // Arrange
            var publicApi = typeof(OpenApiSpecVersion).Assembly.GeneratePublicApi(new() { AllowNamespacePrefixes = new[] { "Microsoft.OpenApi" } } );

            // Act
            var approvedFilePath = Path.Combine("PublicApi", "PublicApi.approved.txt");

            if (!File.Exists(approvedFilePath))
                using (var _ = File.CreateText(approvedFilePath)) { }

            var approvedApi = File.ReadAllText(approvedFilePath);

            if (!approvedApi.Equals(publicApi))
            {
                _output.WriteLine("This test is a safety check when you modify the public api surface.");
                _output.WriteLine("It has failed. This means you changed something public. This is not always a problem!");
                _output.WriteLine("It takes a human to read the change, determine if it is breaking and update the PublicApi.approved.txt with the new approved API surface.");
                _output.WriteLine("The new API surface can be found in PublicApi.current.txt");
                _output.WriteLine(string.Empty);
                _output.WriteLine("The new public api is:");
                _output.WriteLine(publicApi);

                File.WriteAllText(Path.Combine("PublicApi", "PublicApi.current.txt"), publicApi);
            }

            // Assert
            Assert.Equal(approvedApi, publicApi);
        }
    }
}
