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

            var srcProps = srcType.GetProperties().ToDictionary(p => p.Name, p => p);//得到的屬性加到dic中，key值為屬性名稱，value值為屬性類型，如："Name": PropertyInfo,"Age": PropertyInfo,
            var dest = Activator.CreateInstance(destType); //等同於new一個dest出來，會含有dest裡面所有的預設欄位
            var destProps = destType.GetProperties();

            foreach (var destProp in destProps)
            {
                // src找不到對應屬性名稱，就跳過
                if (!srcProps.TryGetValue(destProp.Name, out var srcProp))
                    continue;

                // src中取出對應屬性的值，如果是 null 就跳過
                var srcValue = srcProp.GetValue(source);
                if (srcValue == null)
                    continue;

                var destPropType = destProp.PropertyType;

                // 當屬性是「巢狀類別」時（如Destination 和 Source兩個裡面設有多個變數），就遞迴地去複製它裡面的內容。
                if (destPropType.IsClass && destPropType != typeof(string)) 
                {
                    var nestedValue = RecursiveMap(srcProp.PropertyType, srcValue, destPropType);
                    destProp.SetValue(dest, nestedValue);
                }
                else
                {
                    // 處理 Nullable<T> 類型（例如 int?）時會先還原成原始型別。
                    var targetType = Nullable.GetUnderlyingType(destPropType) ?? destPropType;
                    // 再透過 Convert.ChangeType 轉換型別（例如從 int 轉 double，亦即把source的值轉變成dest要的型別
                    var convertedValue = ConvertToType(srcValue, targetType);
                    // 設定到目標物件的屬性中
                    destProp.SetValue(dest, convertedValue);
                }
            }
            return dest;//以destyp為準創立的instance
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
