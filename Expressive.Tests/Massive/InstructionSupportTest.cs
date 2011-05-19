using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Expressive.Decompilation;
using Expressive.Decompilation.Pipelines;
using Expressive.Decompilation.Steps.StatementInlining;
using Expressive.Elements;
using Expressive.Elements.Instructions;
using MbUnit.Framework;

namespace Expressive.Tests.Massive {
    [TestFixture]
    public class InstructionSupportTest {
        private static readonly HashSet<OpCode> UnsupportedOpCodes = new HashSet<OpCode> {
            OpCodes.Leave_S,
            OpCodes.Stfld,
            OpCodes.Switch,
            OpCodes.Throw,
            OpCodes.Endfinally
        };

        [Test]
        [Ignore("Not passing yet")]
        public void TestAllInstructionsExceptSpecificOnesAreSupported() {
            var pipeline = new DefaultPipeline().Without<LambdaInliningVisitor>();
            var disassembler = new Disassembler();
            var visitor = new InstructionCollectingVisitor();

            foreach (var method in GetAllNonGenericMethods()) {
                var elements = disassembler.Disassemble(method).Select(i => (IElement)new InstructionElement(i)).ToList();
                try { ApplyPipeline(pipeline, elements, method); } catch { continue; }
                visitor.VisitList(elements);
            }

            Assert.AreElementsEqual(
                new OpCode[0],
                visitor.OpCodes.Except(UnsupportedOpCodes).OrderBy(code => code.Name)
            );
        }
        
        [Test]
        [Ignore("Manual only for now")]
        [Factory("GetAllSupportedMethods")]
        public void TestNoExceptionsAreThrownWhenDecompiling(MethodInfo method, IList<Instruction> instructions) {
            var pipeline = new DefaultPipeline().Without<LambdaInliningVisitor>();
            var elements = instructions.Select(i => (IElement)new InstructionElement(i)).ToList();
            Assert.DoesNotThrow(() => {
                try {
                    ApplyPipeline(pipeline, elements, method);
                }
                catch (NotSupportedException) {
                }
            });
        }

        private IEnumerable<object[]> GetAllSupportedMethods() {
            var disassembler = new Disassembler();
            return GetAllNonGenericMethods()
                        .Select(method => new { method, instructions = disassembler.Disassemble(method).ToList() })
                        .Where(x => !x.instructions.Any(i => UnsupportedOpCodes.Contains(i.OpCode)))
                        .Select(x => new object[] { x.method, x.instructions });
        }

        private IEnumerable<MethodInfo> GetAllNonGenericMethods() {
            return typeof(string).Assembly.GetTypes().SelectMany(t => t.GetMethods())
                                                     .Where(m => !m.IsGenericMethodDefinition);
        }

        private static void ApplyPipeline(IDecompilationPipeline pipeline, IList<IElement> elements, MethodBase method) {
            var parameters = method.GetParameters().Select(p => Expression.Parameter(p.ParameterType, p.Name)).ToList();
            if (!method.IsStatic)
                parameters.Insert(0, Expression.Parameter(method.DeclaringType, "<this>"));

            var context = new DecompilationContext(null, method, i => parameters[i]);
            foreach (var step in pipeline.GetSteps()) {
                step.Apply(elements, context);
            }
        }
    }
}
