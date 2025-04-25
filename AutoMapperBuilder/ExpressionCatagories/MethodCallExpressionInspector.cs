using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AutoMapperBuilder.ExpressionCatagories
{
    //  MethodCallExpression //函式呼叫
    public class MethodCallExpressionInspector : ExpressionInspector
    {
        public override bool canHandle(Expression expr) => expr is MethodCallExpression;

        public override ExpressionInfo check(Expression expr)
        {
            var mc = (MethodCallExpression)expr;
            return new ExpressionInfo
            {
                Name = mc.Method.Name, // 可拿 method 名稱做識別
                ExpressionType = "MethodCall",
                Detail = mc.Method.Name,
                Expression = mc
            };
        }
    }
}
