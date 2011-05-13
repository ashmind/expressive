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
        private static readonly HashSet<OpCode> UnsupportedOpCodes = new HashSet<OpCode> {
            OpCodes.Stfld,
            OpCodes.Throw
        };

        [Test]
        [Ignore("Not passing yet")]
        public void TestAllInstructionsExceptSpecificOnesAreSupported() {
            var pipeline = new DefaultPipeline();
            var disassembler = new Disassembler();
            var visitor = new InstructionCollectingVisitor();

            foreach (var method in GetAllMethods()) {
                var elements = disassembler.Disassemble(method).ToList();
                try { ApplyPipeline(pipeline, elements, method); } catch { }
                visitor.VisitList(elements);
            }

            Assert.AreElementsEqual(
                new OpCode[0],
                visitor.OpCodes.Except(UnsupportedOpCodes).OrderBy(code => code.Name)
            );
        }
        
        [Test]
        [Ignore("Manual only for now")]
        [Factory("GetAllMethodsWithSupportedInstructions")]
        public void TestNoExceptionsAreThrownWhenDecompiling(MethodInfo method, IList<IElement> elements) {
            var pipeline = new DefaultPipeline();
            Assert.DoesNotThrow(
                () => ApplyPipeline(pipeline, elements, method)
            );
        }

        private IEnumerable<object[]> GetAllMethodsWithSupportedInstructions() {
            var disassembler = new Disassembler();
            return GetAllMethods()
                        .Select(method => new { method, instructions = disassembler.Disassemble(method).ToList() })
                        .Where(x => !x.instructions.OfType<InstructionElement>().Any(i => UnsupportedOpCodes.Contains(i.OpCode)))
                        .Select(x => new object[] { x.method, x.instructions });
        }

        private IEnumerable<MethodInfo> GetAllMethods() {
            return typeof(string).Assembly.GetTypes().SelectMany(t => t.GetMethods());
        }

        private static void ApplyPipeline(IDecompilationPipeline pipeline, IList<IElement> elements, MethodBase method) {
            var context = new DecompilationContext(method);
            foreach (var step in pipeline.GetSteps()) {
                step.Apply(elements, context);
            }
        }
    }
}
