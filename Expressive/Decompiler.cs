using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using ClrTest.Reflection;

using Expressive.Elements;
using Expressive.Elements.Presentation;
using Expressive.Pipeline;

namespace Expressive {
    public class Decompiler {
        private readonly IInterpretationStep[] pipeline;

        public Decompiler(params IInterpretationStep[] pipeline) {
            this.pipeline = pipeline;
        }

        public LambdaExpression Decompile(MethodBase method) {
            var elements = new ILReader(method)
                                    .Select(instruction => (IElement)new InstructionElement(instruction))
                                    .ToList();

            var context = new InterpretationContext(method, this.pipeline);
            try {
                foreach (var step in this.pipeline) {
                    step.Apply(elements, context);
                }
            }
            catch (Exception ex) {
                throw new InterpretationException(
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
            var expressionElement = returnElement != null ? (returnElement.Result as ExpressionElement) : elements[0] as ExpressionElement;
            if (expressionElement == null)
                throw new InvalidOperationException("Expected ReturnElement or ExpressionElement after interpretation, got " + elements[0].ToString(Indent.FourSpaces) + ".");

            return Expression.Lambda(expressionElement.Expression, context.ExtractedParameters.ToArray());
        }
    }
}
