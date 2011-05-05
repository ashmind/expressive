using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using AshMind.Extensions;

using ClrTest.Reflection;

using Expressive.Elements;
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

            var workspace = new InterpretationWorkspace(elements, method);
            foreach (var step in this.pipeline) {
                step.Apply(workspace);
            }

            if (workspace.Elements.Count > 1)
                throw new InvalidOperationException("Expected 1 element after interpretation, got: " + Environment.NewLine + Describe(workspace.Elements));

            var expressionElement = workspace.Elements[0] as ExpressionElement;
            if (expressionElement == null)
                throw new InvalidOperationException("Expected ExpressionElement after interpretation, got " + Describe(workspace.Elements).SubstringAfter("0: ") + ".");

            return Expression.Lambda(expressionElement.Expression, workspace.ExtractedParameters.ToArray());
        }

        private string Describe(IEnumerable<IElement> elements) {
            return string.Join(Environment.NewLine, elements.Select((e, index) => index + ": " + e.ToString()).ToArray());
        }
    }
}
