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
        // 1. 先判斷source的類型是否是enumerable
        // 2. 若不為enumerable, 直接進入遞迴
        // 3. 若為enumerable, 將每筆資料拆開放到Map中處理進行遞迴
        // 4. return 返回的物件(遞迴的結果)
        public static class ObjectMapper
        {
            public static TDestination Map<TDestination, TSource>(TSource source) where TDestination : new()
            {
                return (TDestination)RecursiveMap(typeof(TSource), source, typeof(TDestination));
            }

            public static List<TDestination> Map<TDestination, TSource>(IEnumerable<TSource> sourceList) where TDestination : new()
            {
                return sourceList.Select(Map<TDestination, TSource>).ToList();
            }

            private static object RecursiveMap(Type srcType, object source, Type destType)
            {
                if (source == null) return null;

                if (IsEnumerableType(srcType))
                {
                    return MapEnumerable(srcType, source, destType);
                }

                return MapObjectProperties(srcType, source, destType);
            }

            private static bool IsEnumerableType(Type type)
            {
                return typeof(System.Collections.IEnumerable).IsAssignableFrom(type) && type != typeof(string);
            }

            private static object MapEnumerable(Type srcType, object source, Type destType)
            {
                var srcElementType = GetElementType(srcType);
                var destElementType = GetElementType(destType);

                if (srcElementType == null || destElementType == null)
                    return null;

                var listType = typeof(List<>).MakeGenericType(destElementType);
                var destList = (System.Collections.IList)Activator.CreateInstance(listType);

                foreach (var item in (System.Collections.IEnumerable)source)
                {
                    var mappedItem = RecursiveMap(srcElementType, item, destElementType);
                    destList.Add(mappedItem);
                }

                if (destType.IsArray)
                {
                    return listType.GetMethod("ToArray").Invoke(destList, null);
                }

                return destList;
            }

            private static Type GetElementType(Type type)
            {
                if (type.IsArray)
                    return type.GetElementType();
                if (type.IsGenericType)
                    return type.GetGenericArguments().FirstOrDefault();
                return null;
            }

            private static object MapObjectProperties(Type srcType, object source, Type destType)
            {
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

                    if (IsEnumerableType(destPropType))
                    {
                        var mappedList = RecursiveMap(srcProp.PropertyType, srcValue, destPropType);
                        destProp.SetValue(dest, mappedList);
                    }
                    else if (IsComplexType(destPropType))
                    {
                        var nestedValue = RecursiveMap(srcProp.PropertyType, srcValue, destPropType);
                        destProp.SetValue(dest, nestedValue);
                    }
                    else
                    {
                        var convertedValue = ConvertToType(srcValue, destPropType);
                        destProp.SetValue(dest, convertedValue);
                    }
                }

                return dest;
            }

            private static bool IsComplexType(Type type)
            {
                return type.IsClass && type != typeof(string);
            }

            private static object ConvertToType(object value, Type targetType)
            {
                if (value == null) return null;

                Type nonNullableType = Nullable.GetUnderlyingType(targetType) ?? targetType;

                try
                {
                    if (nonNullableType == typeof(int)) return Convert.ToInt32(value);
                    if (nonNullableType == typeof(long)) return Convert.ToInt64(value);
                    if (nonNullableType == typeof(float)) return Convert.ToSingle(value);
                    if (nonNullableType == typeof(double)) return Convert.ToDouble(value);
                    if (nonNullableType == typeof(decimal)) return Convert.ToDecimal(value);
                    if (nonNullableType == typeof(bool)) return Convert.ToBoolean(value);
                    if (nonNullableType == typeof(DateTime)) return Convert.ToDateTime(value);
                    if (nonNullableType == typeof(Guid)) return Guid.Parse(value.ToString());
                    if (nonNullableType.IsEnum) return Enum.Parse(nonNullableType, value.ToString());
                    if (nonNullableType == typeof(string)) return value.ToString();

                    return Convert.ChangeType(value, nonNullableType);
                }
                catch
                {
                    return null;
                }
            }
        }


        // 重構前
        //public static TDestination Map<TDestination, TSource>(TSource source) where TDestination : new()
        //{
        //    return (TDestination)RecursiveMap(typeof(TSource), source, typeof(TDestination));
        //}

        //public static List<TDestination> Map<TDestination, TSource>(IEnumerable<TSource> sourceList) where TDestination : new()
        //{
        //    var result = new List<TDestination>();
        //    foreach (var item in sourceList)
        //    {
        //        result.Add(Map<TDestination, TSource>(item));
        //    }
        //    return result;
        //}

        //// 如果發現欄位資料是list,list要做遞迴
        //private static object RecursiveMap(Type srcType, object source, Type destType)
        //{
        //    if (source == null) return null;

        //    // 處理 List<T> 或 T[] 的巢狀集合
        //    if (typeof(System.Collections.IEnumerable).IsAssignableFrom(srcType) && srcType != typeof(string))
        //    {
        //        // 取得 source 裡每個元素的型別
        //        var srcElementType = srcType.IsArray ? srcType.GetElementType() : srcType.GetGenericArguments().FirstOrDefault();
        //        var destElementType = destType.IsArray ? destType.GetElementType() : destType.GetGenericArguments().FirstOrDefault();

        //        if (srcElementType != null && destElementType != null)
        //        {
        //            var listType = typeof(List<>).MakeGenericType(destElementType);
        //            var destList = (System.Collections.IList)Activator.CreateInstance(listType);

        //            foreach (var item in (System.Collections.IEnumerable)source)
        //            {
        //                var mappedItem = RecursiveMap(srcElementType, item, destElementType);
        //                destList.Add(mappedItem);
        //            }

        //            // 如果目標是 Array，就轉成陣列
        //            if (destType.IsArray)
        //            {
        //                var toArrayMethod = listType.GetMethod("ToArray");
        //                return toArrayMethod.Invoke(destList, null);
        //            }

        //            return destList;
        //        }
        //    }

        //    var srcProps = srcType.GetProperties().ToDictionary(p => p.Name, p => p);
        //    var dest = Activator.CreateInstance(destType);
        //    var destProps = destType.GetProperties();

        //    foreach (var destProp in destProps)
        //    {
        //        if (!srcProps.TryGetValue(destProp.Name, out var srcProp))
        //            continue;

        //        var srcValue = srcProp.GetValue(source);
        //        if (srcValue == null)
        //            continue;

        //        var destPropType = destProp.PropertyType;

        //        if (destPropType.IsClass && destPropType != typeof(string))
        //        {
        //            var nestedValue = RecursiveMap(srcProp.PropertyType, srcValue, destPropType);
        //            destProp.SetValue(dest, nestedValue);
        //        }
        //        else
        //        {
        //            var targetType = Nullable.GetUnderlyingType(destPropType) ?? destPropType;
        //            var convertedValue = ConvertToType(srcValue, targetType);
        //            destProp.SetValue(dest, convertedValue);
        //        }
        //    }
        //    return dest;
        //}

        //private static object ConvertToType(object value, Type targetType)
        //{
        //    if (value == null) return null;

        //    Type nonNullableType = Nullable.GetUnderlyingType(targetType) ?? targetType;

        //        if (nonNullableType == typeof(int))
        //            return Convert.ToInt32(value);
        //        if (nonNullableType == typeof(long))
        //            return Convert.ToInt64(value);
        //        if (nonNullableType == typeof(float))
        //            return Convert.ToSingle(value);
        //        if (nonNullableType == typeof(double))
        //            return Convert.ToDouble(value);
        //        if (nonNullableType == typeof(decimal))
        //            return Convert.ToDecimal(value);
        //        if (nonNullableType == typeof(bool))
        //            return Convert.ToBoolean(value);
        //        if (nonNullableType == typeof(DateTime))
        //            return Convert.ToDateTime(value);
        //        if (nonNullableType == typeof(Guid))
        //            return Guid.Parse(value.ToString());
        //        if (nonNullableType.IsEnum)
        //            return Enum.Parse(nonNullableType, value.ToString());
        //        if (nonNullableType == typeof(string))
        //            return value.ToString();

        //        return Convert.ChangeType(value, nonNullableType);
        //    }     
    }
}
