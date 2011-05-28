using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Expressive.Abstraction;
using Expressive.Disassembly.Instructions;

namespace Expressive.Tests.Methods {
    public class TestMethod : IManagedMethod {
        private readonly string name;
        private readonly Type returnType;
        private readonly IEnumerable<IManagedMethodParameter> parameters;
        private readonly IList<Type> localTypes;
        private readonly Instruction[] instructions;

        public TestMethod(string name, Type returnType, IEnumerable<IManagedMethodParameter> parameters, IList<Type> localTypes, Instruction[] instructions) {
            this.name = name;
            this.returnType = returnType;
            this.parameters = parameters;
            this.localTypes = localTypes;
            this.instructions = instructions;
        }

        public string Name {
            get { return this.name; }
        }

        public Type ReturnType {
            get { return returnType; }
        }

        public IEnumerable<IManagedMethodParameter> GetParameters() {
            return parameters.ToArray();
        }

        public Type GetTypeOfLocal(int index) {
            return this.localTypes[index];
        }

        public Instruction[] GetInstructions() {
            return this.instructions;
        }

        public IManagedContext Context {
            get { throw new NotImplementedException(); }
        }

        public bool IsStatic {
            get { return true; }
        }

        public Type DeclaringType {
            get { return null; }
        }
        
        public byte[] GetBodyByteArray() {
            throw new NotSupportedException();
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
