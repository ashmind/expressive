using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using AshMind.Extensions;

using Expressive.Elements;
using Expressive.Elements.Expressions;
using Expressive.Elements.Expressions.Matchers;
using Expressive.Matching;

namespace Expressive.Decompilation.Steps {
    public class AddressOfToCreateDelegateVisitor : ElementVisitor {
        public static readonly MethodInfo CreateDelegateMethodInfo = ((Func<Type, object, MethodInfo, bool, Delegate>)Delegate.CreateDelegate).Method;

        protected override Expression VisitNew(NewExpression nex) {
            nex = (NewExpression)base.VisitNew(nex);

            var instance = (Expression)null;
            var method = (MethodBase)null;
            return Matcher
                .For(nex)
                    .Type(t => t.IsSubclassOf<Delegate>())
                    .Argument(0).AssignTo(out instance)
                
                .For(nex)
                    .Argument(1).As<AddressOfExpression>()
                        .Do(x => method = x.Method)

                .IfMatched<Expression>(
                    () => Expression.Call(null, CreateDelegateMethodInfo, new[] {
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
