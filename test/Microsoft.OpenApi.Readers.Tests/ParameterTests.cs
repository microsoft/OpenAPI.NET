// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.OpenApi.Models;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests
{
    public class ParameterTests
    {
        private const string _UriReservedSymbols = ":/?#[]@!$&'()*+,;=";
        private const string _UriUnreservedSymbols = "-._~";

        private static readonly char[] HexDigits =
        {
            '0',
            '1',
            '2',
            '3',
            '4',
            '5',
            '6',
            '7',
            '8',
            '9',
            'A',
            'B',
            'C',
            'D',
            'E',
            'F'
        };

        private static string Encode(string p, bool allowReserved)
        {
            var result = new StringBuilder();
            foreach (var c in p)
            {
                if ((c >= 'A' && c <= 'z') //Alpha
                    ||
                    (c >= '0' && c <= '9') // Digit
                    ||
                    _UriUnreservedSymbols.IndexOf(c) !=
                    -1 // Unreserved symbols  - These should never be percent encoded
                    ||
                    (allowReserved && _UriReservedSymbols.IndexOf(c) != -1)
                ) // Reserved symbols - should be included if requested (+)
                {
                    result.Append(c);
                }
                else
                {
                    var bytes = Encoding.UTF8.GetBytes(new[] {c});
                    foreach (var abyte in bytes)
                    {
                        result.Append(HexEscape(abyte));
                    }
                }
            }

            return result.ToString();
        }

        public static string HexEscape(byte i)
        {
            var esc = new char[3];
            esc[0] = '%';
            esc[1] = HexDigits[((i & 240) >> 4)];
            esc[2] = HexDigits[(i & 15)];
            return new string(esc);
        }

        public static string HexEscape(char c)
        {
            var esc = new char[3];
            esc[0] = '%';
            esc[1] = HexDigits[((c & 240) >> 4)];
            esc[2] = HexDigits[(c & 15)];
            return new string(esc);
        }

        private bool IsArray(object value)
        {
            return value is IEnumerable;
        }

        private bool IsMap(object value)
        {
            return value is IDictionary<string, string>;
        }

        private bool IsSimple(object value)
        {
            return value is string;
        }

        [Theory]
        [InlineData(ParameterStyle.Matrix, false, new[] {"a", "b"}, ";bar=a,b")]
        [InlineData(ParameterStyle.Matrix, true, new[] {"a", "b"}, ";bar=a;bar=b")]
        [InlineData(ParameterStyle.Label, false, new[] {"a", "b"}, ".a.b")]
        [InlineData(ParameterStyle.Label, true, new[] {"a", "b"}, ".a.b")]
        public void SerializeArrays(ParameterStyle style, bool explode, string[] value, string expected)
        {
            var parameter = new OpenApiParameter
            {
                Name = "bar",
                Style = style,
                Explode = explode
            };

            var actual = SerializeParameterValue(parameter, value);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("://foo/bar?", "://foo/bar?")]
        [InlineData("foo bar", "foo%20bar")]
        public void SerializeEscapedStrings(string value, string expected)
        {
            var parameter = new OpenApiParameter
            {
                AllowReserved = false
            };

            var actual = SerializeParameterValue(parameter, value);
            Assert.Equal(expected, actual);
        }

        // format="space-delimited" type="array" explode=false bar=a b
        // format="space-delimited" type="array" explode=true bar=a bar=b

        [Theory]
        [InlineData(ParameterStyle.Matrix, false, ";bar=a,1,b,2")]
        [InlineData(ParameterStyle.Matrix, true, ";a=1;b=2")]
        [InlineData(ParameterStyle.Label, false, ".a.1.b.2")]
        [InlineData(ParameterStyle.Label, true, ".a=1.b=2")]
        public void SerializeMaps(ParameterStyle style, bool explode, string expected)
        {
            var value = new Dictionary<string, string> {{"a", "1"}, {"b", "2"}};
            var parameter = new OpenApiParameter
            {
                Name = "bar",
                Style = style,
                Explode = explode
            };

            var actual = SerializeParameterValue(parameter, value);
            Assert.Equal(expected, actual);
        }

        // Is it possible to write code based on the information in Parameter
        // to serialize the value like RFC6570 does!
        private string SerializeParameterValue(OpenApiParameter parameter, object value)
        {
            string output;

            switch (parameter.Style)
            {
                case ParameterStyle.Matrix: // Matrix
                    if (!parameter.Explode)
                    {
                        output = SerializeValues(
                            parameter.Name,
                            false,
                            parameter.AllowReserved,
                            value,
                            (n, v, m) => ";" + n + (string.IsNullOrEmpty(v) ? "" : "=") + v,
                            ",");
                    }
                    else
                    {
                        output = SerializeValues(
                            parameter.Name,
                            true,
                            parameter.AllowReserved,
                            value,
                            (n, v, m) => ";" + n + (string.IsNullOrEmpty(v) ? "" : "=") + v,
                            ",");
                    }
                    break;

                case ParameterStyle.Label: // Label
                    if (!parameter.Explode)
                    {
                        output = SerializeValues(
                            parameter.Name,
                            false,
                            parameter.AllowReserved,
                            value,
                            (n, v, m) => "." + (m ? n + "=" : "") + v,
                            ".");
                    }
                    else
                    {
                        output = SerializeValues(
                            parameter.Name,
                            true,
                            parameter.AllowReserved,
                            value,
                            (n, v, m) => "." + (m ? n + "=" : "") + v,
                            ".");
                    }
                    break;

                default: // Simple
                    output = SerializeValues(
                        parameter.Name,
                        false,
                        parameter.AllowReserved,
                        value,
                        (n, v, m) => v,
                        ",");
                    break;
            }

            return output;
        }

        [Theory]
        [InlineData(ParameterStyle.Label, "yo", ".yo")]
        [InlineData(ParameterStyle.Matrix, "x", ";foo=x")]
        [InlineData(ParameterStyle.Matrix, "", ";foo")]
        public void SerializePrefixedStrings(ParameterStyle style, string value, string expected)
        {
            var parameter = new OpenApiParameter
            {
                Name = "foo",
                Style = style
            };

            var actual = SerializeParameterValue(parameter, value);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("foo", "foo")]
        public void SerializeStrings(string value, string expected)
        {
            var parameter = new OpenApiParameter();

            var actual = SerializeParameterValue(parameter, value);
            Assert.Equal(expected, actual);
        }

        private string SerializeValues(
            string name,
            bool explode,
            bool allowReserved,
            object value,
            Func<string, string, bool, string> renderValue,
            string seperator)
        {
            string output = null;
            if (IsSimple(value))
            {
                var stringValue = (string)value;
                output = renderValue(name, stringValue, false);
            }
            else if (IsMap(value))
            {
                var mapValue = (IDictionary<string, string>)value;
                if (explode)
                {
                    foreach (var item in mapValue)
                    {
                        output += renderValue(item.Key, item.Value, true);
                    }
                }
                else
                {
                    output = renderValue(
                        name,
                        string.Join(
                            seperator,
                            mapValue.Select(kvp => kvp.Key + (explode ? "=" : seperator) + kvp.Value).ToList()),
                        false);
                }
            }
            else if (IsArray(value))
            {
                var arrayValue = (string[])value;
                if (explode)
                {
                    foreach (var itemValue in arrayValue)
                    {
                        output += renderValue(name, itemValue, false);
                    }
                }
                else
                {
                    output = renderValue(name, string.Join(seperator, arrayValue), false);
                }
            }

            return Encode(output, !allowReserved);
        }
    }
}