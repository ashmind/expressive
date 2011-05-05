using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Expressive.Tests.Helpers {
    public static class Property {
        public static PropertyInfo Get(Expression<Func<object>> reference) {
            return Property.Get((LambdaExpression)reference);
        }

        public static PropertyInfo Get<T>(Expression<Func<T, object>> reference) {
            return Property.Get((LambdaExpression)reference);
        }

        public static PropertyInfo Get(LambdaExpression reference) {
            return Property.Hierarchy(reference).Single();
        }

        public static PropertyInfo Get(MemberExpression member) {
            return Property.Hierarchy(member).Single();
        }

        public static IEnumerable<PropertyInfo> GetAll<T>(params Expression<Func<T, object>>[] references) {
            return references.SelectMany(Property.GetAll);
        }

        public static IEnumerable<PropertyInfo> GetAll<T, TResult>(Expression<Func<T, TResult>> reference) {
            var target = UnpackUnaryIfRequired(reference.Body);
            var member = target as MemberExpression;
            if (member != null)
                return new[] { Property.Get(member) };

            var @new = target as NewExpression;
            if (@new != null)
                return Property.GetAll(@new.Arguments);

            var array = target as NewArrayExpression;
            if (array != null && array.NodeType == ExpressionType.NewArrayInit)
                return Property.GetAll(array.Expressions);

            throw new NotSupportedException();
        }

        private static IEnumerable<PropertyInfo> GetAll(IEnumerable<Expression> expressions) {
            return expressions.Select(UnpackUnaryIfRequired)
                              .Cast<MemberExpression>()
                              .Select(Property.Get);
        }

        private static Expression UnpackUnaryIfRequired(Expression expression) {
            // Uncast is required for cases like 'Func<object> f = () => value.Id;'
            // If Id is int, compiler will put automatic cast here, changing code to
            // 'Func<object> f = () => (object)value.Id;'
            var cast = (expression as UnaryExpression);
            if (cast != null && cast.NodeType == ExpressionType.Convert)
                return cast.Operand;

            return expression;
        }

        private static PropertyInfo[] Hierarchy(LambdaExpression reference) {
            var member = (MemberExpression)UnpackUnaryIfRequired(reference.Body);
            return Property.Hierarchy(member);
        }

        private static PropertyInfo[] Hierarchy(MemberExpression member) {
            var properties = new List<PropertyInfo>();

            var next = member;
            while (next != null) {
                // The next check is required for closured values.
                // For example, '() => value.Name' is not just a MemberReference to Name, it
                // is an '__automatic_closure_instance.value.Name'.
                // I am not sure if it is the best way to check for it, but I do not see the
                // perfect solution right now.
                if (next.Member is FieldInfo && next.Expression is ConstantExpression)
                    break;

                properties.Add((PropertyInfo)next.Member);
                next = next.Expression as MemberExpression;
            }

            properties.Reverse();
            return properties.ToArray();
        }

        public static string[] Names<T>(params Expression<Func<T, object>>[] references) {
            return Array.ConvertAll(references, Property.Name);
        }

        public static string Name(Expression<Func<object>> reference) {
            return Property.Name(Property.Hierarchy(reference));
        }

        public static string Name<T>(Expression<Func<T, object>> reference) {
            return Property.Name(Property.Hierarchy(reference));
        }

        public static string Name<T, TResult>(Expression<Func<T, TResult>> reference) {
            return Property.Name(Property.Hierarchy(reference));
        }

        public static string Name(MemberExpression reference) {
            return Property.Name(Property.Hierarchy(reference));
        }

        private static string Name(PropertyInfo[] hierarchy) {
            return string.Join(".", Array.ConvertAll(hierarchy, p => p.Name));
        }
    }
}
