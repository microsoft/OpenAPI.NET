// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using FluentAssertions;

namespace Microsoft.OpenApi.Readers.Tests
{
    /// <summary>
    /// Fixture containing default settings for external libraries.
    /// </summary>
    public class DefaultSettingsFixture
    {
        /// <summary>
        /// Initializes an intance of <see cref="DefaultSettingsFixture"/>.
        /// </summary>
        public DefaultSettingsFixture()
        {
            // We need RespectingRuntimeTypes() to ensure equivalence test works property,
            // given that there are multiple types that can be used for the declared type OpenApiAny.
            // Without this option, properties specific to those types would not be compared.
            AssertionOptions.AssertEquivalencyUsing(
                o => o.AllowingInfiniteRecursion().RespectingRuntimeTypes());
        }
    }
}