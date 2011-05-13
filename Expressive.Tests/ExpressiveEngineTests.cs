using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using MbUnit.Framework;

using AshMind.Extensions;

using Expressive.Tests.Helpers;
using Expressive.Tests.TestClasses;

namespace Expressive.Tests {
    [TestFixture]
    public class ExpressiveEngineTests {
        [Test]
        [Factory("GetConditionals")]
        public void TestConditional(PropertyInfo property) {
            var decompiled = ExpressiveEngine.ToExpression(property.GetGetMethod());

            AssertMatches(
                new[] { typeof(ClassWithNames) },
                new[] {
                    @"{0} => IIF(Not(IsNullOrEmpty({0}.FirstName)), Concat({0}.FirstName, "" "", {0}.LastName), {0}.LastName)",
                    @"{0} => IIF(IsNullOrEmpty({0}.FirstName), {0}.LastName, Concat({0}.FirstName, "" "", {0}.LastName))",
                },
                decompiled
            );
        }

        private IEnumerable<PropertyInfo> GetConditionals() {
            yield return Property.Get<ClassWithNames>(c => c.FullNameWithInlineConditional);
            yield return Property.Get<ClassWithNames>(c => c.FullNameWithExplicitConditional);
        }

        [Test]
        [Factory("GetTestMethodsAndProperties")]
        public void TestDecompilesTo(MethodBase method, string pattern) {
            var decompiled = ExpressiveEngine.ToExpression(method);
            var parameterTypes = method.GetParameters().Select(p => p.ParameterType).ToList();
            if (!method.IsStatic)
                parameterTypes.Insert(0, method.DeclaringType);

            AssertMatches(parameterTypes, pattern, decompiled);
        }

        public IEnumerable<object[]> GetTestMethodsAndProperties() {
            var someTestClass = typeof(ClassWithNames);
            var attributed = from type in someTestClass.Assembly.GetTypes()
                             where type.Namespace == someTestClass.Namespace
                             from member in type.GetMembers()
                             let attribute = member.GetCustomAttributes<ExpectedExpressionAttribute>().SingleOrDefault()
                             where attribute != null
                             from method in GetMethods(member)
                             select new { method, attribute };

            return attributed.Select(a => new object[] {
                a.method,
                a.attribute.Pattern
            });
        }

        private IEnumerable<MethodBase> GetMethods(MemberInfo member) {
            var property = member as PropertyInfo;
            if (property != null)
                return property.GetAccessors();

            return new[] { (MethodBase)member };
        }

        private void AssertMatches(IEnumerable<Type> parameterTypes, string pattern, LambdaExpression expression) {
            AssertMatches(parameterTypes, new[] { pattern }, expression);
        }

        private void AssertMatches(IEnumerable<Type> parameterTypes, string[] patterns, LambdaExpression expression) {
            Assert.AreElementsSame(parameterTypes, expression.Parameters.Select(p => p.Type));

            var parameterNames = expression.Parameters.Select(p => p.Name).ToArray();
            var expected = patterns.Select(p => string.Format(p, parameterNames));
            Assert.Contains(expected, expression.ToString());
        }
    }
}
