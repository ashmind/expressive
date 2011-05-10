using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using MbUnit.Framework;

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
        public void TestPropertyWithComparisonAndStaticFieldAndBooleanOperations() {
            var decompiled = ExpressiveEngine.ToExpression(
                Property.Get<ClassWithMagic>(m => m.CanDoMagic).GetGetMethod()
             );

            AssertMatches(
                new[] { typeof(ClassWithMagic) },
                new[] {
                    "{0} => (Not(({0}.Mana <= ClassWithMagic.ManaRequiredForMagic)) AndAlso {0}.IsAllowedToDoMagic)"
                },
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
