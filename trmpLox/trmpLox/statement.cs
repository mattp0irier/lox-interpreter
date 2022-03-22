using System;

namespace trmpLox
{
	abstract class Statement
	{
        public interface Visitor<T>
        {
            T visitBlockStatement(Block stmt);
            T visitExpressionStatement(Expression stmt);
            T visitFunctionStatement(Function stmt);
            T visitIfStatement(If stmt);
            T visitPrintStatement(Print stmt);
            T visitReturnStatement(Return stmt);
            T visitVarStatement(Var stmt);
            T visitWhileStatement(While stmt);
        }

        public abstract T accept<T>(Visitor<T> visitor);

        public override string ToString()
        {
            return this.GetType().Name;
        }

        public class Block : Statement
        {
            readonly List<Statement> statements;

            Block (List<Statement> statements)
            {
                this.statements = statements;
            }

            public override T accept<T>(Visitor<T> visitor)
            {
                return visitor.visitBlockStatement(this);
            }
        }

        public class Expression : Statement
        {
            readonly Expression expression;

            Expression(Expression expression)
            {
                this.expression = expression;
            }

            public override T accept<T>(Visitor<T> visitor)
            {
                return visitor.visitExpressionStatement(this);
            }
        }

        public class Function : Statement
        {
            readonly Token name;
            readonly List<Token> parameters;
            readonly List<Statement> body;

            Function(Token name, List<Token> parameters, List<Statement> body)
            {
                this.name = name;
                this.parameters = parameters;
                this.body = body;
            }

            public override T accept<T>(Visitor<T> visitor)
            {
                return visitor.visitFunctionStatement(this);
            }
        }

        public class If : Statement
        {
            readonly Expression condition;
            readonly Statement trueBranch;
            readonly Statement falseBranch;

            If(Expression condition, Statement trueBranch, Statement falseBranch)
            {
                this.condition = condition;
                this.trueBranch = trueBranch;
                this.falseBranch = falseBranch;
            }

            public override T accept<T>(Visitor<T> visitor)
            {
                return visitor.visitIfStatement(this);
            }
        }

        public class Print : Statement
        {
            readonly Expression expression;

            Print(Expression expression)
            {
                this.expression = expression;
            }

            public override T accept<T>(Visitor<T> visitor)
            {
                return visitor.visitPrintStatement(this);
            }
        }

        public class Return : Statement
        {
            readonly Token keyword;
            readonly Expression value;

            Return(Token keyword, Expression value)
            {
                this.keyword = keyword;
                this.value = value;
            }

            public override T accept<T>(Visitor<T> visitor)
            {
                return visitor.visitReturnStatement(this);
            }
        }

        public class Var : Statement
        {
            readonly Token name;
            readonly Expression initializer;

            Var(Token name, Expression initializer)
            {
                this.name = name;
                this.initializer = initializer;
            }

            public override T accept<T>(Visitor<T> visitor)
            {
                return visitor.visitVarStatement(this);
            }
        }

        public class While : Statement
        {
            readonly Expression condition;
            readonly Statement body;

            While(Expression condition, Statement body)
            {
                this.condition = condition;
                this.body = body;
            }

            public override T accept<T>(Visitor<T> visitor)
            {
                return visitor.visitWhileStatement(this);
            }
        }
    }
}

