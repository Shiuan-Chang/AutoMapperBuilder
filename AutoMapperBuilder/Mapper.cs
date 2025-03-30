using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace AutoMapperBuilder
{
    public static class Mapper
    {
        
        public static TDestination Map<TDestination, TSource>(TSource source) where TDestination : new()
        {
            return (TDestination)RecursiveMap(typeof(TSource), source, typeof(TDestination));
        }


        public static List<TDestination> Map<TDestination, TSource>(IEnumerable<TSource> sourceList) where TDestination : new()
        {
            var result = new List<TDestination>();
            foreach (var item in sourceList)
            {
                result.Add(Map<TDestination, TSource>(item));
            }
            return result;
        }

        private static object RecursiveMap(Type srcType, object source, Type destType)
        {
            if (source == null) return null;

            // 處理 List<T> 或 T[] 的巢狀集合
            if (typeof(System.Collections.IEnumerable).IsAssignableFrom(srcType) && srcType != typeof(string))
            {
                // 取得 source 裡每個元素的型別
                var srcElementType = srcType.IsArray ? srcType.GetElementType() : srcType.GetGenericArguments().FirstOrDefault();
                var destElementType = destType.IsArray ? destType.GetElementType() : destType.GetGenericArguments().FirstOrDefault();

                if (srcElementType != null && destElementType != null)
                {
                    var listType = typeof(List<>).MakeGenericType(destElementType);
                    var destList = (System.Collections.IList)Activator.CreateInstance(listType);

                    foreach (var item in (System.Collections.IEnumerable)source)
                    {
                        var mappedItem = RecursiveMap(srcElementType, item, destElementType);
                        destList.Add(mappedItem);
                    }

                    // 如果目標是 Array，就轉成陣列
                    if (destType.IsArray)
                    {
                        var toArrayMethod = listType.GetMethod("ToArray");
                        return toArrayMethod.Invoke(destList, null);
                    }

                    return destList;
                }
            }

            var srcProps = srcType.GetProperties().ToDictionary(p => p.Name, p => p);
            var dest = Activator.CreateInstance(destType);
            var destProps = destType.GetProperties();

            foreach (var destProp in destProps)
            {
                if (!srcProps.TryGetValue(destProp.Name, out var srcProp))
                    continue;

                var srcValue = srcProp.GetValue(source);
                if (srcValue == null)
                    continue;

                var destPropType = destProp.PropertyType;

                if (destPropType.IsClass && destPropType != typeof(string))
                {
                    var nestedValue = RecursiveMap(srcProp.PropertyType, srcValue, destPropType);
                    destProp.SetValue(dest, nestedValue);
                }
                else
                {
                    var targetType = Nullable.GetUnderlyingType(destPropType) ?? destPropType;
                    var convertedValue = ConvertToType(srcValue, targetType);
                    destProp.SetValue(dest, convertedValue);
                }
            }
            return dest;
        }


        private static object ConvertToType(object value, Type targetType)
        {
            if (value == null) return null;

            Type nonNullableType = Nullable.GetUnderlyingType(targetType) ?? targetType;

                if (nonNullableType == typeof(int))
                    return Convert.ToInt32(value);
                if (nonNullableType == typeof(long))
                    return Convert.ToInt64(value);
                if (nonNullableType == typeof(float))
                    return Convert.ToSingle(value);
                if (nonNullableType == typeof(double))
                    return Convert.ToDouble(value);
                if (nonNullableType == typeof(decimal))
                    return Convert.ToDecimal(value);
                if (nonNullableType == typeof(bool))
                    return Convert.ToBoolean(value);
                if (nonNullableType == typeof(DateTime))
                    return Convert.ToDateTime(value);
                if (nonNullableType == typeof(Guid))
                    return Guid.Parse(value.ToString());
                if (nonNullableType.IsEnum)
                    return Enum.Parse(nonNullableType, value.ToString());
                if (nonNullableType == typeof(string))
                    return value.ToString();

                return Convert.ChangeType(value, nonNullableType);
            }     
        }    
}
