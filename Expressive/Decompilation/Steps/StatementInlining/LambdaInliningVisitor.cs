using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

using AshMind.Extensions;

using Expressive.Elements;
using Expressive.Elements.Expressions.Matchers;

namespace Expressive.Decompilation.Steps.StatementInlining {
    public class LambdaInliningVisitor : ElementVisitor {
        private readonly IDictionary<FieldInfo, Expression> collectedFieldValues = new Dictionary<FieldInfo, Expression>();

        protected override IElement VisitIfThen(IfThenElement ifThen) {
            ifThen = (IfThenElement)base.VisitIfThen(ifThen);

            var cachingField = (FieldInfo)null;
            return Matcher
                .For(ifThen.Condition)
                    .OneOf(ExpressionType.Equal)
                    .AsBinary()
                    .LeftAndRight(
                        leftOrRight => leftOrRight.AsPropertyOrField().Field()
                                                  .Match(IsDelegateCachingField)
                                                  .AssignTo(out cachingField),

                        leftOrRight => leftOrRight.Constant(value => value == null)
                    )
                
                .For(ifThen.Then)
                    .Count(1)
                    .For(list => list.Single()).As<FieldAssignmentElement>()
                        .Match(a => a.Field == cachingField)
                        .Do(a => collectedFieldValues.Add(a.Field, a.Value))
                
                .For(ifThen.Else).Count(0)

                .Choose(() => null, ifThen);
        }

        protected override Expression VisitMemberAccess(MemberExpression m) {
            m = (MemberExpression)base.VisitMemberAccess(m);

            var field = m.Member as FieldInfo;
            if (field == null)
                return m;

            return this.collectedFieldValues.GetValueOrDefault(field, (Expression)m);
        }

        private static bool IsDelegateCachingField(FieldInfo field) {
            return field.IsStatic
                && field.IsDefined<CompilerGeneratedAttribute>(false)
                && field.FieldType.IsSubclassOf<Delegate>();
        }
    }
}
