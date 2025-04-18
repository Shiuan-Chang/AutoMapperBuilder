using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AutoMapperBuilder.ExpressionCatagories
{
    public class ConstantExpressionInspector : ExpressionInspector
    {
        public override bool canHandle(Expression expression) => expression is ConstantExpression;
        public override ExpressionInfo check(Expression expression)
        {
            var ce = (ConstantExpression)expression;
            return new ExpressionInfo { expressionType = "Constant", detail = ce.Value?.ToString() ?? "null", expression = ce };
        }
    }
}
