using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoMapperBuilder.Factories
{
    public abstract class MappingBase
    {
        public abstract object Map(
        Type srcType,
        object srcValue,
        Type destType,
        Func<Type, object, Type, object> recursiveMap);
    }
}
