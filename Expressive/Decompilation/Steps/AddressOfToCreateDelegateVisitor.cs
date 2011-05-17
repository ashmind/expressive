using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using AshMind.Extensions;

using Expressive.Elements;
using Expressive.Elements.Expressions;
using Expressive.Elements.Expressions.Matchers;

namespace Expressive.Decompilation.Steps {
    public class AddressOfToCreateDelegateVisitor : ElementVisitor {
        private static readonly MethodInfo CreateDelegate = ((Func<Type, object, MethodInfo, bool, Delegate>)Delegate.CreateDelegate).Method;

        protected override Expression VisitNew(NewExpression nex) {
            nex = (NewExpression)base.VisitNew(nex);

            var instance = (Expression)null;
            var method = (MethodBase)null;
            return Matcher
                .Match(nex)
                    .Type(t => t.IsSubclassOf<Delegate>())
                    .Argument(0).AssignTo(out instance)
                
                .Match(nex)
                    .Argument(1).As<AddressOfExpression>()
                        .Do(x => method = x.Method)

                .Choose<Expression>(
                    () => Expression.Call(null, CreateDelegate, new[] {
                        Expression.Constant(nex.Type),
                        instance,
                        Expression.Constant(method),
                        Expression.Constant(true)
                    }).Convert(nex.Type),
                    nex
                );
        }
    }
}
