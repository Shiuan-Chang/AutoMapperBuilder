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
        // ConstantExpression //常數，查找固定不變的值
        public override bool canHandle(Expression expr) => expr is ConstantExpression;

        public override ExpressionInfo check(Expression expr)
        {
            var ce = (ConstantExpression)expr;
            return new ExpressionInfo
            {
                Name = ce.Value?.ToString() ?? "null",
                ExpressionType = "Constant",
                Detail = ce.Value?.ToString() ?? "null",
                Expression = ce
            };
        }
    }
}
