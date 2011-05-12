using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using ClrTest.Reflection;
using Expressive.Decompilation.Pipelines;
using Expressive.Elements;
using Expressive.Elements.Presentation;
using Expressive.Decompilation;

namespace Expressive {
    public class Decompiler {
        private readonly IDecompilationPipeline pipeline;

        public Decompiler(IDecompilationPipeline pipeline) {
            this.pipeline = pipeline;
        }

        public LambdaExpression Decompile(MethodBase method) {
            var elements = new ILReader(method)
                                    .Select(instruction => (IElement)new InstructionElement(instruction))
                                    .ToList();

            var context = new DecompilationContext(method);
            try {
                foreach (var step in this.pipeline.GetSteps()) {
                    step.Apply(elements, context);
                }
            }
            catch (Exception ex) {
                throw new DecompilationException(
                    "Exception while interpreting "
                        + Environment.NewLine
                        + ElementHelper.ToString(elements, Indent.FourSpaces)
                        + Environment.NewLine
                        + ex.Message, ex
                );
            }

            if (elements.Count > 1)
                throw new InvalidOperationException("Expected 1 element after interpretation, got: " + Environment.NewLine + ElementHelper.ToString(elements, Indent.FourSpaces));

            var returnElement = elements[0] as ReturnElement;
            var expressionElement = elements[0] as ExpressionElement;
            if (returnElement == null && expressionElement == null)
                throw new InvalidOperationException("Expected ReturnElement or ExpressionElement after interpretation, got " + elements[0].ToString(Indent.FourSpaces) + ".");

            var expression = returnElement != null ? returnElement.Result : expressionElement.Expression;
            return Expression.Lambda(expression, context.ExtractedParameters.ToArray());
        }
    }
}
