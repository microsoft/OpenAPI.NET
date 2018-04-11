// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Models;
using SharpYaml;

namespace Microsoft.OpenApi.Readers
{
    /// <summary>
    /// Error detected during the reading of some input and converting to an OpenApiDocument
    /// </summary>
    public class OpenApiReaderError : OpenApiError
    {

        /// <summary>
        /// Creates error object from thrown exception
        /// </summary>
        /// <param name="exception"></param>
        public OpenApiReaderError(OpenApiException exception) : base(exception)
        {

        }

        public OpenApiReaderError(ParsingContext context, string message) : base(context.GetLocation(), message)
        {

        }


        /// <summary>
        /// Create error object from YAML SyntaxErrorException
        /// </summary>
        /// <param name="exception"></param>
        public OpenApiReaderError(SyntaxErrorException exception) : base(String.Empty, exception.Message)
        {

        }
    }
}
