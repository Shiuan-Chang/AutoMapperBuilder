using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoMapperBuilder.Factories
{
    internal class EnumerableMapping : MappingBase
    {
        public override object Map(Type srcType, object source, Type destType, Func<Type, object, Type, object> mappingFunc)
        {
            if (!(source is IEnumerable sourceEnumerable))
                throw new Exception("source 需為 IEnumerable 或陣列");

            // 判斷目標集合的元素型別
            Type destElementType;
            if (destType.IsArray)
                destElementType = destType.GetElementType();
            else if (destType.IsGenericType)
                destElementType = destType.GetGenericArguments().FirstOrDefault();
            else
                throw new Exception("dest 需為 IEnumerable 或陣列");

            // 用T的型別取得List<T>的型別
            var destListType = typeof(List<>).MakeGenericType(destElementType);
            // 建立新的 dest List
            var destList = (IList)Activator.CreateInstance(destListType);

            foreach (var src in sourceEnumerable)
            {
                var mappedClass = mappingFunc(src.GetType(), src, destElementType);
                destList.Add(mappedClass);
            }

            if (destType.IsArray)
            {
                var array = Array.CreateInstance(destElementType, destList.Count);
                destList.CopyTo(array, 0);
                return array;
            }

            return destList;
        }
    }
}
