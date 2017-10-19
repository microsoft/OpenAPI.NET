// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.IO;

namespace Microsoft.OpenApi.Readers
{
    /// <summary>
    /// Service class for converting strings into OpenApiDocument instances
    /// </summary>
    public class OpenApiStringReader : IOpenApiReader<string, ParsingContext>
    {
        /// <summary>
        /// Reads the string input and parses it into an Open API document.
        /// </summary>
        public OpenApiDocument Read(string input, out ParsingContext context)
        {
            var memoryStream = new MemoryStream();

            var writer = new StreamWriter(memoryStream);
            writer.Write(input);
            writer.Flush();
            memoryStream.Position = 0;

            return new OpenApiStreamReader().Read(memoryStream, out context);
        }
    }
}