using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        public void TestInlineConditional() {
            var decompiled = ExpressiveEngine.ToExpression(
                Property.Get<ClassWithNames>(c => c.FullNameWithInlineConditional).GetGetMethod()
            );

            AssertMatches(
                new[] { typeof(ClassWithNames) },
                @"{0} => IIF(IsNullOrEmpty({0}.FirstName), {0}.LastName, Concat({0}.FirstName, "" "", {0}.LastName))",
                decompiled
            );
        }

        private void AssertMatches(IEnumerable<Type> parameterTypes, string pattern, LambdaExpression expression) {
            Assert.AreElementsSame(parameterTypes, expression.Parameters.Select(p => p.Type));
            var expected = string.Format(pattern, expression.Parameters.Select(p => p.Name).ToArray());
            Assert.AreEqual(expected, expression.ToString());
        }
    }
}
