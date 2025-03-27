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
                        var convertedValue = Convert.ChangeType(srcValue, targetType);
                        destProp.SetValue(dest, convertedValue);
                }
            }

            return dest;
        }


        //public static IEnumerable<TDestination> Map<TDestination, TSource>(IEnumerable<TSource> sources) where TDestination:new() // where TDestination : new() 限制 TDestination 必須有 無參數建構函式（default constructor），才能在程式中使用 new TDestination() 建立物件實體。
        //{
        //    //可以物件對物件(object)，不一定要是inumbrable
        //    // 只有欄位名稱相同可轉，類型可能不是同一個，但還是能轉(先不要管轉換欄位名字一不一樣)
        //    // 型別互轉，不要用convert直接轉，外層要先去寫判斷邏輯，再把convert放到裡面處理

        //    List<TDestination> result = new List<TDestination>();

        //    PropertyInfo[] destionationProperties = typeof(TDestination).GetProperties();
        //    PropertyInfo[] sourceProperties= typeof(TSource).GetProperties();

        //    foreach(var data in sources) // 對來源資料去做foreach，透過反射得到property後得到property的值獲取source資料
        //    {
        //        TDestination destination = new TDestination(); // 無參數建構函式（default constructor）
        //        foreach (var source in sourceProperties) 
        //        {
        //            var destinationProp = destionationProperties.FirstOrDefault(p => p.Name == source.Name && p.PropertyType == source.PropertyType);
        //            if (destinationProp != null)
        //            {

        //                destinationProp.SetValue(destination, source.GetValue(data));
        //            }
        //        }
        //        result.Add(destination);
        //    }
        //     return result;
        //}




    }
}
//List<TDestination> result = new List<TDestination>();
//PropertyInfo[] destinationProperties = typeof(TDestination).GetProperties();
//PropertyInfo[] sourceProperties = typeof(TSource).GetProperties();

//foreach (var source in sources)
//{
//    TDestination destination = new TDestination();
//    foreach (var sourceProp in sourceProperties)
//    {
//        var destProp = destinationProperties.FirstOrDefault(p => p.Name == sourceProp.Name && p.PropertyType == sourceProp.PropertyType);
//        if (destProp != null && destProp.CanWrite)
//        {
//            destProp.SetValue(destination, sourceProp.GetValue(source));
//        }
//    }
//    result.Add(destination);
//}

//return result;