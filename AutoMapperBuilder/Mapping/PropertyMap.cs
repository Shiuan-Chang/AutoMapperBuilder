using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AutoMapperBuilder.Mapping
{
    // 定義source和destinaiton間不同欄位對應的關係
    public class PropertyMap<TSource, TDestination>
    {
        public Dictionary<string, string> MappingDict { get; } = new();// 同 = new Dictionary<string, string>();

        public PropertyMap<TSource, TDestination> ForMember<TDestProp, TSourceProp>(
            Expression<Func<TDestination, TDestProp>> destSelector,
            Expression<Func<TSource, TSourceProp>> srcSelector)
        {

            // ConstantExpression    //常數

            //  ParameterExpression  //變數

            //  MethodCallExpression //函式呼叫

            //  MemberExpression     //成員

            //  LambdaExpression     //Lambda表達式


            //  BinaryExpression       //二元運算式


            //  UnaryExpression 一元運算（包含轉型）


            // 做到能根據這些類型做到查找(一定要開class，要有個model紀錄哪種類型) => 先能夠做出來，再優化架構
            string destName = ((MemberExpression)destSelector.Body).Member.Name;
            string srcName = ((MemberExpression)srcSelector.Body).Member.Name;
            MappingDict[destName] = srcName;//把 srcName 加進字典 MappingDict 裡，並指定鍵為 destName。如果該鍵已存在，則更新它的值
            return this;//將目前這個 PropertyMap<TSource, TDestination> 實例本身回傳出去，讓你可以連續呼叫多個 ForMember() 方法
                        //return this:把目前這個物件傳出去，讓呼叫者可以繼續對它做事。
        }
    }
}
