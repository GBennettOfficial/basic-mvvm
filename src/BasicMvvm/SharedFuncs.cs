using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace BasicMvvm
{
    public static class SharedFuncs
    {
        public static string GetPropertyNameFromFunc<TObj, TProp>(Expression<Func<TObj, TProp>> expression)
        {
            var body = expression.Body is UnaryExpression u ? u.Operand : expression.Body;
            if (ReferenceEquals(body, expression.Parameters[0]))
                return "";
            return body is MemberExpression m
                ? m.Member.Name
                : throw new ArgumentException("Expression must be a property access, e.g. x => x.Name");
        }
    }
}
