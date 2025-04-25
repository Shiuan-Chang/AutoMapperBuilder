using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AutoMapperBuilder.ExpressionCatagories
{
    //ParameterExpression  //變數:Lambda 表達式中的參數變數本身
    public class ParameterExpressionInspector : ExpressionInspector
    {
        public override bool canHandle(Expression expr) => expr is ParameterExpression;
        public override ExpressionInfo check(Expression expr)
        {
            var pe = (ParameterExpression)expr;
            return new ExpressionInfo
            {
                Name = pe.Name,
                ExpressionType = "Parameter",
                Detail = pe.Name,
                Expression = pe
            };
        }
    }
}
