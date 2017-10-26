// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.OpenApi
{
    public class RuntimeExpression
    {
        public string Expression
        {
            get { return this.expression; }
        }


        string expression;
        public RuntimeExpression(string expression)
        {
            this.expression = expression;
        }
    }
}
