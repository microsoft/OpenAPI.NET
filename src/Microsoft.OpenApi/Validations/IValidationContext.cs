// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Validations
{
    /// <summary>
    /// Constrained interface used to provide context to rule implementation
    /// </summary>
    public interface IValidationContext
    {
        /// <summary>
        /// Register an error with the validation context.
        /// </summary>
        /// <param name="error">Error to register.</param>
        void AddError(OpenApiValidatorError error);

        /// <summary>
        /// Register a warning with the validation context.
        /// </summary>
        /// <param name="warning">Warning to register.</param>
        void AddWarning(OpenApiValidatorWarning warning);

        /// <summary>
        /// Allow Rule to indicate validation error occured at a deeper context level.
        /// </summary>
        /// <param name="segment">Identifier for context</param>
        void Enter(string segment);

        /// <summary>
        /// Exit from path context level.  Enter and Exit calls should be matched.
        /// </summary>
        void Exit();

        /// <summary>
        /// Pointer to source of validation error in document
        /// </summary>
        string PathString { get; }

        /// <summary>
        /// 
        /// </summary>
        OpenApiDocument HostDocument { get; }
    }
}
