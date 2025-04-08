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
        public Dictionary<string, string> MappingDict { get; } = new();

        public PropertyMap<TSource, TDestination> ForMember<TDestProp, TSourceProp>(
            Expression<Func<TDestination, TDestProp>> destSelector,
            Expression<Func<TSource, TSourceProp>> srcSelector)
        {
            string destName = ((MemberExpression)destSelector.Body).Member.Name;
            string srcName = ((MemberExpression)srcSelector.Body).Member.Name;
            MappingDict[destName] = srcName;
            return this;
        }
    }
}
