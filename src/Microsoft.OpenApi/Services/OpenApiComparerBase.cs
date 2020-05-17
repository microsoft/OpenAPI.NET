// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Services
{
    /// <summary>
    /// Defines behavior for comparing parts of <see cref="OpenApiDocument"/> class.
    /// </summary>
    /// <typeparam name="T">Type of class to compare.</typeparam>
    public abstract class OpenApiComparerBase<T>
    {
        /// <summary>
        /// Validates a fragment of <see cref="OpenApiDocument"/>.
        /// </summary>
        /// <param name="sourceFragment">The source fragment.</param>
        /// <param name="targetFragment">The target fragment.</param>
        /// <param name="comparisonContext">Context under which to compare fragment.</param>
        public abstract void Compare(T sourceFragment, T targetFragment, ComparisonContext comparisonContext);

        /// <summary>
        /// Compares two string object.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="target">The target string.</param>
        /// <param name="comparisonContext">The context under which to compare the objects.</param>
        internal void Compare(string source, string target, ComparisonContext comparisonContext)
        {
            if (string.IsNullOrWhiteSpace(source) && string.IsNullOrWhiteSpace(target))
            {
                return;
            }

            if (string.Compare(source, target, StringComparison.CurrentCultureIgnoreCase) != 0)
            {
                comparisonContext.AddOpenApiDifference(new OpenApiDifference
                {
                    OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                    OpenApiComparedElementType = typeof(string),
                    SourceValue = source,
                    TargetValue = target,
                    Pointer = comparisonContext.PathString
                });
            }
        }

        /// <summary>
        /// Compares two Uri object.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <param name="comparisonContext">The context under which to compare the objects.</param>
        internal void Compare(Uri source, Uri target, ComparisonContext comparisonContext)
        {
            if (source == null && target == null)
            {
                return;
            }

            if (source != target)
            {
                comparisonContext.AddOpenApiDifference(new OpenApiDifference
                {
                    OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                    OpenApiComparedElementType = typeof(Uri),
                    SourceValue = source,
                    TargetValue = target,
                    Pointer = comparisonContext.PathString
                });
            }
        }

        /// <summary>
        /// Compares two boolean object.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <param name="comparisonContext">The context under which to compare the objects.</param>
        internal void Compare(bool? source, bool? target, ComparisonContext comparisonContext)
        {
            if (source == null && target == null)
            {
                return;
            }

            if (source != target)
            {
                comparisonContext.AddOpenApiDifference(new OpenApiDifference
                {
                    OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                    OpenApiComparedElementType = typeof(bool?),
                    SourceValue = source,
                    TargetValue = target,
                    Pointer = comparisonContext.PathString
                });
            }
        }

        /// <summary>
        /// Compares two decimal object.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <param name="comparisonContext">The context under which to compare the objects.</param>
        internal void Compare(decimal? source, decimal? target, ComparisonContext comparisonContext)
        {
            if (source == null && target == null)
            {
                return;
            }

            if (source != target)
            {
                comparisonContext.AddOpenApiDifference(new OpenApiDifference
                {
                    OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                    OpenApiComparedElementType = typeof(decimal?),
                    SourceValue = source,
                    TargetValue = target,
                    Pointer = comparisonContext.PathString
                });
            }
        }

        /// <summary>
        /// Compares Enum.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <param name="comparisonContext">The context under which to compare the objects.</param>
        internal void Compare<TEnum>(Enum source, Enum target, ComparisonContext comparisonContext)
        {
            if (source == null && target == null)
            {
                return;
            }

            if (source == null || target == null)
            {
                comparisonContext.AddOpenApiDifference(new OpenApiDifference
                {
                    OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                    OpenApiComparedElementType = typeof(TEnum),
                    SourceValue = source,
                    TargetValue = target,
                    Pointer = comparisonContext.PathString
                });

                return;
            }

            if (!source.Equals(target))
            {
                comparisonContext.AddOpenApiDifference(new OpenApiDifference
                {
                    OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                    OpenApiComparedElementType = typeof(TEnum),
                    SourceValue = source,
                    TargetValue = target,
                    Pointer = comparisonContext.PathString
                });
            }
        }

        /// <summary>
        /// Compares <see cref="IDictionary{TKey,TValue}"/> where TKey is <see cref="string"/> and TValue is
        /// <see cref="string"/>.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <param name="comparisonContext">The context under which to compare the objects.</param>
        internal void Compare(IDictionary<string, string> source, IDictionary<string, string> target,
            ComparisonContext comparisonContext)
        {
            if (source == null && target == null)
            {
                return;
            }

            if (source == null || target == null)
            {
                comparisonContext.AddOpenApiDifference(
                    new OpenApiDifference
                    {
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Update,
                        SourceValue = source,
                        TargetValue = target,
                        OpenApiComparedElementType = typeof(IDictionary<string, T>),
                        Pointer = comparisonContext.PathString
                    });

                return;
            }

            var newKeysInTarget = target.Keys.Except(source.Keys).ToList();

            foreach (var newKeyInTarget in newKeysInTarget)
            {
                WalkAndAddOpenApiDifference(
                    comparisonContext,
                    newKeyInTarget,
                    new OpenApiDifference
                    {
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Add,
                        TargetValue = target[newKeyInTarget],
                        OpenApiComparedElementType = typeof(string)
                    });
            }

            var removedKeysFromSource = source.Keys.Except(target.Keys).ToList();

            foreach (var removedKeyFromSource in removedKeysFromSource)
            {
                WalkAndAddOpenApiDifference(
                    comparisonContext,
                    removedKeyFromSource,
                    new OpenApiDifference
                    {
                        OpenApiDifferenceOperation = OpenApiDifferenceOperation.Remove,
                        SourceValue = source[removedKeyFromSource],
                        OpenApiComparedElementType = typeof(string)
                    });
            }
        }

        /// <summary>
        /// Adds a segment to the context path to enable pointing to the current location in the document.
        /// </summary>
        /// <param name="comparisonContext">The context under which to compare the objects.</param>
        /// <param name="segment">An identifier for the segment.</param>
        /// <param name="openApiDifference">The open api difference to add.</param>
        internal void WalkAndAddOpenApiDifference(
            ComparisonContext comparisonContext,
            string segment,
            OpenApiDifference openApiDifference)
        {
            comparisonContext.Enter(segment.Replace("~", "~0").Replace("/", "~1"));
            openApiDifference.Pointer = comparisonContext.PathString;
            comparisonContext.AddOpenApiDifference(openApiDifference);
            comparisonContext.Exit();
        }

        /// <summary>
        /// Adds a segment to the context path to enable pointing to the current location in the document.
        /// </summary>
        /// <param name="comparisonContext">The context under which to compare the objects.</param>
        /// <param name="segment">An identifier for the segment.</param>
        /// <param name="compare">An action that compares objects within the context.</param>
        protected virtual void WalkAndCompare(
            ComparisonContext comparisonContext,
            string segment,
            Action compare)
        {
            comparisonContext.Enter(segment.Replace("~", "~0").Replace("/", "~1"));
            compare();
            comparisonContext.Exit();
        }
    }
}
