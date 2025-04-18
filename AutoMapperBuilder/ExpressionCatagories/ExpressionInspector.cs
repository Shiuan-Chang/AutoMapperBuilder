using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AutoMapperBuilder.ExpressionCatagories
{
    public abstract class ExpressionInspector
    {
        public abstract bool canHandle(Expression expression);
        public abstract ExpressionInfo check(Expression expression);

    }
}
