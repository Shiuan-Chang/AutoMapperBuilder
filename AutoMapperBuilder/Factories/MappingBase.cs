using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoMapperBuilder.Factories
{
    public abstract class MappingBase
    {
        public abstract object Map( // 抽象可以做到不一定要實作，建造者模式通常採用
        Type srcType,
        object srcValue,
        Type destType,
        Func<Type, object, Type, object> recursiveMap);
    }
}
