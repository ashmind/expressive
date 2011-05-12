using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Expressive.Decompilation;
using Expressive.Decompilation.Pipelines;
using Expressive.Elements;
using MbUnit.Framework;

namespace Expressive.Tests.Massive {
    [TestFixture]
    public class InstructionSupportTest {
        [Test]
        [Ignore("Not passing yet")]
        public void TestAllInstructionsAreSupported() {
            var pipeline = new DefaultPipeline();
            var allMethods = GetAllMethods();
            var visitor = new InstructionCollectingVisitor();

            foreach (var method in allMethods) {
                var elements = ApplyPipeline(method, pipeline, suppressExceptions: true);
                visitor.VisitList(elements);
            }

            Assert.AreElementsEqual(new OpCode[0], visitor.OpCodes.OrderBy(code => code.Name));
        }
        
        [Test]
        [Ignore("Manual only for now")]
        [Factory("GetAllMethods")]
        public void TestNoExceptionsAreThrownWhenDecompiling(MethodInfo method) {
            var pipeline = new DefaultPipeline();
            Assert.DoesNotThrow(
                () => ApplyPipeline(method, pipeline, suppressExceptions: false)
            );
        }

        private IEnumerable<MethodInfo> GetAllMethods() {
            return typeof(string).Assembly.GetTypes().SelectMany(t => t.GetMethods());
        }

        private IList<IElement> ApplyPipeline(MethodBase method, IDecompilationPipeline pipeline, bool suppressExceptions) {
            var elements = new Disassembler().Disassemble(method).ToList();
            var context = new DecompilationContext(method);

            try {
                foreach (var step in pipeline.GetSteps()) {
                    step.Apply(elements, context);
                }
            }
            catch {
                if (suppressExceptions)
                    return new IElement[0];

                throw;
            }

            return elements;
        }
    }
}
