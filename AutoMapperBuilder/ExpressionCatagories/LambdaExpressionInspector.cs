using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AutoMapperBuilder.ExpressionCatagories
{
    //  LambdaExpression     //Lambda表達式
    public class LambdaExpressionInspector : ExpressionInspector
    {
        public override bool canHandle(Expression expr) => expr is LambdaExpression;

        public override ExpressionInfo check(Expression expr)
        {
            var le = (LambdaExpression)expr;
            return new ExpressionInfo
            {
                Name = le.Parameters.FirstOrDefault()?.Name ?? "lambda",
                ExpressionType = "Lambda",
                Detail = le.Body.ToString(),
                Expression = le
            };
        }
    }
}
