using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Expressive.Elements.Instructions;

namespace Expressive.Tests.Methods {
    public class TestMethod : StaticPseudoMethod {
        private readonly string name;
        private readonly Type returnType;
        private readonly IList<ParameterInfo> parameters;
        private readonly IList<LocalVariableInfo> locals;
        private readonly Instruction[] instructions;

        public TestMethod(string name, Type returnType, IList<ParameterInfo> parameters, IList<LocalVariableInfo> locals, Instruction[] instructions) {
            this.name = name;
            this.returnType = returnType;
            this.parameters = parameters;
            this.locals = locals;
            this.instructions = instructions;
        }

        public override string Name {
            get { return this.name; }
        }

        public override Type ReturnType {
            get { return returnType; }
        }

        public override ParameterInfo[] GetParameters() {
            return parameters.ToArray();
        }

        public Instruction[] GetInstructions() {
            return this.instructions;
        }

        public override MethodBody GetMethodBody() {
            return new PseudoMethodBody(this.locals.ToArray());
        }

        public override string ToString() {
            return string.Format(
                "{0} {1}({2})",
                this.ReturnType.Name, this.Name,
                string.Join(", ", this.parameters.Select(p => p.ParameterType.Name))
            );
        }
    }
}
