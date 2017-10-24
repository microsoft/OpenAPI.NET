using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Microsoft.OpenApi.Readers.Tests
{
    public class ParameterTests
    {

        [Theory,
            InlineData("foo","foo")]
        public void SerializeStrings(string value, string expected)
        {
            var parameter = new OpenApiParameter() {

            };

            var actual = SerializeParameterValue(parameter,value);
            Assert.Equal(expected, actual);
        }

        [Theory, 
         InlineData("://foo/bar?", "://foo/bar?"),
         InlineData("foo bar", "foo%20bar")]
        public void SerializeEscapedStrings(string value, string expected)
        {
            var parameter = new OpenApiParameter()
            {
                AllowReserved = false
            };

            var actual = SerializeParameterValue(parameter, value);
            Assert.Equal(expected, actual);
        }

        [Theory,
            InlineData("label","yo", ".yo"),
            InlineData("matrix", "x", ";foo=x"),
            InlineData("matrix", "", ";foo")]
        public void SerializePrefixedStrings(string style,string value, string expected)
        {
            var parameter = new OpenApiParameter()
            {
                Name = "foo",
                Style = style
            };
             
            var actual = SerializeParameterValue(parameter, value);
            Assert.Equal(expected, actual);
        }

        [Theory,
            InlineData("matrix",new[] { "a", "b" }, ";bar=a,b"),
            InlineData("exploded-matrix", new[] { "a", "b" }, ";bar=a;bar=b"),
            InlineData("label", new[] { "a", "b" }, ".a.b"),
            InlineData("exploded-label", new[] { "a", "b" }, ".a.b")]
        public void SerializeArrays(string style, string[] value, string expected)
        {
            var parameter = new OpenApiParameter()
            {
                Name = "bar",
                Style = style
            };

            var actual = SerializeParameterValue(parameter, value);
            Assert.Equal(expected, actual);
        }

        // format="space-delimited" type="array" explode=false bar=a b
        // format="space-delimited" type="array" explode=true bar=a bar=b



        [Theory,
            InlineData("matrix", ";bar=a,1,b,2"),
            InlineData("exploded-matrix", ";a=1;b=2"),
            InlineData("label",  ".a.1.b.2"),
            InlineData("exploded-label", ".a=1.b=2")]
        public void SerializeMaps(string style, string expected)
        {
            var value = new Dictionary<string, string> { { "a", "1" }, { "b", "2" } };
            var parameter = new OpenApiParameter()
            {
                Name = "bar",
                Style = style
            };

            var actual = SerializeParameterValue(parameter, value);
            Assert.Equal(expected, actual);
        }
        // Is it possible to write code based on the information in Parameter
        // to serialize the value like RFC6570 does!
        private string SerializeParameterValue(OpenApiParameter parameter, object value )
        {
            string output;
 
            switch (parameter.Style)
            {
                case "matrix":  // Matrix
                    output = SerializeValues(parameter.Name, false,parameter.AllowReserved,value, (n,v,m) => ";" + n + (string.IsNullOrEmpty(v) ? "" : "=") + v, ",");
                    break;
                case "exploded-matrix":  // Matrix
                    output = SerializeValues(parameter.Name, true, parameter.AllowReserved, value, (n, v, m) => ";" + n + (string.IsNullOrEmpty(v) ? "" : "=") + v, ",");
                    break;

                case "label":  // Label
                    output = SerializeValues(parameter.Name,false, parameter.AllowReserved, value, (n,v,m) => "." + ( m ? n + "=" : "") + v , ".");
                    break;

                case "exploded-label":  // Label
                    output = SerializeValues(parameter.Name, true, parameter.AllowReserved, value, (n, v, m) => "." + (m ? n + "=" : "") + v, ".");
                    break;

                default: // Simple
                    output = SerializeValues(parameter.Name, false, parameter.AllowReserved, value, (n, v,m) => v, ",");
                    break;

            }
            return output;
        }

        private string SerializeValues(string name, bool explode, bool allowReserved, object value, Func<string,string,bool,string> renderValue, string seperator)
        {
            string output = null;
            if (IsSimple(value))
            {
                var stringValue = (string)value;
                output = renderValue(name, stringValue,false);
            }
            else if (IsMap(value))
            {
                var mapValue = (IDictionary<string, string>)value;
                if (explode)
                {
                    foreach (var item in mapValue)
                    {
                        output += renderValue(item.Key, item.Value,true);
                    }
                }
                else
                {
                    output = renderValue(name, string.Join(seperator, mapValue.Select(kvp => kvp.Key + (explode ? "=" : seperator) + kvp.Value).ToList()),false);
                }
            }
            else if (IsArray(value)) {
                var arrayValue = (string[])value;
                if (explode) {
                    foreach(var itemValue in arrayValue)
                    {
                        output += renderValue(name, itemValue,false);
                    }
                }
                else
                {
                    output = renderValue(name, string.Join(seperator, arrayValue),false);
                }
            }

            return Encode(output, !allowReserved);

        }

        private bool IsMap(object value)
        {
            return value is IDictionary<string, string>;
        }

        private bool IsArray(object value)
        {
            return value is IEnumerable;
        }

        private bool IsSimple(object value)
        {
            return value is string;
        }


        private const string _UriReservedSymbols = ":/?#[]@!$&'()*+,;=";
        private const string _UriUnreservedSymbols = "-._~";
        private static string Encode(string p, bool allowReserved)
        {

            var result = new StringBuilder();
            foreach (char c in p)
            {
                if ((c >= 'A' && c <= 'z')   //Alpha
                    || (c >= '0' && c <= '9')  // Digit
                    || _UriUnreservedSymbols.IndexOf(c) != -1  // Unreserved symbols  - These should never be percent encoded
                    || (allowReserved && _UriReservedSymbols.IndexOf(c) != -1))  // Reserved symbols - should be included if requested (+)
                {
                    result.Append(c);
                }
                else
                {
                    var bytes = Encoding.UTF8.GetBytes(new[] { c });
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
            esc[1] = HexDigits[(((int)c & 240) >> 4)];
            esc[2] = HexDigits[((int)c & 15)];
            return new string(esc);
        }
        private static readonly char[] HexDigits = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };


    }
}
