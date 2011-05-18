using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

using AshMind.Extensions;

using Expressive.Elements;
using Expressive.Elements.Expressions;
using Expressive.Elements.Expressions.Matchers;
using Expressive.Matching;

namespace Expressive.Decompilation.Steps.StatementInlining {
    public class LambdaInliningVisitor : ContextualVisitor {
        private class TargetInliningVisitor : ExpressionTreeVisitor {
            private readonly IDictionary<MemberInfo, Expression> values;
            private string targetParameterName;

            public TargetInliningVisitor(IDictionary<MemberInfo, Expression> values) {
                this.values = values;
            }

            public new Expression Visit(Expression expression) {
                return base.Visit(expression);
            }

            protected override Expression VisitLambda(LambdaExpression lambda) {
                this.targetParameterName = lambda.Parameters[0].Name;
                return Expression.Lambda(this.Visit(lambda.Body), lambda.Parameters.Skip(1).ToArray());
            }

            protected override Expression VisitMemberAccess(MemberExpression m) {
                m = (MemberExpression)base.VisitMemberAccess(m);
                return Matcher
                    .For(m.Expression).AsParameter()
                    .Name(targetParameterName)
                    .IfMatched(() => this.values[m.Member], m);
            }
        }

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
                    .For(list => list.Single()).As<MemberAssignmentElement>()
                        .Match(a => a.Member == cachingField)
                        .Do(a => collectedFieldValues.Add((FieldInfo)a.Member, a.Value))
                
                .For(ifThen.Else).Count(0)

                .IfMatched(() => null, ifThen);
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
            var memberInit = (MemberInitExpression)null;
            return Matcher
                .For(u).AsConvert()
                .Operand().AsMethodCall()
                    .Method(outer => outer == AddressOfToCreateDelegateVisitor.CreateDelegateMethodInfo)
                    .Argument<object>(a => a.Do(x => memberInit = x as MemberInitExpression))
                    .Argument<MethodInfo>()
                        .AsConstant().Value().As<MethodInfo>()
                            .Match(IsCompilerLambda)
                            .AssignTo(out method)

                .IfMatched<Expression>(() => Inline(method, memberInit), u);
        }

        private LambdaExpression Inline(MethodInfo method, MemberInitExpression memberInit) {
            var decompiled = this.Context.Decompiler.Decompile(method);
            if (method.IsStatic)
                return decompiled;

            return (LambdaExpression)new TargetInliningVisitor(
                memberInit.Bindings.Cast<MemberAssignment>()
                                   .ToDictionary(a => a.Member, a => a.Expression)
            ).Visit(decompiled);
        }

        private static bool IsLambdaCachingField(FieldInfo field) {
            return field.IsStatic
                && field.IsDefined<CompilerGeneratedAttribute>(false)
                && field.FieldType.IsSubclassOf<Delegate>();
        }

        private bool IsCompilerLambda(MethodInfo method) {
            return method.IsDefined<CompilerGeneratedAttribute>(false)
                || method.DeclaringType.IsDefined<CompilerGeneratedAttribute>(false);
        }
    }
}
