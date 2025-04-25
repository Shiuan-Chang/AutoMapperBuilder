using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AutoMapperBuilder.ExpressionCatagories
{
    ////二元運算式
    public class BinaryExpressionInspector : ExpressionInspector
    {
        public override bool canHandle(Expression expr) => expr is BinaryExpression;
        public override ExpressionInfo check(Expression expr)
        {
            var be = (BinaryExpression)expr;
            return new ExpressionInfo
            {
                Name = be.NodeType.ToString(),
                ExpressionType = "Binary",     
                Detail = $"{be.Left} {be.NodeType} {be.Right}",
                Expression = be
            };
        }
    }
}
