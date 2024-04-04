// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

namespace Microsoft.OpenApi.Reader.V2
{
    /// <summary>
    /// Strings to be used as keys for the temporary storage.
    /// </summary>
    internal static class TempStorageKeys
    {
        public const string ResponseSchema = "responseSchema";
        public const string BodyParameter = "bodyParameter";
        public const string FormParameters = "formParameters";
        public const string OperationProduces = "operationProduces";
        public const string ResponseProducesSet = "responseProducesSet";
        public const string OperationConsumes = "operationConsumes";
        public const string GlobalConsumes = "globalConsumes";
        public const string GlobalProduces = "globalProduces";
        public const string ParameterIsBodyOrFormData = "parameterIsBodyOrFormData";
        public const string Examples = "examples";
    }
}
