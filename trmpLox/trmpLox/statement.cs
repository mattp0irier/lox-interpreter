using System;

namespace trmpLox
{
    // Statement abstract class
	public abstract class Statement
	{
        // Visitor interface
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

        // Accept<T>: abstract method to be implemented
        public abstract T accept<T>(Visitor<T> visitor);

        // ToString: return type of object
        public override string ToString()
        {
            return this.GetType().Name;
        }

    }

    // BlockStmt class inherits Statement: for blocks containing statements
    public class BlockStmt : Statement
    {
        public readonly List<Statement> statements;

        // BlockStmt: constructor for BlockStmt expression
        public BlockStmt(List<Statement> statements)
        {
            this.statements = statements;
        }

        // Accept<T>: implement Accept function
        public override T accept<T>(Visitor<T> visitor)
        {
            return visitor.visitBlockStatement(this);
        }
    }

    // ExpressionStmt class inherits Statement: for expressions
    public class ExpressionStmt : Statement
    {
        public readonly Expression expression;

        // ExpressionStmt: constructor for ExpressionStmt expression
        public ExpressionStmt(Expression expression)
        {
            this.expression = expression;
        }

        // Accept<T>: implement Accept function
        public override T accept<T>(Visitor<T> visitor)
        {
            return visitor.visitExpressionStatement(this);
        }
    }

    // FunctionStmt class inherits Statement: for function definitions
    public class FunctionStmt : Statement
    {
        public readonly Token name;
        public readonly List<Token> parameters;
        public readonly List<Statement> body;

        // FunctionStmt: constructor for FunctionStmt expression
        public FunctionStmt(Token name, List<Token> parameters, List<Statement> body)
        {
            this.name = name;
            this.parameters = parameters;
            this.body = body;
        }

        // Accept<T>: implement Accept function
        public override T accept<T>(Visitor<T> visitor)
        {
            return visitor.visitFunctionStatement(this);
        }
    }

    // IfStmt class inherits Statement: for if statements
    public class IfStmt : Statement
    {
        public readonly Expression condition;
        public readonly Statement trueBranch;
        public readonly Statement? falseBranch;

        // IfStmt: constructor for IfStmt expression
        public IfStmt(Expression condition, Statement trueBranch, Statement? falseBranch)
        {
            this.condition = condition;
            this.trueBranch = trueBranch;
            this.falseBranch = falseBranch;
        }

        // Accept<T>: implement Accept function
        public override T accept<T>(Visitor<T> visitor)
        {
            return visitor.visitIfStatement(this);
        }
    }

    // PrintStmt class inherits Statement: for print statements
    public class PrintStmt : Statement
    {
        public readonly Expression expression;

        // PrintStmt: constructor for PrintStmt expression
        public PrintStmt(Expression expression)
        {
            this.expression = expression;
        }

        // Accept<T>: implement Accept function
        public override T accept<T>(Visitor<T> visitor)
        {
            return visitor.visitPrintStatement(this);
        }
    }

    // ReturnStmt class inherits Statement: for print statements
    public class ReturnStmt : Statement
    {
        public readonly Token keyword;
        public readonly Expression? value;

        // ReturnStmt: constructor for ReturnStmt expression
        public ReturnStmt(Token keyword, Expression? value)
        {
            this.keyword = keyword;
            this.value = value;
        }

        // Accept<T>: implement Accept function
        public override T accept<T>(Visitor<T> visitor)
        {
            return visitor.visitReturnStatement(this);
        }
    }

    // VarStmt class inherits Statement: for variable initialization statements
    public class VarStmt : Statement
    {
        public readonly Token name;
        public readonly Expression? initializer;

        // VarStmt: constructor for VarStmt expression
        public VarStmt(Token name, Expression? initializer)
        {
            this.name = name;
            this.initializer = initializer;
        }

        // Accept<T>: implement Accept function
        public override T accept<T>(Visitor<T> visitor)
        {
            return visitor.visitVarStatement(this);
        }
    }

    // WhileStmt class inherits Statement: for while loops
    public class WhileStmt : Statement
    {
        public readonly Expression condition;
        public readonly Statement body;

        // WhileStmt: constructor for WhileStmt expression
        public WhileStmt(Expression condition, Statement body)
        {
            this.condition = condition;
            this.body = body;
        }

        // Accept<T>: implement Accept function
        public override T accept<T>(Visitor<T> visitor)
        {
            return visitor.visitWhileStatement(this);
        }
    }
}

