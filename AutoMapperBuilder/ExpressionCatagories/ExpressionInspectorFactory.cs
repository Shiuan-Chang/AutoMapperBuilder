using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AutoMapperBuilder.ExpressionCatagories
{
    public static class ExpressionInspectorFactory
    {
        private static readonly List<ExpressionInspector> inspectors = new()
    {
        new ConstantExpressionInspector(),
        new ParameterExpressionInspector(),
        new MethodCallExpressionInspector(),
        new MemberExpressionInspector(),
        new LambdaExpressionInspector(),
        new BinaryExpressionInspector(),
        new UnaryExpressionInspector()
    };

        public static ExpressionInfo Analyze(Expression expr)
        {
            foreach (var inspector in inspectors)
                if (inspector.canHandle(expr))
                    return inspector.check(expr);

            return new ExpressionInfo
            {
                ExpressionType = "Unknown",
                Detail = expr.ToString(),
                Expression = expr
            };
        }
    }
}
