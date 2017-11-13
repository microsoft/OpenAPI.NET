// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using SharpYaml.Serialization;
using System;
using System.IO;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Xunit;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Microsoft.OpenApi.Tests
{
    public class ExampleTests
    {
        [Fact] 
        public void WriteResponseExample()
        {
            var doc = new OpenApiDocument
            {
                Info = new OpenApiInfo()
                {
                    Title = "test",
                    Version = new Version(1, 0)
                }
            };

            doc.AddPathItem("/test",
                p => p.AddOperation(OperationType.Get,
                  o => o.AddResponse("200", r =>
                 {
                     r.Description = "foo";
                     r.AddMediaType("application/json", c =>
                     {
                         c.Example = new OpenApiString("xyz"); ///"{ \"foo\": \"bar\" }"; This doesn't work because parser treats it as a node
                     });
                })));

            var stream = new MemoryStream();
            doc.SerializeAsJson(stream);
            stream.Position = 0;

            var root = JObject.Load(new JsonTextReader(new StreamReader(stream)));

            var example = root["paths"]["/test"]["get"]["responses"]["200"]["content"]["application/json"]["example"].Value<string>(); 

            Assert.Equal("xyz", example);
        }
    }

    public static class YamlExtensions
    {
        public static YamlMappingNode AsMap(this YamlNode node)
        {
            return node as YamlMappingNode;
        }
    }
}
