using System;
using System.Collections.Generic;
using System.Linq;

using MbUnit.Framework;

using Expressive.Tests.Helpers;
using Expressive.Tests.TestClasses;

namespace Expressive.Tests {
    [TestFixture]
    public class ExpressiveEngineTests {
        [Test]
        public void TestSimplePropertyWithReferenceToThis() {
            var decompiled = ExpressiveEngine.ToExpression(
                Property.Get<ClassWithNames>(c => c.FullNameSimple).GetGetMethod()
            );

            Assert.AreElementsSame(
                new[] { typeof(ClassWithNames) },
                decompiled.Parameters.Select(p => p.Type)
            );
            var expected = string.Format(@"{0} => Concat({0}.FirstName, "" "", {0}.LastName)", decompiled.Parameters[0].Name);
            Assert.AreEqual(expected, decompiled.ToString());
        }
    }
}
