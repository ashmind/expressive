using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using AshMind.Extensions;

using Expressive.Elements;

namespace Expressive.Decompilation.Steps.Clarity {
    public class NewNullableToCastVisitor : ElementVisitor {
        protected override Expression VisitNew(NewExpression nex) {
            nex = (NewExpression)base.VisitNew(nex);
            if (!nex.Constructor.DeclaringType.IsGenericTypeDefinedAs(typeof(Nullable<>)))
                return nex;

            return Expression.Convert(nex.Arguments[0], nex.Type);
        }
    }
}
