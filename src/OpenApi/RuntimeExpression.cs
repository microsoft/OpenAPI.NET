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
