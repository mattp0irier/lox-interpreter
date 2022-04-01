using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace trmpLox
{
    // Expression abstract class
    public abstract class Expression
    {
        // Visitor interface
        public interface Visitor<T> 
        {
            T visitAssignExpr(Assign expr);
            T visitBinaryExpr(Binary expr);
            T visitCallExpr(Call expr);
            //T visitGetExpr(Get expr);
            T visitGroupingExpr(Grouping expr);
            T visitLiteralExpr(Literal expr);
            T visitLogicalExpr(Logical expr);
            //T visitSetExpr(Set expr);
            //T visitThisExpr(This expr);
            T visitUnaryExpr(Unary expr);
            T visitVariableExpr(Variable expr);
        }

        // Accept<T>: abstract method to be implemented
        public abstract T Accept<T>(Visitor<T> visitor);

        // ToString: return type of object
        public override string ToString()
        {
            return this.GetType().Name;
        }
    }

    // Assign class inherits Expression: for assigning variables
    public class Assign : Expression
    {
        public readonly Token name;
        public readonly Expression value;

        // Assign: constructor for Assign expression
        public Assign(Token name, Expression value)
        {
            this.name = name;
            this.value = value;
        }

        // Accept<T>: implement Accept function
        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.visitAssignExpr(this);
        }
    }

    // Binary class inherits Expression: for binary operations
    public class Binary : Expression
    {
        public readonly Expression left;
        public readonly Token op;
        public readonly Expression right;

        // Binary: constructor for Binary expression
        public Binary(Expression left, Token op, Expression right)
        {
            this.left = left;
            this.op = op;
            this.right = right;
        }

        // Accept<T>: implement Accept function
        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.visitBinaryExpr(this);
        }
    }

    // Call class inherits Expression: stores function calls 
    public class Call : Expression
    {
        public readonly Expression callee;
        public readonly Token paren;
        public readonly List<Expression> args;

        // Call: constructor for Call expression
        public Call(Expression callee, Token paren, List<Expression> args)
        {
            this.callee = callee;
            this.paren = paren;
            this.args = args;
        }

        // Accept<T>: implement Accept function
        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.visitCallExpr(this);
        }
    }

    //public class Get : Expression
    //{
    //    public readonly Expression obj;
    //    public readonly Token name;


    //    public Get(Expression obj, Token name)
    //    {
    //        this.obj = obj;
    //        this.name = name;
    //    }

    //    public override T Accept<T>(Visitor<T> visitor)
    //    {
    //        return visitor.visitGetExpr(this);
    //    }
    //}

    // Grouping class inherits Expression: stores groupings of expressions 
    public class Grouping : Expression
    {
        public readonly Expression expr;

        // Grouping: constructor for Grouping expression
        public Grouping(Expression expr)
        {
            this.expr = expr;
        }

        // Accept<T>: implement Accept function
        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.visitGroupingExpr(this);
        }
    }

    // Literal class inherits Expression: stores literals
    public class Literal : Expression
    {
        public readonly object? value;

        // Literal: constructor for Literal expression
        public Literal(object? value)
        {
            this.value = value;
        }

        // Accept<T>: implement Accept function
        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.visitLiteralExpr(this);
        }
    }

    // Logical class inherits Expression: stores logical expressions
    public class Logical : Expression
    {
        public readonly Expression left;
        public readonly Token op;
        public readonly Expression right;

        // Logical: constructor for Logical expression
        public Logical(Expression left, Token op, Expression right)
        {
            this.left = left;
            this.op = op;
            this.right = right;
        }

        // Accept<T>: implement Accept function
        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.visitLogicalExpr(this);
        }
    }

    //public class Set : Expression
    //{
    //    public Set(Expression obj, Token name, Expression value)
    //    {
    //        this.obj = obj;
    //        this.name = name;
    //        this.value = value;
    //    }

    //    public override T Accept<T>(Visitor<T> visitor)
    //    {
    //        return visitor.visitSetExpr(this);
    //    }

    //    public readonly Expression obj;
    //    public readonly Token name;
    //    public readonly Expression value;
    //}
    //public class This : Expression
    //{
    //    public This(Token keyword)
    //    {
    //        this.keyword = keyword;
    //    }

    //    public override T Accept<T>(Visitor<T> visitor)
    //    {
    //        return visitor.visitThisExpr(this);
    //    }

    //    public readonly Token keyword;
    //}

    // Unary class inherits Expression: stores unary expressions
    public class Unary : Expression
    {
        public readonly Token op;
        public readonly Expression right;

        // Unary: constructor for Unary expression
        public Unary(Token op, Expression right)
        {
            this.op = op;
            this.right = right;
        }

        // Accept<T>: implement Accept function
        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.visitUnaryExpr(this);
        }
    }

    // Variable class inherits Expression: stores variable expressions
    public class Variable : Expression
    {
        public readonly Token name;

        // Variable: constructor for Variable expression
        public Variable(Token name)
        {
            this.name = name;
        }

        // Accept<T>: implement Accept function
        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.visitVariableExpr(this);
        }
    }

}
