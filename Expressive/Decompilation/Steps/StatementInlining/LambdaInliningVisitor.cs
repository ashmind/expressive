using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

using AshMind.Extensions;

using Expressive.Elements;
using Expressive.Elements.Expressions.Matchers;
using Expressive.Matching;

namespace Expressive.Decompilation.Steps.StatementInlining {
    public class LambdaInliningVisitor : ContextualVisitor {
        private readonly IDictionary<FieldInfo, Expression> collectedFieldValues = new Dictionary<FieldInfo, Expression>();

        public LambdaInliningVisitor(DecompilationContext context) : base(context) {
        }

        protected override IElement VisitIfThen(IfThenElement ifThen) {
            ifThen = (IfThenElement)base.VisitIfThen(ifThen);

            var cachingField = (FieldInfo)null;
            return Matcher
                .For(ifThen.Condition)
                    .OneOf(ExpressionType.Equal)
                    .AsBinary()
                    .LeftAndRight(
                        leftOrRight => leftOrRight.AsPropertyOrField().Field()
                                                  .Match(IsLambdaCachingField)
                                                  .AssignTo(out cachingField),

                        leftOrRight => leftOrRight.AsConstant().ValueIsNull()
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

        protected override Expression VisitUnary(UnaryExpression u) {
            u = (UnaryExpression)base.VisitUnary(u);

            MethodInfo method;
            return Matcher
                .For(u).AsConvert()
                .Operand().AsMethodCall()
                    .Method(outer => outer == AddressOfToCreateDelegateVisitor.CreateDelegateMethodInfo)
                    .Argument<MethodInfo>()
                        .AsConstant().Value().As<MethodInfo>()
                            .Match(IsCompilerLambda)
                            .AssignTo(out method)

                .Choose<Expression>(() => this.Context.Decompiler.Decompile(method), u);
        }

        private static bool IsLambdaCachingField(FieldInfo field) {
            return field.IsStatic
                && field.IsDefined<CompilerGeneratedAttribute>(false)
                && field.FieldType.IsSubclassOf<Delegate>();
        }

        private bool IsCompilerLambda(MethodInfo method) {
            return method.IsDefined<CompilerGeneratedAttribute>(false);
        }
    }
}
