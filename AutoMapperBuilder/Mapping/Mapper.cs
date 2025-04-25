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
using AutoMapperBuilder.ExpressionCatagories;

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
            var destProps = destType.GetProperties().ToDictionary(p => p.Name, p => p);

            // 取出 mapping dict（若有指定 PropertyMap）
            Dictionary<string, string> mapDict = null;
            Dictionary<string, ExpressionInfo> srcExprInfoDict = null;
            if (propertyMap != null)
            {
                var mapDictProp = propertyMap.GetType().GetProperty("MappingDict");
                var exprDictProp = propertyMap.GetType().GetProperty("SourceExpressionInfoDict");

                mapDict = (Dictionary<string, string>)mapDictProp?.GetValue(propertyMap);
                srcExprInfoDict = (Dictionary<string, ExpressionInfo>)exprDictProp?.GetValue(propertyMap);
            }

            foreach (var destProp in destProps.Values)
            {
                string srcPropName = destProp.Name;

                if (mapDict != null && mapDict.TryGetValue(destProp.Name, out var mappedName))
                    srcPropName = mappedName;

                object srcValue = null;
                Type srcValueType = null;

               
                if (srcExprInfoDict != null &&
                    srcExprInfoDict.TryGetValue(destProp.Name, out var exprInfo) &&
                    exprInfo.Getter != null)
                {
                    srcValue = exprInfo.Getter(source);
                    srcValueType = srcValue?.GetType() ?? destProp.PropertyType;
                }
                else
                {
                  
                    if (!srcProps.TryGetValue(srcPropName, out var srcProp))
                        continue;

                    srcValue = srcProp.GetValue(source);
                    srcValueType = srcProp.PropertyType;
                }

                if (srcValue == null)
                    continue;

                var mappingTag = ConvertType(destProp.PropertyType);
                string typeName = $"AutoMapperBuilder.Factories.{mappingTag}Mapping";
                Type mappingType = Type.GetType(typeName);
                MappingBase mapping = (MappingBase)Activator.CreateInstance(mappingType);

                var recursive = (Type a, object b, Type c) => RecursiveMap(a, b, c, null);

                var destValue = mapping.Map(
                    srcValueType,
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
            if (type == typeof(bool))
                return MappingTag.Boolean;
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
