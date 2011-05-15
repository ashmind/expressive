using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Expressive.Elements.Instructions;
using Expressive.Tests.Helpers;

namespace Expressive.Tests.Methods {
    public class TestMethodBuilder {
        private string name;
        private Type returnType;
        private readonly IList<ParameterInfo> parameters = new List<ParameterInfo>();
        private readonly IList<LocalVariableInfo> locals = new List<LocalVariableInfo>();
        private Instruction[] instructions;

        public TestMethodBuilder Name(string name) {
            this.name = name;
            return this;
        }

        public TestMethodBuilder Returns<T>() {
            return this.Returns(typeof(T));
        }

        public TestMethodBuilder Returns(Type type) {
            this.returnType = type;
            return this;
        }

        public TestMethodBuilder Parameter<T>(string name) {
            return this.Parameter(name, typeof(T));
        }

        public TestMethodBuilder Parameter(string name, Type type) {
            parameters.Add(new PseudoParameter(name, type));
            return this;
        }

        public TestMethodBuilder Local<T>() {
            return this.Local(typeof(T));
        }

        public TestMethodBuilder Local(Type type) {
            locals.Add(new PseudoLocalVariable(type));
            return this;
        }

        public TestMethodBuilder Assemble(Func<Assembler, Assembler> assemble) {
            this.instructions = assemble(Assembler.Start).End.ToArray();
            return this;
        }

        public MethodBase ToMethod() {
            return new TestMethod(
                this.name ?? "TestMethod?",
                this.returnType ?? typeof(void),
                this.parameters,
                this.locals,
                this.instructions
            );
        }
    }
}
