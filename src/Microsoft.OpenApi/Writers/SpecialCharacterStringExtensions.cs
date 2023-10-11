// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.OpenApi.Writers
{
    /// <summary>
    /// Extensions class for strings to handle special characters.
    /// </summary>
    public static class SpecialCharacterStringExtensions
    {
        // Plain style strings cannot start with indicators.
        // http://www.yaml.org/spec/1.2/spec.html#indicator//
        private static readonly HashSet<string> _yamlIndicators = new()
        {
            "-",
            "?",
            ":",
            ",",
            "{",
            "}",
            "[",
            "]",
            "&",
            "*",
            "#",
            "?",
            "|",
            "-",
            ">",
            "!",
            "%",
            "@",
            "`",
            "'",
            "\""
        };

        // Plain style strings cannot contain these character combinations.
        // http://www.yaml.org/spec/1.2/spec.html#style/flow/plain
        private static readonly HashSet<string> _yamlPlainStringForbiddenCombinations = new()
        {
            ": ",
            " #",

            // These are technically forbidden only inside flow collections, but
            // for the sake of simplicity, we will never allow them in our generated plain string.
            "[",
            "]",
            "{",
            "}",
            ","
        };

        // Plain style strings cannot end with these characters.
        // http://www.yaml.org/spec/1.2/spec.html#style/flow/plain
        private static readonly HashSet<string> _yamlPlainStringForbiddenTerminals = new()
        {
            ":"
        };

        // Double-quoted strings are needed for these non-printable control characters.
        // http://www.yaml.org/spec/1.2/spec.html#style/flow/double-quoted
        private static readonly Dictionary<char, string> _yamlControlCharacterCharReplacements = new()
        {
            {'\0', "\\0"},
            {'\x01', "\\x01"},
            {'\x02', "\\x02"},
            {'\x03', "\\x03"},
            {'\x04', "\\x04"},
            {'\x05', "\\x05"},
            {'\x06', "\\x06"},
            {'\a', "\\a"},
            {'\b', "\\b"},
            {'\t', "\\t"},
            {'\n', "\\n"},
            {'\v', "\\v"},
            {'\f', "\\f"},
            {'\r', "\\r"},
            {'\x0e', "\\x0e"},
            {'\x0f', "\\x0f"},
            {'\x10', "\\x10"},
            {'\x11', "\\x11"},
            {'\x12', "\\x12"},
            {'\x13', "\\x13"},
            {'\x14', "\\x14"},
            {'\x15', "\\x15"},
            {'\x16', "\\x16"},
            {'\x17', "\\x17"},
            {'\x18', "\\x18"},
            {'\x19', "\\x19"},
            {'\x1a', "\\x1a"},
            {'\x1b', "\\x1b"},
            {'\x1c', "\\x1c"},
            {'\x1d', "\\x1d"},
            {'\x1e', "\\x1e"},
            {'\x1f', "\\x1f"},
        };
        
        private static readonly Dictionary<string, string> _yamlControlCharacterStringReplacements = _yamlControlCharacterCharReplacements
            .ToDictionary(x => x.Key.ToString(), x => x.Value);

        /// <summary>
        /// Escapes all special characters and put the string in quotes if necessary to
        /// get a YAML-compatible string.
        /// </summary>
        internal static string GetYamlCompatibleString(this string input)
        {
            switch (input)
            {
                // If string is an empty string, wrap it in quote to ensure it is not recognized as null.
                case "":
                    return "''";
                // If string is the word null, wrap it in quote to ensure it is not recognized as empty scalar null.
                case "null":
                    return "'null'";
                // If string is the letter ~, wrap it in quote to ensure it is not recognized as empty scalar null.
                case "~":
                    return "'~'";
            }

            // If string includes a control character, wrapping in double quote is required.
            if (input.Any(c => _yamlControlCharacterCharReplacements.ContainsKey(c)))
            {
                // Replace the backslash first, so that the new backslashes created by other Replaces are not duplicated.
                input = input.Replace("\\", "\\\\");

                // Escape the double quotes.
                input = input.Replace("\"", "\\\"");

                // Escape all the control characters.
                foreach (var replacement in _yamlControlCharacterStringReplacements)
                {
                    input = input.Replace(replacement.Key, replacement.Value);
                }
                
                return $"\"{input}\"";
            }

            // If string
            // 1) includes a character forbidden in plain string,
            // 2) starts with an indicator, OR
            // 3) has trailing/leading white spaces,
            // wrap the string in single quote.
            // http://www.yaml.org/spec/1.2/spec.html#style/flow/plain
            if (_yamlPlainStringForbiddenCombinations.Any(fc => input.Contains(fc)) ||
                _yamlIndicators.Any(i => input.StartsWith(i)) ||
                _yamlPlainStringForbiddenTerminals.Any(i => input.EndsWith(i)) ||
                input.Trim() != input)
            {
                // Escape single quotes with two single quotes.
                input = input.Replace("'", "''");

                return $"'{input}'";
            }

            // If string can be mistaken as a number, c-style hexadecimal notation, a boolean, or a timestamp,
            // wrap it in quote to indicate that this is indeed a string, not a number, c-style hexadecimal notation, a boolean, or a timestamp
            if (decimal.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out _) ||
                IsHexadecimalNotation(input) ||
                bool.TryParse(input, out _) ||
                DateTime.TryParse(input, CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
            {
                return $"'{input}'";
            }

            return input;
        }

        /// <summary>
        /// Handles control characters and backslashes and adds double quotes
        /// to get JSON-compatible string.
        /// </summary>
        internal static string GetJsonCompatibleString(this string value)
        {
            if (value == null)
            {
                return "null";
            }

            // Show the control characters as strings
            // http://json.org/

            // Replace the backslash first, so that the new backslashes created by other Replaces are not duplicated.
            value = value.Replace("\\", "\\\\")
                .Replace("\b", "\\b")
                .Replace("\f", "\\f")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r")
                .Replace("\t", "\\t")
                .Replace("\"", "\\\"");

            return $"\"{value}\"";
        }

        internal static bool IsHexadecimalNotation(string input)
        {
            return input.StartsWith("0x") && int.TryParse(input.Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out _);
        }
    }
}
