using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AutoMapperBuilder.ExpressionCatagories
{
    //  MemberExpression     //成員
    public class MemberExpressionInspector : ExpressionInspector
    {
        public override bool canHandle(Expression expr) => expr is MemberExpression;
        public override ExpressionInfo check(Expression expr)
        {
            var me = (MemberExpression)expr;
            return new ExpressionInfo {
                Name = me.Member.Name,
                ExpressionType = "Member",
                Detail = me.ToString(),
                Expression = me
            };
        }
    }
}
