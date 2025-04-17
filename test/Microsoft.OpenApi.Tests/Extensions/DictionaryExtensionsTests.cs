using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Extensions;
using Xunit;

namespace Microsoft.OpenApi.Tests.Extensions
{
    public class DictionaryExtensionsTests
    {
        [Fact]
        public void ShouldSortStringIntDictionaryInAscendingOrder()
        {
            var dict = new Dictionary<string, int> { { "b", 2 }, { "a", 1 } };
            var result = dict.Sort();
            Assert.Equal(["a", "b"], result.Keys);
        }

        [Fact]
        public void ShouldReturnEmptyDictionaryWhenSourceIsEmpty()
        {
            var dict = new Dictionary<string, int>();
            var result = dict.Sort();
            Assert.Empty(result);
        }

        [Fact]
        public void ShouldKeepOrderWhenDictionaryIsAlreadySorted()
        {
            var dict = new Dictionary<string, int> { { "a", 1 }, { "b", 2 } };
            var result = dict.Sort();
            Assert.Equal(["a", "b"], result.Keys);
        }

        [Fact]
        public void ShouldSortNumericKeysNaturally()
        {
            var dict = new Dictionary<int, string> { { 10, "a" }, { 1, "b" } };
            var result = dict.Sort();
            Assert.Equal([1, 10], result.Keys);
        }

        [Fact]
        public void ShouldSortDateTimeKeysInAscendingOrder()
        {
            var now = DateTime.Now;
            var later = now.AddHours(1);

            var dict = new Dictionary<DateTime, string>
            {
                [later] = "future",
                [now] = "present"
            };

            var result = dict.Sort();
            Assert.Equal([now, later], result.Keys);
        }

        [Fact]
        public void ShouldSortWithCustomDescendingComparer()
        {
            var dict = new Dictionary<string, int> { { "a", 1 }, { "b", 2 } };
            var result = dict.Sort(Comparer<string>.Create((x, y) => y.CompareTo(x)));
            Assert.Equal(["b", "a"], result.Keys);
        }

        [Fact]
        public void ShouldSortDictionaryWithComplexValueTypes()
        {
            var dict = new Dictionary<string, ISet<string>>
        {
            { "z", new HashSet<string> { "value1" } },
            { "a", new HashSet<string> { "value2" } }
        };

            var result = dict.Sort();
            Assert.Equal(["a", "z"], result.Keys);
            Assert.Equal(new HashSet<string> { "value2" }, result["a"]);
        }

        [Fact]
        public void ShouldSortDictionaryWithNullValues()
        {
            var dict = new Dictionary<string, string>
        {
            { "b", null },
            { "a", "value" }
        };

            var result = dict.Sort();
            Assert.Equal(["a", "b"], result.Keys);
            Assert.Null(result["b"]);
        }

        [Fact]
        public void ShouldSortDictionaryOfDictionariesByOuterKey()
        {
            var dict = new Dictionary<string, Dictionary<string, string>>
            {
                ["z"] = new Dictionary<string, string> { { "x", "1" } },
                ["a"] = new Dictionary<string, string> { { "y", "2" } }
            };

            var result = dict.Sort();
            Assert.Equal(["a", "z"], result.Keys);
            Assert.Equal("2", result["a"]["y"]);
        }
    }
}
