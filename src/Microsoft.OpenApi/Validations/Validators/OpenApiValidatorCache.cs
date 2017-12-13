// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OpenApi.Validations.Validators
{
    internal static class OpenApiValidatorCache
    {
        private static IDictionary<Type, IValidator> _elementVisitor = new Dictionary<Type, IValidator>();

        public static IValidator GetValidator(Type elementType)
        {
            IValidator validator;
            if (_elementVisitor.TryGetValue(elementType, out validator))
            {
                return validator;
            }

            var attribute = elementType.GetCustomAttributes(typeof(OpenApiValidatorAttribute), false).FirstOrDefault();
            if (attribute == null)
            {
                return null;
            }

            OpenApiValidatorAttribute validateAttr = attribute as OpenApiValidatorAttribute;
            if (validateAttr == null)
            {
                return null;
            }

            object instance = Activator.CreateInstance(validateAttr.ValidatorType);
            validator = instance as IValidator;
            if (validator != null)
            {
                _elementVisitor[validateAttr.ValidatorType] = validator;
            }

            return validator;
        }
    }
}
