using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using AutoMapperBuilder.Factories;
using AutoMapperBuilder.Enums;
using AutoMapperBuilder.Factories;
using AutoMapperBuilder.Mapping;
using System.Linq.Expressions;

namespace AutoMapperBuilder.Mapping
{
    public class Mapper
    {
        // 考慮Destination只有一個欄位時
        public static TDestination Map<TDestination, TSource>(TSource source, PropertyMap<TSource, TDestination> propertyMap = null) where TDestination : new()
        {
            return (TDestination)RecursiveMap(typeof(TSource), source, typeof(TDestination), propertyMap);
        }

        // 考慮Destination有多個欄位時
        public static List<TDestination> Map<TDestination, TSource>(IEnumerable<TSource> sourceList, PropertyMap<TSource, TDestination> propertyMap) where TDestination : new()
        {
            var result = new List<TDestination>();
            foreach (var item in sourceList)
            {
                result.Add(Map<TDestination, TSource>(item, propertyMap));
            }
            return result;
        }

        // 如果發現欄位資料是list,list要做遞迴
        private static object RecursiveMap(Type srcType, object source, Type destType, object propertyMap)
        {
            if (source == null) return null;

            var srcProps = srcType.GetProperties().ToDictionary(p => p.Name, p => p);
            var dest = Activator.CreateInstance(destType);
            var destProps = destType.GetProperties();

            Dictionary<string, string> mapDict = null;
            if (propertyMap != null)
            {
                var field = propertyMap.GetType().GetProperty("MappingDict");
                mapDict = (Dictionary<string, string>)field.GetValue(propertyMap);
            }

            foreach (var destProp in destProps)
            {
                string srcPropName = destProp.Name;

                if (mapDict != null && mapDict.TryGetValue(destProp.Name, out var mappedName))
                {
                    srcPropName = mappedName;
                }

                if (!srcProps.TryGetValue(srcPropName, out var srcProp))
                    continue;

                var srcValue = srcProp.GetValue(source);
                if (srcValue == null)
                    continue;

                var mappingTag = ConvertType(destProp.PropertyType);
                string typeName = $"AutoMapperBuilder.Factories.{mappingTag}Mapping";
                Type mappingType = Type.GetType(typeName);
                MappingBase mapping = (MappingBase)Activator.CreateInstance(mappingType);

                var recursive = (Type a, object b, Type c) => RecursiveMap(a, b, c, propertyMap);

                var destValue = mapping.Map(
                    srcProp.PropertyType,
                    srcValue,
                    destProp.PropertyType,
                    recursive
                );

                destProp.SetValue(dest, destValue);
            }

            return dest;
        }

        static MappingTag ConvertType(Type type)
        {
            if (type.IsEnum)
                return MappingTag.Enum;
            if (typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string))
                return MappingTag.Enumerable;
            if (type.IsClass && type != typeof(string))
                return MappingTag.Class;

            if (Enum.TryParse(typeof(MappingTag), type.Name, out var result))
                return (MappingTag)result;

            throw new ArgumentOutOfRangeException($"無法將型別 {type.FullName} 映射至 MappingTag");
        }
    }
}
