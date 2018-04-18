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
    /// When setting the 'Custom Tool' property of a C#, VB, or J# project item to "OpenApi3_0JsonToYamlGenerator", 
    /// the GenerateCode function will get called and will return the contents of the generated file to the project system
    /// </summary>
    [ComVisible(true)]
    [Guid("5FE20A01-597D-47AD-9635-08C91E64281A")]
    [ProvideObject(typeof(OpenApi3_0JsonToYamlGenerator))]
    [CodeGeneratorRegistration(typeof(OpenApi3_0JsonToYamlGenerator),"C# OpenAPI 3.0 JSON to YAML Generator",vsContextGuids.vsContextGuidVCSProject, GeneratesDesignTimeSource=true)]
    [CodeGeneratorRegistration(typeof(OpenApi3_0JsonToYamlGenerator), "C# OpenAPI 3.0 JSON to YAML Generator", vsContextGuids.vsContextGuidVBProject, GeneratesDesignTimeSource = true)]
    public class OpenApi3_0JsonToYamlGenerator : BaseCodeGeneratorWithSite
    {
#pragma warning disable 0414
        //The name of this generator (use for 'Custom Tool' property of project item)
        internal static string name = "OpenApi3_0JsonToYamlGenerator";
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
                return openApiGenerator.ConvertOpenApiDocument(inputFileContent, OpenApiSpecVersion.OpenApi3_0, OpenApiFormat.Yaml);
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
            return ".yaml";
        }
    }
}