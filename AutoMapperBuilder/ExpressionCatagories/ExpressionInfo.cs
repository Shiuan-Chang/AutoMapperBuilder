using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AutoMapperBuilder.ExpressionCatagories
{
    public class ExpressionInfo
    {
        public string Name { get; set; }
        public string ExpressionType { get; set; }     
        public string Detail { get; set; }            
        public Expression Expression { get; set; }
        public Func<object, object> Getter { get; set; }
    }
}
