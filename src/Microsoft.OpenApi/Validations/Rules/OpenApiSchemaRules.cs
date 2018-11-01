// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Properties;

namespace Microsoft.OpenApi.Validations.Rules
{
    /// <summary>
    /// The validation rules for <see cref="OpenApiSchema"/>.
    /// </summary>
    [OpenApiRule]
    public static class OpenApiSchemaRules
    {
        internal const string DataTypeMismatchedErrorMessage = "Data and type mismatch found in \"default\". ";

        private static void ValidateDataTypeMismatch(IValidationContext context, IOpenApiAny value, OpenApiSchema schema)
        {
            if (schema == null)
            {
                return;
            }

            var type = schema.Type;
            var format = schema.Format;

            if (type == "object")
            {
                if (!(value is OpenApiObject))
                {
                    context.CreateError(
                        nameof(SchemaMismatchedDataType),
                        DataTypeMismatchedErrorMessage);
                    return;
                }

                var anyObject = (OpenApiObject)value;

                foreach (var key in anyObject.Keys)
                {
                    context.Enter(key);

                    if (schema.Properties != null && schema.Properties.ContainsKey(key))
                    {
                        ValidateDataTypeMismatch(context, anyObject[key], schema.Properties[key]);
                    }
                    else
                    {
                        ValidateDataTypeMismatch(context, anyObject[key], schema.AdditionalProperties);
                    }
                    
                    context.Exit();
                }

                return;
            }

            if (type == "array")
            {
                if (!(value is OpenApiArray))
                {
                    context.CreateError(
                        nameof(SchemaMismatchedDataType),
                        DataTypeMismatchedErrorMessage);
                    return;
                }

                var anyArray = (OpenApiArray)value;

                for (int i = 0; i < anyArray.Count; i++)
                {
                    context.Enter(i.ToString());

                    ValidateDataTypeMismatch(context, anyArray[i], schema.Items);

                    context.Exit();
                }

                return;
            }

            if (type == "integer" && format == "int32" )
            {
                if (!(value is OpenApiInteger))
                {
                    context.CreateError(
                        nameof(SchemaMismatchedDataType),
                        DataTypeMismatchedErrorMessage);
                }

                return;
            }

            if (type == "integer" && format == "int64")
            {
                if (!(value is OpenApiLong))
                {
                    context.CreateError(
                       nameof(SchemaMismatchedDataType),
                       DataTypeMismatchedErrorMessage);
                }

                return;
            }

            if (type == "integer" && !(value is OpenApiInteger))
            {
                if (!(value is OpenApiInteger))
                {
                    context.CreateError(
                        nameof(SchemaMismatchedDataType),
                        DataTypeMismatchedErrorMessage);
                }

                return;
            }

            if (type == "number" && format == "float")
            {
                if (!(value is OpenApiFloat))
                {
                    context.CreateError(
                        nameof(SchemaMismatchedDataType),
                        DataTypeMismatchedErrorMessage);
                }

                return;
            }

            if (type == "number" && format == "double")
            {
                if (!(value is OpenApiDouble))
                {
                    context.CreateError(
                        nameof(SchemaMismatchedDataType),
                        DataTypeMismatchedErrorMessage);
                }

                return;
            }

            if (type == "number")
            {
                if (!(value is OpenApiDouble))
                {
                    context.CreateError(
                        nameof(SchemaMismatchedDataType),
                        DataTypeMismatchedErrorMessage);
                }

                return;
            }

            if (type == "string" && format == "byte")
            {
                if (!(value is OpenApiByte))
                {
                    context.CreateError(
                        nameof(SchemaMismatchedDataType),
                        DataTypeMismatchedErrorMessage);
                }

                return;
            }

            if (type == "string" && format == "date")
            {
                if (!(value is OpenApiDate))
                {
                    context.CreateError(
                        nameof(SchemaMismatchedDataType),
                        DataTypeMismatchedErrorMessage);
                }

                return;
            }

            if (type == "string" && format == "date-time")
            {
                if (!(value is OpenApiDateTime))
                {
                    context.CreateError(
                        nameof(SchemaMismatchedDataType),
                        DataTypeMismatchedErrorMessage);
                }

                return;
            }

            if (type == "string" && format == "password")
            {
                if (!(value is OpenApiPassword))
                {
                    context.CreateError(
                        nameof(SchemaMismatchedDataType),
                        DataTypeMismatchedErrorMessage);
                }

                return;
            }

            if (type == "string")
            {
                if (!(value is OpenApiString))
                {
                    context.CreateError(
                        nameof(SchemaMismatchedDataType),
                        DataTypeMismatchedErrorMessage);
                }

                return;
            }

            if (type == "boolean" )
            {
                if (!(value is OpenApiBoolean))
                {
                    context.CreateError(
                        nameof(SchemaMismatchedDataType),
                        DataTypeMismatchedErrorMessage);
                }

                return;
            }
        }

        /// <summary>
        /// Validate the field is required.
        /// </summary>
        public static ValidationRule<OpenApiSchema> SchemaMismatchedDataType =>
            new ValidationRule<OpenApiSchema>(
                (context, schema) =>
                {
                    // default
                    context.Enter("default");

                    if (schema.Default != null)
                    {
                        ValidateDataTypeMismatch(context, schema.Default, schema);
                    }

                    context.Exit();

                    // example
                    context.Enter("example");

                    if (schema.Example != null)
                    {
                        ValidateDataTypeMismatch(context, schema.Example, schema);
                    }

                    context.Exit();


                    // enum
                    context.Enter("enum");

                    if (schema.Enum != null)
                    {
                        for (int i = 0; i < schema.Enum.Count; i++)
                        {
                            context.Enter(i.ToString());
                            ValidateDataTypeMismatch(context, schema.Enum[i], schema);
                            context.Exit();
                        }
                    }

                    context.Exit();
                });

        // add more rule.
    }
}
