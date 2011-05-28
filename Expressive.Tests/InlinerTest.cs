using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Expressive.Decompilation.Pipelines;
using Expressive.Tests.Helpers;
using MbUnit.Framework;

using Expressive.Tests.TestClasses;

namespace Expressive.Tests {
    [TestFixture]
    public class InlinerTest {
        [Test]
        public void TestInliningOfBasicProperty() {
            Expression<Func<ClassWithNames, bool>> predicate = c => c.FullNameSimple.Contains("Test"); // not a recommended database scenario
            var inliner = new Inliner(ExpressiveEngine.GetDecompiler());

            var inlined = inliner.Inline(predicate, p => p.Name == "FullNameSimple");

            Assert.AreEqual("c => String.Concat(c.FirstName, \" \", c.LastName).Contains(\"Test\")", ToStringVisitor.ToString(inlined));
        }
    }
}
