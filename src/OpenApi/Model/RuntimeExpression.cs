namespace Tavis.OpenApi.Model
{
    public class RuntimeExpression
    {
        public string Expression
        {
            get { return this.expression; }
        }
        public static RuntimeExpression Load(ParseNode node)
        {
            var value = node.GetScalarValue();
            return new RuntimeExpression(value);
        }

        string expression;
        public RuntimeExpression(string expression)
        {
            this.expression = expression;
        }

    }
}
