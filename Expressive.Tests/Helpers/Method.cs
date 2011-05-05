using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Expressive.Tests.Helpers {
    public static class Method {
        public static string Name<TDeclaring>(Expression<Action<TDeclaring>> call) {
            return Method.Get(call).Name;
        }

        public static MethodInfo Get(Expression<Action> call) {
            return ((MethodCallExpression)call.Body).Method;
        }

        public static MethodInfo Get<TDeclaring>(Expression<Action<TDeclaring>> call) {
            var method = ((MethodCallExpression)call.Body).Method;
            if (typeof(TDeclaring).IsValueType && method.DeclaringType == typeof(object)) {
                // weird stuff :)
                method = typeof(TDeclaring).GetMethod(
                    method.Name,
                    method.GetParameters().Select(p => p.ParameterType).ToArray()
                );
                if (method == null)
                    throw new InvalidOperationException("Method resolution failed for " + call);
            }
            
            return method;
        }
    }
}
