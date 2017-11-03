// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers.ParseNodes;

namespace Microsoft.OpenApi.Readers.OpenApiV2Deserializer
{
    /// <summary>
    /// Class containing logic to deserialize Open API V2 document into
    /// runtime Open API object model.
    /// </summary>
    internal static partial class OpenApiV2Deserializer
    {
        #region ExampleObject
        private static FixedFieldMap<OpenApiExample> ExampleFixedFields = new FixedFieldMap<OpenApiExample>
        {
        };

        private static PatternFieldMap<OpenApiExample> ExamplePatternFields = new PatternFieldMap<OpenApiExample>
        {
            { (s)=> s.StartsWith("x-"), (o,k,n)=> o.Extensions.Add(k,  new OpenApiString(n.GetScalarValue())) }
        };

        public static OpenApiExample LoadExample(ParseNode node)
        {
            var mapNode = node.CheckMapNode("Example");
            var example = new OpenApiExample();
            foreach (var property in mapNode)
            {
                property.ParseField(example, ExampleFixedFields, ExamplePatternFields);
            }

            return example;
        }


        #endregion
    }
}
