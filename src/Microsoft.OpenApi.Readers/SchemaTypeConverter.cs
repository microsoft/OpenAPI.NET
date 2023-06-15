// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using Json.Schema;

namespace Microsoft.OpenApi.Readers
{
    internal static class SchemaTypeConverter
    {
        internal static SchemaValueType ConvertToSchemaValueType(string value)
        {
            return value switch
            {
                "string" => SchemaValueType.String,
                "number" => SchemaValueType.Number,
                "integer" => SchemaValueType.Integer,
                "boolean" => SchemaValueType.Boolean,
                "array" => SchemaValueType.Array,
                "object" => SchemaValueType.Object,
                "null" => SchemaValueType.Null,
                "double" => SchemaValueType.Number,
                _ => throw new NotSupportedException(),
            };
        }        
    }
}
