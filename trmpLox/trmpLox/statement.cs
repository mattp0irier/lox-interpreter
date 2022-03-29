using System;

namespace trmpLox
{
	public abstract class Statement
	{
        public interface Visitor<T>
        {
            T visitBlockStatement(BlockStmt stmt);
            T visitExpressionStatement(ExpressionStmt stmt);
            T visitFunctionStatement(FunctionStmt stmt);
            T visitIfStatement(IfStmt stmt);
            T visitPrintStatement(PrintStmt stmt);
            T visitReturnStatement(ReturnStmt stmt);
            T visitVarStatement(VarStmt stmt);
            T visitWhileStatement(WhileStmt stmt);
        }

        public abstract T accept<T>(Visitor<T> visitor);

        public override string ToString()
        {
            return this.GetType().Name;
        }

    }
    public class BlockStmt : Statement
    {
        public readonly List<Statement> statements;

        public BlockStmt(List<Statement> statements)
        {
            this.statements = statements;
        }

        public override T accept<T>(Visitor<T> visitor)
        {
            return visitor.visitBlockStatement(this);
        }
    }

    public class ExpressionStmt : Statement
    {
        public readonly Expression expression;

        public ExpressionStmt(Expression expression)
        {
            this.expression = expression;
        }

        public override T accept<T>(Visitor<T> visitor)
        {
            return visitor.visitExpressionStatement(this);
        }
    }

    public class FunctionStmt : Statement
    {
        public readonly Token name;
        public readonly List<Token> parameters;
        public readonly List<Statement> body;

        public FunctionStmt(Token name, List<Token> parameters, List<Statement> body)
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

    public class IfStmt : Statement
    {
        public readonly Expression condition;
        public readonly Statement trueBranch;
        public readonly Statement? falseBranch;

        public IfStmt(Expression condition, Statement trueBranch, Statement? falseBranch)
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

    public class PrintStmt : Statement
    {
        public readonly Expression expression;

        public PrintStmt(Expression expression)
        {
            this.expression = expression;
        }

        public override T accept<T>(Visitor<T> visitor)
        {
            return visitor.visitPrintStatement(this);
        }
    }

    public class ReturnStmt : Statement
    {
        public readonly Token keyword;
        public readonly Expression? value;

        public ReturnStmt(Token keyword, Expression? value)
        {
            this.keyword = keyword;
            this.value = value;
        }

        public override T accept<T>(Visitor<T> visitor)
        {
            return visitor.visitReturnStatement(this);
        }
    }

    public class VarStmt : Statement
    {
        public readonly Token name;
        public readonly Expression? initializer;

        public VarStmt(Token name, Expression? initializer)
        {
            this.name = name;
            this.initializer = initializer;
        }

        public override T accept<T>(Visitor<T> visitor)
        {
            return visitor.visitVarStatement(this);
        }
    }

    public class WhileStmt : Statement
    {
        public readonly Expression condition;
        public readonly Statement body;

        public WhileStmt(Expression condition, Statement body)
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

