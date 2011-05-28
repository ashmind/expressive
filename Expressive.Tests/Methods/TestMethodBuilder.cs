using System;
using System.Collections.Generic;
using System.Linq;

using Expressive.Abstraction;
using Expressive.Disassembly.Instructions;
using Expressive.Tests.Helpers;

namespace Expressive.Tests.Methods {
    public class TestMethodBuilder {
        private string name;
        private Type returnType;
        private readonly IList<TestMethodParameter> parameters = new List<TestMethodParameter>();
        private readonly IList<Type> localTypes = new List<Type>();
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
            this.parameters.Add(new TestMethodParameter(name, type));
            return this;
        }

        public TestMethodBuilder Local<T>() {
            return this.Local(typeof(T));
        }

        public TestMethodBuilder Local(Type type) {
            this.localTypes.Add(type);
            return this;
        }

        public TestMethodBuilder Assemble(Func<Assembler, Assembler> assemble) {
            this.instructions = assemble(Assembler.Start).End.ToArray();
            return this;
        }

        public IManagedMethod ToMethod() {
            return new TestMethod(
                this.name ?? "TestMethod?",
                this.returnType ?? typeof(void),
                this.parameters,
                this.localTypes,
                this.instructions
            );
        }
    }
}
