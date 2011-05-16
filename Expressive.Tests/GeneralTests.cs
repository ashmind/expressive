using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using MbUnit.Framework;

using AshMind.Extensions;

using Expressive.Decompilation.Pipelines;
using Expressive.Tests.Methods;
using Expressive.Tests.TestClasses;

namespace Expressive.Tests {
    [TestFixture]
    public class GeneralTests {
        [Test]
        [Factory("GetTestMethods")]
        public void TestDecompilesTo(MethodBase method, IEnumerable<string> patterns) {
            var decompiled = Decompile(method);

            var parameterNames = decompiled.Parameters.Select(p => p.Name).ToArray();
            var expected = patterns.Select(p => string.Format(p, parameterNames)).ToList();

            if (expected.Count <= 1) {
                Assert.AreEqual(expected.Single(), decompiled.ToString());
            }
            else {
                Assert.Contains(expected, decompiled.ToString());
            }
        }

        [Test]
        [Factory("GetTestMethods")]
        public void TestDecompilationResultHasCorrectParameters(MethodBase method) {
            var decompiled = Decompile(method);
            var parameterTypes = method.GetParameters().Select(p => p.ParameterType).ToList();
            if (!method.IsStatic)
                parameterTypes.Insert(0, method.DeclaringType);

            Assert.AreElementsSame(parameterTypes, decompiled.Parameters.Select(p => p.Type));
        }

        [Test]
        [Factory("GetTestMethods")]
        public void TestDecompilationResultIsCompilable(MethodBase method) {
            var decompiled = Decompile(method);
            Assert.DoesNotThrow(() => decompiled.Compile());
        }

        [Test]
        [Ignore("http://connect.microsoft.com/VisualStudio/feedback/details/361546/provide-implementation-for-getmethodbody-in-dynamicmethod")]
        public void TestDecompilationOfConversion(
            [Column(typeof(int))] Type originalType,
            [Column(typeof(int))] Type targetType
        ) {
            var parameter = Expression.Parameter(originalType, "x");
            var lambda = Expression.Lambda(
                typeof(Func<,>).MakeGenericType(originalType, targetType),
                parameter,
                parameter
            ).Compile();

            var decompiled = Decompile(lambda.Method);
            Assert.AreEqual("x => x", decompiled.ToString());
        }

        private static LambdaExpression Decompile(MethodBase method) {
            return new Decompiler(new TestDisassembler(), new DefaultPipeline()).Decompile(method);
        }

        public IEnumerable<object[]> GetTestMethods() {
            var someTestClass = typeof(ClassWithNames);
            var attributed = from type in someTestClass.Assembly.GetTypes()
                             where type.Namespace == someTestClass.Namespace
                             from member in type.GetMembers()
                             let attribute = member.GetCustomAttributes<ExpectedExpressionAttribute>().SingleOrDefault()
                             where attribute != null
                             select new { method = GetMethod(member), attribute };

            return attributed.Select(a => new object[] {
                a.method,
                a.attribute.Patterns
            });
        }

        private MethodBase GetMethod(MemberInfo member) {
            var property = member as PropertyInfo;
            if (property != null)
                return property.GetGetMethod();

            var field = member as FieldInfo;
            if (field != null)
                return (MethodBase)field.GetValue(null);

            return (MethodBase)member;
        }
    }
}
