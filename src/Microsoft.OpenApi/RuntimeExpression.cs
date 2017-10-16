//---------------------------------------------------------------------
// <copyright file="RuntimeExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

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
