// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;

namespace Microsoft.OpenApi.Attributes
{
    /// <summary>
    /// Represents the Open Api Data type metadata attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class DisplayAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayAttribute"/> class.
        /// </summary>
        /// <param name="name">The display name.</param>
        public DisplayAttribute(string name)
        {
            Name = Utils.CheckArgumentNullOrEmpty(name);
        }

        /// <summary>
        /// The display Name.
        /// </summary>
        public string Name { get; }
    }
}
