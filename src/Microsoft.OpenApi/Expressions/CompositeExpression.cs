using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OpenApi.Expressions
{
    public class CompositeExpression : RuntimeExpression
    {
        private string template;

        public CompositeExpression(string expression)
        {
            template = expression;

            // Extract subexpressions and convert to RuntimeExpressions

        }

        public override string Expression => template;
    }
}
