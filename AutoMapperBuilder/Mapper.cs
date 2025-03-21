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
        public static IEnumerable<TDestination> Map<TDestination, TSource>(IEnumerable<TSource> sources) where TDestination:new() // where TDestination : new() 限制 TDestination 必須有 無參數建構函式（default constructor），才能在程式中使用 new TDestination() 建立物件實體。
        {
            List<TDestination> result = new List<TDestination>();

            PropertyInfo[] destionationProperties = typeof(TDestination).GetProperties();
            PropertyInfo[] sourceProperties= typeof(TSource).GetProperties();

            foreach(var data in sources) // 對來源資料去做foreach，透過反射得到property後得到property的值獲取source資料
            {
                TDestination destination = new TDestination();
                foreach (var source in sourceProperties) 
                {
                    var destinationProp = destionationProperties.FirstOrDefault(p => p.Name == source.Name && p.PropertyType == source.PropertyType);
                    if (destinationProp != null)
                    {
                        destinationProp.SetValue(destination, source.GetValue(data));
                    }
                }
                result.Add(destination);
            }
             return result;
        }
        
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