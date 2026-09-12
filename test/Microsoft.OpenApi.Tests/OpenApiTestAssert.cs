// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;
using Xunit;

namespace Microsoft.OpenApi.Tests;

public static class OpenApiTestAssert
{
    public static void Equivalent<T>(T expected, T actual, params string[] excludedMemberNames)
    {
        Equivalent(expected, actual, path => excludedMemberNames.Contains(path.MemberName));
    }

    public static void Equivalent<T>(T expected, T actual, Func<EquivalencyPath, bool> exclude)
    {
        Compare(expected, actual, EquivalencyPath.Root, exclude, new HashSet<ReferencePair>());
    }

    public readonly record struct EquivalencyPath(string Value, string MemberName)
    {
        public static EquivalencyPath Root { get; } = new(string.Empty, string.Empty);

        public EquivalencyPath Member(string memberName)
        {
            var value = string.IsNullOrEmpty(Value) ? memberName : $"{Value}.{memberName}";
            return new(value, memberName);
        }

        public EquivalencyPath Index(object index)
        {
            return new($"{Value}[{index}]", MemberName);
        }
    }

    private static void Compare(object expected, object actual, EquivalencyPath path, Func<EquivalencyPath, bool> exclude, HashSet<ReferencePair> visited)
    {
        if (exclude(path))
        {
            return;
        }

        if (expected is null || actual is null)
        {
            Assert.Equal(expected, actual);
            return;
        }

        if (expected is JsonNode || actual is JsonNode)
        {
            var expectedNode = Assert.IsAssignableFrom<JsonNode>(expected);
            var actualNode = Assert.IsAssignableFrom<JsonNode>(actual);
            Assert.True(JsonNode.DeepEquals(expectedNode, actualNode), $"Expected JSON node at '{path.Value}' to be equivalent.");
            return;
        }

        var expectedType = expected.GetType();
        if (IsSimpleType(expectedType))
        {
            Assert.Equal(expected, actual);
            return;
        }

        if (!expectedType.IsValueType)
        {
            var pair = new ReferencePair(expected, actual);
            if (!visited.Add(pair))
            {
                return;
            }
        }

        if (expected is IDictionary expectedDictionary && actual is IDictionary actualDictionary)
        {
            Assert.Equal(expectedDictionary.Count, actualDictionary.Count);
            foreach (DictionaryEntry expectedEntry in expectedDictionary)
            {
                Assert.True(actualDictionary.Contains(expectedEntry.Key), $"Expected dictionary at '{path.Value}' to contain key '{expectedEntry.Key}'.");
                Compare(expectedEntry.Value, actualDictionary[expectedEntry.Key], path.Index(expectedEntry.Key), exclude, visited);
            }

            return;
        }

        if (expected is IEnumerable expectedEnumerable && actual is IEnumerable actualEnumerable && expected is not string)
        {
            var expectedItems = expectedEnumerable.Cast<object>().ToList();
            var actualItems = actualEnumerable.Cast<object>().ToList();

            Assert.Equal(expectedItems.Count, actualItems.Count);
            for (var index = 0; index < expectedItems.Count; index++)
            {
                Compare(expectedItems[index], actualItems[index], path.Index(index), exclude, visited);
            }

            return;
        }

        foreach (var property in expectedType.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(static p => p.GetIndexParameters().Length == 0))
        {
            var propertyPath = path.Member(property.Name);
            if (exclude(propertyPath))
            {
                continue;
            }

            var actualProperty = actual.GetType().GetProperty(property.Name, BindingFlags.Instance | BindingFlags.Public);
            Assert.NotNull(actualProperty);
            Compare(property.GetValue(expected), actualProperty.GetValue(actual), propertyPath, exclude, visited);
        }

        foreach (var field in expectedType.GetFields(BindingFlags.Instance | BindingFlags.Public))
        {
            var fieldPath = path.Member(field.Name);
            if (exclude(fieldPath))
            {
                continue;
            }

            var actualField = actual.GetType().GetField(field.Name, BindingFlags.Instance | BindingFlags.Public);
            Assert.NotNull(actualField);
            Compare(field.GetValue(expected), actualField.GetValue(actual), fieldPath, exclude, visited);
        }
    }

    private static bool IsSimpleType(Type type)
    {
        return type.IsPrimitive
            || type.IsEnum
            || type == typeof(string)
            || type == typeof(decimal)
            || type == typeof(DateTime)
            || type == typeof(DateTimeOffset)
            || type == typeof(TimeSpan)
            || type == typeof(Uri)
            || type == typeof(Guid);
    }

    private readonly record struct ReferencePair(object Expected, object Actual)
    {
        public bool Equals(ReferencePair other)
        {
            return ReferenceEquals(Expected, other.Expected) && ReferenceEquals(Actual, other.Actual);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(RuntimeHelpers.GetHashCode(Expected), RuntimeHelpers.GetHashCode(Actual));
        }
    }
}
