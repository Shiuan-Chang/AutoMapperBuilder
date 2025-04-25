using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AutoMapperBuilder.ExpressionCatagories
{
    public class UnaryExpressionInspector : ExpressionInspector
    {
        //  UnaryExpression 一元運算（包含轉型）
        public override bool canHandle(Expression expr) => expr is UnaryExpression;

        public override ExpressionInfo check(Expression expr)
        {
            var ue = (UnaryExpression)expr;
            return new ExpressionInfo
            {
                Name = ue.NodeType.ToString(),
                ExpressionType = "Unary",
                Detail = ue.Operand.ToString(),
                Expression = ue
            };
        }
    }
}
