using AutoMapperBuilder.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapperBuilder.Interface;

namespace AutoMapperBuilder.Factories
{
    internal class MappingOption
    {
            public static MappingBase CreateMapping(MappingTag mappingTag)
            {
                switch (mappingTag)
                {
                    case MappingTag.Int32:
                        return new Int32Mapping();
                    case MappingTag.Int64:
                        return new LongMapping();
                    case MappingTag.Single:
                        return new FloatMapping();
                    case MappingTag.Double:
                        return new DoubleMapping();
                    case MappingTag.Char:
                        return new CharMapping();
                    case MappingTag.String:
                        return new StringMapping();
                    case MappingTag.Boolean:
                        return new BoolMapping();
                    case MappingTag.Enum:
                        return new EnumMapping();
                    case MappingTag.Enumerable:
                        return new EnumerableMapping();
                    case MappingTag.Class:
                        return new ClassMapping();
                    default:
                        throw new ArgumentOutOfRangeException($"不支援的轉換類型 : {mappingTag}");
                }
            }
        
    }
}
