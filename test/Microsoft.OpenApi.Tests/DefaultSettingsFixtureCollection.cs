// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using Xunit;

namespace Microsoft.OpenApi.Tests
{
    /// <summary>
    /// Collection dummy class for <see cref="DefaultSettingsFixture"/>.
    /// </summary>
    /// <remarks>
    /// This class is needed in xUnit framework to define collection name
    /// to be used in unit test classes.
    /// </remarks>
    [CollectionDefinition("DefaultSettings")]
    public class DefaultSettingsFixtureCollection : ICollectionFixture<DefaultSettingsFixture>
    {
    }
}