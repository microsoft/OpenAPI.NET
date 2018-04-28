/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

namespace Microsoft.OpenApi.VisualStudio.Generators
{
    using System;
    using System.Runtime.InteropServices;

    using Microsoft.OpenApi;
    using Microsoft.OpenApi.VisualStudio.Generators.CodeGenerators;
    using Microsoft.VisualStudio.Shell;

    using VSLangProj80;

    /// <summary>
    /// When setting the 'Custom Tool' property of a C#, VB, or J# project item to "OpenApi3_0YamlToJsonGenerator", 
    /// the GenerateCode function will get called and will return the contents of the generated file to the project system
    /// </summary>
    [ComVisible(true)]
    [Guid("72060E8C-4062-4D19-AA6E-E693CD604652")]
    [ProvideObject(typeof(OpenApi3_0YamlToJsonGenerator))]
    [CodeGeneratorRegistrationWithFileExtension(typeof(OpenApi3_0YamlToJsonGenerator), "", "{9A19103F-16F7-4668-BE54-9A1E7A4F7556}", GeneratesDesignTimeSource = true, FileExtension = ".yaml")]
    [CodeGeneratorRegistrationWithFileExtension(typeof(OpenApi3_0YamlToJsonGenerator), "", vsContextGuids.vsContextGuidVCSProject, GeneratesDesignTimeSource = true, FileExtension = ".yaml")]
    public class OpenApi3_0YamlToJsonGenerator : BaseCodeGeneratorWithSite
    {
#pragma warning disable 0414
        //The name of this generator (use for 'Custom Tool' property of project item)
        internal static string name = "OpenApi3_0YamlToJsonGenerator";
#pragma warning restore 0414

        /// <summary>
        /// Function that builds the contents of the generated file based on the contents of the input file
        /// </summary>
        /// <param name="inputFileContent">Content of the input file</param>
        /// <returns>Generated file as a byte array</returns>
        protected override byte[] GenerateCode(string inputFileContent)
        {
            try
            {
                var openApiGenerator = new OpenApiGenerator(this.CodeGeneratorProgress);
                return openApiGenerator.ConvertOpenApiDocument(inputFileContent, OpenApiSpecVersion.OpenApi3_0, OpenApiFormat.Json);
            }
            catch (Exception e)
            {
                this.GeneratorError(4, e.ToString(), 1, 1);
                //Returning null signifies that generation has failed
                return null;
            }
        }

        /// <summary>
        /// Set the extension of the generated file (the JSON version of the YAML OpenAPI document)
        /// </summary>
        /// <returns></returns>
        protected override string GetDefaultExtension()
        {
            return ".json";
        }
    }
}