// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.IO;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.Interface;

namespace Microsoft.OpenApi.Readers
{
    /// <summary>
    /// Service class for converting strings into OpenApiDocument instances
    /// </summary>
    public class OpenApiStringReader : IOpenApiReader<string, OpenApiDiagnostic>
    {
        /// <summary>
        /// Reads the string input and parses it into an Open API document.
        /// </summary>
        public OpenApiDocument Read(string input, out OpenApiDiagnostic diagnostic)
        {
            using (var memoryStream = new MemoryStream())
            {
                var writer = new StreamWriter(memoryStream);
                writer.Write(input);
                writer.Flush();
                memoryStream.Position = 0;

                return new OpenApiStreamReader().Read(memoryStream, out diagnostic);
            }
        }
    }
}