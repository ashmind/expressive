using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using AshMind.Extensions;
using Expressive.Elements.Expressions;

namespace Expressive.Tests.Helpers {
    public class ToStringVisitor : ExpressionTreeVisitor {
        private static readonly IDictionary<ExpressionType, string> operatorToString = new Dictionary<ExpressionType, string> {
            { ExpressionType.Coalesce,           "??" },    
        
            { ExpressionType.Negate,             "-" },
            { ExpressionType.Add,                "+" },
            { ExpressionType.Subtract,           "-" },
            { ExpressionType.Multiply,           "*" },
            { ExpressionType.Divide,             "/" },
            { ExpressionType.Modulo,             "%" },

            { ExpressionType.And,                "&" },
            { ExpressionType.Or,                 "|" },
            { ExpressionType.ExclusiveOr,        "^" },

            { ExpressionType.LeftShift,          "<<" },
            { ExpressionType.RightShift,         ">>" },

            { ExpressionType.Equal,              "==" },
            { ExpressionType.NotEqual,           "!=" },
            { ExpressionType.GreaterThan,        ">" },
            { ExpressionType.GreaterThanOrEqual, ">=" },
            { ExpressionType.LessThan,           "<" },
            { ExpressionType.LessThanOrEqual,    "<=" },
 
            { ExpressionType.Not,                "!" },
            { ExpressionType.AndAlso,            "&&" },
            { ExpressionType.OrElse,             "||" }
        };

        private readonly StringBuilder builder;

        private ToStringVisitor() {
            this.builder = new StringBuilder();
        }

        protected override Expression VisitLambda(LambdaExpression lambda) {
            if (lambda.Parameters.Count != 1)
                this.builder.Append("(");
            this.AppendCommaSeparated(lambda.Parameters);
            if (lambda.Parameters.Count != 1)
                this.builder.Append(")");

            this.builder.Append(" => ");
            this.Visit(lambda.Body);
            
            return lambda;
        }

        protected override Expression VisitParameter(ParameterExpression p) {
            this.builder.Append(p.Name);
            return p;
        }

        protected override Expression VisitMemberAccess(MemberExpression m) {
            this.Visit(m.Expression);
            builder.Append(".")
                   .Append(m.Member.Name);
            return m;
        }

        protected override Expression VisitMethodCall(MethodCallExpression m) {
            var arguments = (IEnumerable<Expression>)m.Arguments;
            if (m.Object != null) {
                this.Visit(m.Object);
            }
            else if (m.Method.IsDefined<ExtensionAttribute>(false)) {
                this.Visit(arguments.First());
                arguments = arguments.Skip(1);
            }
            else {
                this.AppendType(m.Method.ReflectedType);
            }

            builder.Append(".")
                   .Append(m.Method.Name)
                   .Append("(");
            this.AppendCommaSeparated(arguments);
            builder.Append(")");

            return m;
        }

        protected override Expression VisitNew(NewExpression nex) {
            this.AppendNew(nex, false);
            return nex;
        }

        protected override Expression VisitNewArray(NewArrayExpression na) {
            this.builder.Append("new ");
            var elementType = na.Type.GetElementType();
            if (na.Expressions.All(e => e.Type != elementType))
                this.AppendType(elementType);

            this.builder.Append("[] { ");
            this.AppendCommaSeparated(na.Expressions);
            this.builder.Append(" }");

            return na;
        }

        protected override Expression VisitMemberInit(MemberInitExpression init) {
            this.AppendNew(init.NewExpression, false);
            builder.Append(" { ");
            this.AppendCommaSeparated(init.Bindings, b => this.VisitBinding(b));
            builder.Append(" }");
            return init;
        }

        protected override MemberAssignment VisitMemberAssignment(MemberAssignment assignment) {
            builder.Append(assignment.Member.Name)
                   .Append(" = ");
            this.Visit(assignment.Expression);
            return assignment;
        }

        protected override Expression VisitListInit(ListInitExpression init) {
            this.AppendNew(init.NewExpression, false);
            builder.Append(" { ");
            this.AppendCommaSeparated(init.Initializers, i => this.VisitElementInitializer(i));
            builder.Append(" }");
            return init;
        }

        protected override ElementInit VisitElementInitializer(ElementInit initializer) {
            if (initializer.Arguments.Count > 1)
                this.builder.Append("{ ");
            AppendCommaSeparated(initializer.Arguments);
            if (initializer.Arguments.Count > 1)
                this.builder.Append(" }");

            return initializer;
        }

        private void AppendNew(NewExpression nex, bool requireBraces) {
            builder.Append("new ");
            this.AppendType(nex.Type);
            requireBraces = requireBraces | nex.Arguments.Count > 0;

            if (requireBraces)
                builder.Append("(");

            AppendCommaSeparated(nex.Arguments);

            if (requireBraces)
                builder.Append(")");
        }

        protected override Expression VisitConditional(ConditionalExpression c) {
            this.builder.Append("(");
            this.Visit(c.Test);
            this.builder.Append(" ? ");
            this.Visit(c.IfTrue);
            this.builder.Append(" : ");
            this.Visit(c.IfFalse);
            this.builder.Append(")");

            return c;
        }

        protected override Expression VisitBinary(BinaryExpression b) {
            this.builder.Append("(");
            this.Visit(b.Left);
            this.builder.Append(" ").Append(operatorToString[b.NodeType]).Append(" ");
            this.Visit(b.Right);
            this.builder.Append(")");
            
            return b;
        }

        protected override Expression VisitUnary(UnaryExpression u) {
            if (u.NodeType != ExpressionType.Convert) {
                this.builder.Append(operatorToString[u.NodeType]);
            }
            else {
                this.builder.Append("(");
                this.AppendType(u.Type);
                this.builder.Append(")");
            }

            this.Visit(u.Operand);

            return u;
        }

        protected override Expression VisitConstant(ConstantExpression c) {
            if (c.Value is string)
                builder.Append("\"");

            builder.Append(c.Value ?? "null");

            if (c.Value is string)
                builder.Append("\"");

            return c;
        }

        private void AppendType(Type type) {
            if (type.IsGenericTypeDefinedAs(typeof(Nullable<>))) {
                this.AppendType(type.GetGenericArguments()[0]);
                this.builder.Append("?");
                return;
            }

            this.builder.Append(type.Name.SubstringBefore("`"));
            if (!type.IsGenericType)
                return;

            this.builder.Append("<");
            AppendCommaSeparated(type.GetGenericArguments(), this.AppendType);
            this.builder.Append(">");
        }

        private void AppendCommaSeparated<TExpression>(IEnumerable<TExpression> expressions)
            where TExpression : Expression
        {
            this.AppendCommaSeparated(expressions, e => this.Visit(e));
        }

        private void AppendCommaSeparated<T>(IEnumerable<T> items, Action<T> append) {
            var first = true;
            foreach (var item in items) {
                if (!first)
                    this.builder.Append(", ");

                append(item);
                first = false;
            }
        }

        public static string ToString(Expression expression) {
            var visitor = new ToStringVisitor();
            visitor.Visit(expression);
            return visitor.ToString();
        }

        public override string ToString() {
            return this.builder.ToString();
        }
    }
}
