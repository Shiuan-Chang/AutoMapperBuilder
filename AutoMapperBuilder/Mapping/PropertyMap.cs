using AutoMapperBuilder.ExpressionCatagories;
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
        public Dictionary<string, ExpressionInfo> SourceExpressionInfoDict { get; } = new();

        public PropertyMap<TSource, TDestination> ForMember<TDestProp, TSourceProp>(
            Expression<Func<TDestination, TDestProp>> destSelector,
            Expression<Func<TSource, TSourceProp>> srcSelector)
        {

            var destInfo = ExpressionInspectorFactory.Analyze(destSelector.Body);
            var srcInfo = ExpressionInspectorFactory.Analyze(srcSelector.Body);
            srcInfo.Getter = CompileGetter(srcSelector);
            var destKey = destInfo.Name ?? destSelector.ToString();
            MappingDict[destKey] = srcInfo.Name ?? srcSelector.ToString();
            SourceExpressionInfoDict[destKey] = srcInfo;

            return this;
            //將目前這個 PropertyMap<TSource, TDestination> 實例本身回傳出去，讓你可以連續呼叫多個 ForMember() 方法
                        //return this:把目前這個物件傳出去，讓呼叫者可以繼續對它做事。
        }

        private Func<object, object> CompileGetter<TSource, TSourceProp>(Expression<Func<TSource, TSourceProp>> expr)
        {
            var param = Expression.Parameter(typeof(object), "obj");
            var converted = Expression.Convert(param, typeof(TSource));
            var body = Expression.Invoke(expr, converted);
            var lambda = Expression.Lambda<Func<object, object>>(Expression.Convert(body, typeof(object)), param);
            return lambda.Compile();
        }
    }
}
