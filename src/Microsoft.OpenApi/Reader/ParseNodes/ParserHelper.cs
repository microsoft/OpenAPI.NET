// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Globalization;

namespace Microsoft.OpenApi.Reader.ParseNodes
{
    /// <summary>
    /// Useful tools to parse data
    /// </summary>
    internal class ParserHelper
    {
        /// <summary>
        /// Parses decimal in invariant culture.
        /// If the decimal is too big or small, it returns the default value
        /// 
        /// Note: sometimes developers put Double.MaxValue or Long.MaxValue as min/max values for numbers in json schema even if their numbers are not expected to be that big/small.
        /// As we have already released the library with Decimal type for Max/Min, let's not introduce the breaking change and just fallback to Decimal.Max / Min. This should satisfy almost every scenario.
        /// We can revisit this if somebody really needs to have double or long here.
        /// </summary>
        /// <returns></returns>
        public static decimal ParseDecimalWithFallbackOnOverflow(string value, decimal defaultValue)
        {
            try
            {
                return decimal.Parse(value, NumberStyles.Float, CultureInfo.InvariantCulture);
            }
            catch (OverflowException)
            {
                return defaultValue;
            }
        }
    }
}
