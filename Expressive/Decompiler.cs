using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Expressive.Abstraction;
using Expressive.Decompilation.Pipelines;
using Expressive.Elements;
using Expressive.Elements.Presentation;
using Expressive.Decompilation;

namespace Expressive {
    public class Decompiler : IDecompiler {
        private readonly IDisassembler disassembler;
        private readonly IDecompilationPipeline pipeline;

        public Decompiler(IDisassembler disassembler, IDecompilationPipeline pipeline) {
            this.disassembler = disassembler;
            this.pipeline = pipeline;
        }

        public virtual LambdaExpression Decompile(IManagedMethod method) {
            var parameters = method.GetParameters().Select(p => Expression.Parameter(p.ParameterType, p.Name)).ToList();
            if (!method.IsStatic)
                parameters.Insert(0, Expression.Parameter(method.DeclaringType, "<this>"));

            var body = this.Decompile(method, parameters.Cast<Expression>().ToArray());
            return Expression.Lambda(body, parameters.ToArray());
        }

        public virtual Expression Decompile(IManagedMethod method, IList<Expression> arguments) {
            var elements = this.disassembler.Disassemble(method)
                                .Select(i => (IElement)new InstructionElement(i))
                                .ToList();

            var context = new DecompilationContext(this, method, i => arguments[i]);
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

            return GetSingleExpression(elements);
        }

        protected virtual Expression GetSingleExpression(IList<IElement> elements) {
            if (elements.Count > 1)
                throw new InvalidOperationException("Expected 1 element after interpretation, got: " + Environment.NewLine + ElementHelper.ToString(elements, Indent.FourSpaces));
            
            var returnElement = elements[0] as ReturnElement;
            var expressionElement = elements[0] as ExpressionElement;
            if (returnElement == null && expressionElement == null)
                throw new InvalidOperationException("Expected ReturnElement or ExpressionElement after interpretation, got " + elements[0].ToString(Indent.FourSpaces) + ".");
            
            var expression = returnElement != null ? returnElement.Result : expressionElement.Expression;
            return expression;
        }
    }
}
