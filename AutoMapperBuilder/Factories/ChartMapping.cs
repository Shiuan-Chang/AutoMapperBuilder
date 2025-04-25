using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoMapperBuilder.Factories
{
    internal class CharMapping : MappingBase
    {
        public override object Map(Type srcType, object source, Type destType, Func<Type, object, Type, object> recursive)
        {
            if (source == null) return null;

            var srcProps = srcType.GetProperties();
            var dest = Activator.CreateInstance(destType);
            var destProps = destType.GetProperties().ToDictionary(p => p.Name, p => p);

            foreach (var srcProp in srcProps)
            {
                if (destProps.TryGetValue(srcProp.Name, out var destProp))
                {
                    var srcValue = srcProp.GetValue(source);
                    var destValue = recursive(srcProp.PropertyType, srcValue, destProp.PropertyType);
                    destProp.SetValue(dest, destValue);
                }
            }

            return dest;
        }
    }
}
