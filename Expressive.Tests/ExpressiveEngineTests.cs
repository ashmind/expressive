using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using MbUnit.Framework;

using AshMind.Extensions;

using Expressive.Tests.TestClasses;

namespace Expressive.Tests {
    [TestFixture]
    public class ExpressiveEngineTests {
        [Test]
        [Factory("GetTestMethodsAndProperties")]
        public void TestDecompilesTo(MethodBase method, IEnumerable<string> patterns) {
            var decompiled = ExpressiveEngine.ToExpression(method);
            var parameterTypes = method.GetParameters().Select(p => p.ParameterType).ToList();
            if (!method.IsStatic)
                parameterTypes.Insert(0, method.DeclaringType);

            Assert.AreElementsSame(parameterTypes, decompiled.Parameters.Select(p => p.Type));

            var parameterNames = decompiled.Parameters.Select(p => p.Name).ToArray();
            var expected = patterns.Select(p => string.Format(p, parameterNames));

            Assert.Contains(expected, decompiled.ToString());
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
                a.attribute.Patterns
            });
        }

        private IEnumerable<MethodBase> GetMethods(MemberInfo member) {
            var property = member as PropertyInfo;
            if (property != null)
                return property.GetAccessors();

            return new[] { (MethodBase)member };
        }
    }
}
