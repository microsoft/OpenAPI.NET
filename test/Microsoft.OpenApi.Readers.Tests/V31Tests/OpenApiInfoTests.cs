using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Reader;
using Microsoft.OpenApi.Reader.ParseNodes;
using Microsoft.OpenApi.Reader.V31;
using SharpYaml.Serialization;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests.V31Tests
{
    public class OpenApiInfoTests
    {
        private const string SampleFolderPath = "V31Tests/Samples/OpenApiInfo/";

        [Fact]
        public void ParseBasicInfoShouldSucceed()
        {
            using var stream = Resources.GetStream(Path.Combine(SampleFolderPath, "basicInfo.yaml"));
            var yamlStream = new YamlStream();
            yamlStream.Load(new StreamReader(stream));
            var yamlNode = yamlStream.Documents.First().RootNode;

            var diagnostic = new OpenApiDiagnostic();
            var context = new ParsingContext(diagnostic);

            var asJsonNode = yamlNode.ToJsonNode();
            var node = new MapNode(context, asJsonNode);

            // Act
            var openApiInfo = OpenApiV31Deserializer.LoadInfo(node);

            // Assert
            openApiInfo.Should().BeEquivalentTo(
                new OpenApiInfo
                {
                    Title = "Basic Info",
                    Summary = "Sample Summary",
                    Description = "Sample Description",
                    Version = "1.0.1",
                    TermsOfService = new Uri("http://swagger.io/terms/"),
                    Contact = new OpenApiContact
                    {
                        Email = "support@swagger.io",
                        Name = "API Support",
                        Url = new Uri("http://www.swagger.io/support")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Apache 2.0",
                        Url = new Uri("http://www.apache.org/licenses/LICENSE-2.0.html")
                    }
                });
        }
    }
}
