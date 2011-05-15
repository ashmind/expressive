using System;
using System.Globalization;
using System.Reflection;

namespace Expressive.Tests.Methods {
    public abstract class StaticPseudoMethod : MethodInfo {
        public override MemberTypes MemberType {
            get { return MemberTypes.Method; }
        }

        public override Type DeclaringType {
            get { return null; }
        }

        public override Type ReflectedType {
            get { return null; }
        }

        public override RuntimeMethodHandle MethodHandle {
            get { return new RuntimeMethodHandle(); }
        }

        public override MethodAttributes Attributes {
            get { return MethodAttributes.Public | MethodAttributes.Static; }
        }

        public override object[] GetCustomAttributes(bool inherit) {
            return new object[0];
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit) {
            return new object[0];
        }

        public override bool IsDefined(Type attributeType, bool inherit) {
            return false;
        }

        public override MethodImplAttributes GetMethodImplementationFlags() {
            return MethodImplAttributes.IL;
        }

        public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture) {
            throw new NotSupportedException();
        }

        public override MethodInfo GetBaseDefinition() {
            return null;
        }

        public override ICustomAttributeProvider ReturnTypeCustomAttributes {
            get { return this.ReturnType; }
        }
    }
}