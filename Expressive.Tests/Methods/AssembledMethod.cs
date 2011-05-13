using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Expressive.Elements.Instructions;
using Expressive.Tests.Helpers;

namespace Expressive.Tests.Methods {
    public class AssembledMethod : StaticPseudoMethod {
        private readonly string name;
        private readonly IList<ParameterInfo> parameters = new List<ParameterInfo>();
        private Instruction[] instructions;

        public AssembledMethod(string name) {
            this.name = name;
        }

        public override string Name {
            get { return this.name; }
        }

        public AssembledMethod Parameter<T>(string name) {
            return this.Parameter(name, typeof(T));
        }

        public AssembledMethod Parameter(string name, Type type) {
            parameters.Add(new PseudoParameter(name, type));
            return this;
        }

        public AssembledMethod Assemble(Func<Assembler, Assembler> assemble) {
            this.instructions = assemble(Assembler.Start).End.ToArray();
            return this;
        }

        public override ParameterInfo[] GetParameters() {
            return parameters.ToArray();
        }

        public Instruction[] GetInstructions() {
            return this.instructions;
        }

        public override string ToString() {
            return this.Name;
        }
    }
}
