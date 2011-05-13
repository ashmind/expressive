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
        public void TestSimplePropertyWithReferenceToThis() {
            var decompiled = ExpressiveEngine.ToExpression(
                Property.Get<ClassWithNames>(c => c.JustFirstName).GetGetMethod()
            );

            AssertMatches(new[] { typeof(ClassWithNames) }, @"{0} => {0}.FirstName", decompiled);
        }

        [Test]
        public void TestPropertyWithReferenceToThisAndSimpleConcat() {
            var decompiled = ExpressiveEngine.ToExpression(
                Property.Get<ClassWithNames>(c => c.FullNameSimple).GetGetMethod()
            );

            AssertMatches(new[] { typeof (ClassWithNames) }, @"{0} => Concat({0}.FirstName, "" "", {0}.LastName)", decompiled);
        }

        [Test]
        public void TestStaticProperty() {
            var decompiled = ExpressiveEngine.ToExpression(
                Property.Get(() => ClassWithNames.StaticName).GetGetMethod()
            );

            AssertMatches(new Type[0], @"() => ""Test""", decompiled);
        }

        [Test]
        public void TestPropertyWithComparisonAndStaticFieldAndBooleanOperator() {
            var decompiled = ExpressiveEngine.ToExpression(
                Property.Get<ComplexClass>(m => m.ComplexProperty1).GetGetMethod()
             );

            AssertMatches(
                new[] { typeof(ComplexClass) },
                "{0} => (({0}.Int32Property > ComplexClass.Int32Field) AndAlso {0}.BooleanProperty)",
                decompiled
            );
        }

        [Test]
        public void TestPropertyWithComparisonAndConstantAndAnotherBooleanOperator() {
            var decompiled = ExpressiveEngine.ToExpression(
                Property.Get<ComplexClass>(m => m.ComplexProperty2).GetGetMethod()
             );

            AssertMatches(
                new[] { typeof(ComplexClass) },
                "{0} => (({0}.Int32Property > 0) OrElse Not({0}.BooleanProperty))",
                decompiled
            );
        }

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
        public void TestRange() {
            var decompiled = ExpressiveEngine.ToExpression(
                Method.Get<Range>(r => r.Contains(0))
            );

            AssertMatches(
                new[] { typeof(Range), typeof(int) },
                @"({0}, value) => ((value >= {0}.Min) AndAlso (value <= {0}.Max))",
                decompiled
            );
        }

        [Test]
        [Factory("GetTestMethodsAndProperties")]
        public void TestDecompilesTo(MethodBase method, IEnumerable<Type> parameterTypes, string pattern) {
            var decompiled = ExpressiveEngine.ToExpression(method);
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
                a.attribute.ParameterTypes,
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
