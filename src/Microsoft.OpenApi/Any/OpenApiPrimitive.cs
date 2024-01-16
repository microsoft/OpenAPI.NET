// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Globalization;
using System.Text;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Properties;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Any
{
    /// <summary>
    /// Open API primitive class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class OpenApiPrimitive<T> : IOpenApiPrimitive
    {
        /// <summary>
        /// Initializes the <see cref="IOpenApiPrimitive"/> class with the given value.
        /// </summary>
        /// <param name="value"></param>
        public OpenApiPrimitive(T value)
        {
            Value = value;
        }

        /// <summary>
        /// Initializes a copy of an <see cref="IOpenApiPrimitive"/> object
        /// </summary>
        /// <param name="openApiPrimitive"></param>
        public OpenApiPrimitive(OpenApiPrimitive<T> openApiPrimitive)
        {
            Value = openApiPrimitive.Value;
        }

        /// <summary>
        /// The kind of <see cref="IOpenApiAny"/>.
        /// </summary>
        public AnyType AnyType => AnyType.Primitive;

        /// <summary>
        /// The primitive class this object represents.
        /// </summary>
        public abstract PrimitiveType PrimitiveType { get; }

        /// <summary>
        /// Value of this <see cref="IOpenApiPrimitive"/>
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// Write out content of primitive element
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="specVersion"></param>
        public void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
        {
            switch (this.PrimitiveType)
            {
                case PrimitiveType.Integer:
                    var intValue = (OpenApiInteger)(IOpenApiPrimitive)this;
                    writer.WriteValue(intValue.Value);
                    break;

                case PrimitiveType.Long:
                    var longValue = (OpenApiLong)(IOpenApiPrimitive)this;
                    writer.WriteValue(longValue.Value);
                    break;

                case PrimitiveType.Float:
                    var floatValue = (OpenApiFloat)(IOpenApiPrimitive)this;
                    writer.WriteValue(floatValue.Value);
                    break;

                case PrimitiveType.Double:
                    var doubleValue = (OpenApiDouble)(IOpenApiPrimitive)this;
                    var actualValue = doubleValue.Value;
                    if (actualValue.Equals(double.NaN)
                        || actualValue.Equals(double.NegativeInfinity)
                        || actualValue.Equals(double.PositiveInfinity))
                    {
                        // Write out NaN, -Infinity, Infinity as strings
                        writer.WriteValue(actualValue.ToString(CultureInfo.InvariantCulture));
                        break;
                    }
                    else
                    {
                        writer.WriteValue(actualValue);
                    }
                    break;

                case PrimitiveType.String:
                    var stringValue = (OpenApiString)(IOpenApiPrimitive)this;
                    if (stringValue.IsRawString())
                        writer.WriteRaw(stringValue.Value);
                    else
                        writer.WriteValue(stringValue.Value);
                    break;

                case PrimitiveType.Byte:
                    var byteValue = (OpenApiByte)(IOpenApiPrimitive)this;
                    if (byteValue.Value == null)
                    {
                        writer.WriteNull();
                    }
                    else
                    {
                        writer.WriteValue(Convert.ToBase64String(byteValue.Value));
                    }

                    break;

                case PrimitiveType.Binary:
                    var binaryValue = (OpenApiBinary)(IOpenApiPrimitive)this;
                    if (binaryValue.Value == null)
                    {
                        writer.WriteNull();
                    }
                    else
                    {
                        writer.WriteValue(Encoding.UTF8.GetString(binaryValue.Value));
                    }

                    break;

                case PrimitiveType.Boolean:
                    var boolValue = (OpenApiBoolean)(IOpenApiPrimitive)this;
                    writer.WriteValue(boolValue.Value);
                    break;

                case PrimitiveType.Date:
                    var dateValue = (OpenApiDate)(IOpenApiPrimitive)this;
                    writer.WriteValue(dateValue.Value.ToString("o").Substring(0, 10));
                    break;

                case PrimitiveType.DateTime:
                    var dateTimeValue = (OpenApiDateTime)(IOpenApiPrimitive)this;
                    writer.WriteValue(dateTimeValue.Value);
                    break;

                case PrimitiveType.Password:
                    var passwordValue = (OpenApiPassword)(IOpenApiPrimitive)this;
                    writer.WriteValue(passwordValue.Value);
                    break;

                default:
                    throw new OpenApiWriterException(
                        string.Format(
                            SRResource.PrimitiveTypeNotSupported,
                            this.PrimitiveType));
            }
        }
    }
}
