using System;
using System.Collections.Generic;
using System.Linq;
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
            var decompiled = new Decompiler(new TestDisassembler(), new DefaultPipeline()).Decompile(method);
            var parameterTypes = method.GetParameters().Select(p => p.ParameterType).ToList();
            if (!method.IsStatic)
                parameterTypes.Insert(0, method.DeclaringType);

            Assert.AreElementsSame(parameterTypes, decompiled.Parameters.Select(p => p.Type));

            var parameterNames = decompiled.Parameters.Select(p => p.Name).ToArray();
            var expected = patterns.Select(p => string.Format(p, parameterNames));

            Assert.Contains(expected, decompiled.ToString());
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
