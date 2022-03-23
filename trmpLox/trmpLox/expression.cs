using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace trmpLox
{
    public abstract class Expression
    {
        public interface Visitor<T> 
        {
            T visitAssignExpr(Assign expr);
            T visitBinaryExpr(Binary expr);
            T visitCallExpr(Call expr);
            T visitGetExpr(Get expr);
            T visitGroupingExpr(Grouping expr);
            T visitLiteralExpr(Literal expr);
            T visitLogicalExpr(Logical expr);
            T visitSetExpr(Set expr);
            T visitThisExpr(This expr);
            T visitUnaryExpr(Unary expr);
            T visitVariableExpr(Variable expr);
        }
        public abstract T Accept<T>(Visitor<T> visitor);

        public override string ToString()
        {
            return this.GetType().Name;
        }
    }

    public class Assign : Expression
    {
        Assign(Token name, Expression value)
        {
            this.name = name;
            this.value = value;
        }

        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.visitAssignExpr(this);
        }

        readonly Token name;
        readonly Expression value;
    }

    public class Binary : Expression
    {
        public Binary(Expression left, Token op, Expression right)
        {
            this.left = left;
            this.op = op;
            this.right = right;
        }

        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.visitBinaryExpr(this);
        }

        public readonly Expression left;
        public readonly Token op;
        public readonly Expression right;
    }
    public class Call : Expression
    {
        public Call(Expression callee, Token paren, List<Expression> args)
        {
            this.callee = callee;
            this.paren = paren;
            this.args = args;
        }

        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.visitCallExpr(this);
        }

        public readonly Expression callee;
        public readonly Token paren;
        public readonly List<Expression> args;
    }
    public class Get : Expression
    {
        public Get(Expression obj, Token name)
        {
            this.obj = obj;
            this.name = name;
        }

        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.visitGetExpr(this);
        }

        public readonly Expression obj;
        public readonly Token name;
    }
    public class Grouping : Expression
    {
        public Grouping(Expression expr)
        {
            this.expr = expr;
        }

        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.visitGroupingExpr(this);
        }

        public readonly Expression expr;
    }
    public class Literal : Expression
    {
        public Literal(object? value)
        {
            this.value = value;
        }

        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.visitLiteralExpr(this);
        }

        public readonly object? value;
    }
    public class Logical : Expression
    {
        public Logical(Expression left, Token op, Expression right)
        {
            this.left = left;
            this.op = op;
            this.right = right;
        }

        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.visitLogicalExpr(this);
        }

        public readonly Expression left;
        public readonly Token op;
        public readonly Expression right;
    }
    public class Set : Expression
    {
        public Set(Expression obj, Token name, Expression value)
        {
            this.obj = obj;
            this.name = name;
            this.value = value;
        }

        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.visitSetExpr(this);
        }

        public readonly Expression obj;
        public readonly Token name;
        public readonly Expression value;
    }
    public class This : Expression
    {
        public This(Token keyword)
        {
            this.keyword = keyword;
        }

        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.visitThisExpr(this);
        }

        public readonly Token keyword;
    }
    public class Unary : Expression
    {
        public Unary(Token op, Expression right)
        {
            this.op = op;
            this.right = right;
        }

        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.visitUnaryExpr(this);
        }

        public readonly Token op;
        public readonly Expression right;
    }
    public class Variable : Expression
    {
        public Variable(Token name)
        {
            this.name = name;
        }

        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.visitVariableExpr(this);
        }

        public readonly Token name;
    }

}
