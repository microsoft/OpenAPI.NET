// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OpenApi.Readers.Interface
{
    /// <summary>
    /// Interface for the log
    /// </summary>
    /// <typeparam name="TError">Type of recorded errors</typeparam>
    public interface ILog<TError>
    {
        /// <summary>
        /// List of recorded errors.
        /// </summary>
        IList<TError> Errors { get; set; }
    }
}