namespace Microsoft.OpenApi.VisualStudio.Generators
{
    using System;
    using System.IO;
    using System.Text;

    using Microsoft.OpenApi.Extensions;
    using Microsoft.OpenApi.Readers;
    using Microsoft.VisualStudio.Shell.Interop;

    internal class OpenApiGenerator
    {
        private readonly IVsGeneratorProgress codeGeneratorProgress;

        internal OpenApiGenerator(IVsGeneratorProgress codeGeneratorProgress)
        {
            this.codeGeneratorProgress = codeGeneratorProgress;
        }

        internal byte[] ConvertOpenApiDocument(string inputFileContent, OpenApiSpecVersion apiSpecVersion, OpenApiFormat apiFormat)
        {
            using (Stream stream = this.CreateStream(inputFileContent))
            {
                var document = new OpenApiStreamReader().Read(stream, out var context);

                this.codeGeneratorProgress?.Progress(50, 100);

                var outputStream = new MemoryStream();
                document.Serialize(outputStream, apiSpecVersion, apiFormat);
                
                this.codeGeneratorProgress?.Progress(100, 100);
                var encoding = Encoding.GetEncoding(Encoding.UTF8.WindowsCodePage);

                //Get the preamble (byte-order mark) for our encoding
                byte[] preamble = encoding.GetPreamble();
                int preambleLength = preamble.Length;

                outputStream.Position = 0;

                //Convert the writer contents to a byte array
                byte[] body = encoding.GetBytes(new StreamReader(outputStream).ReadToEnd());

                //Prepend the preamble to body (store result in resized preamble array)
                Array.Resize(ref preamble, preambleLength + body.Length);
                Array.Copy(body, 0, preamble, preambleLength, body.Length);

                //Return the combined byte array
                return preamble;
            }
        }

        private MemoryStream CreateStream(string text)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);

            writer.Write(text);
            writer.Flush();
            stream.Position = 0;

            return stream;
        }
    }
}