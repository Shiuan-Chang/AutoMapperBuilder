using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using AutoMapperBuilder.Interface;
using AutoMapperBuilder.Factories;
using AutoMapperBuilder.Enums;
using AutoMapperBuilder.Factories;

namespace AutoMapperBuilder.Mapping
{
    public class Mapper 
    {

        // 考慮Destination只有一個欄位時
        public static TDestination Map<TDestination, TSource>(TSource source) where TDestination : new()
        {
            return (TDestination)RecursiveMap(typeof(TSource), source, typeof(TDestination));
        }

        // 考慮Destination有多個欄位時
        public static List<TDestination> Map<TDestination, TSource>(IEnumerable<TSource> sourceList) where TDestination : new()
        {
            var result = new List<TDestination>();
            foreach (var item in sourceList)
            {
                result.Add(Map<TDestination, TSource>(item));
            }
            return result;
        }

        // 如果發現欄位資料是list,list要做遞迴
        private static object RecursiveMap(Type srcType, object source, Type destType)
        {
            if (source == null) return null;
            var srcProps = srcType.GetProperties().ToDictionary(p => p.Name, p => p);//p => p.Name：把屬性名稱當成Key，p => p：整個 PropertyInfo 當成Value
            var dest = Activator.CreateInstance(destType);
            var destPropInfos = destType.GetProperties().ToList();
            foreach (var destProp in destPropInfos)
            {
                if (!srcProps.TryGetValue(destProp.Name, out var srcProp))
                    continue;
                var srcValue = srcProp.GetValue(source);
                if (srcValue == null)
                    continue;
                // 取得要轉換的目標type tag
                MappingTag mappingTag = ConvertType(destProp.PropertyType);
                // 依照type tag取得Mapping實作
                string typeName = $"AutoMapperBuilder.Factories.{mappingTag}Mapping";
                Type mappingType = Type.GetType(typeName);

                if (mappingType == null)
                    throw new InvalidOperationException($"找不到對應的 Mapping 類別: {typeName}");

                MappingBase mapping = (MappingBase)Activator.CreateInstance(mappingType);
                // 將src的值轉為dest的型別
                object destValue = mapping.Map(srcProp.PropertyType, srcValue, destProp.PropertyType, RecursiveMap);
                // 設定dest的值
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
