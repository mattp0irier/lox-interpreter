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
            Binary(Expression left, Token op, Expression right)
            {
                this.left = left;
                this.op = op;
                this.right = right;
            }

            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.visitBinaryExpr(this);
            }

            readonly Expression left;
            readonly Token op;
            readonly Expression right;
        }
        public class Call : Expression
        {
            Call(Expression callee, Token paren, List<Expression> args)
            {
                this.callee = callee;
                this.paren = paren;
                this.args = args;
            }

            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.visitCallExpr(this);
            }

            readonly Expression callee;
            readonly Token paren;
            readonly List<Expression> args;
        }
        public class Get : Expression
        {
            Get(Expression obj, Token name)
            {
                this.obj = obj;
                this.name = name;
            }

            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.visitGetExpr(this);
            }

            readonly Expression obj;
            readonly Token name;
        }
        public class Grouping : Expression
        {
            Grouping(Expression expr)
            {
                this.expr = expr;
            }

            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.visitGroupingExpr(this);
            }

            readonly Expression expr;
        }
        public class Literal : Expression
        {
            Literal(object value)
            {
                this.value = value;
            }

            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.visitLiteralExpr(this);
            }

            readonly object value;
        }
        public class Logical : Expression
        {
            Logical(Expression left, Token op, Expression right)
            {
                this.left = left;
                this.op = op;
                this.right = right;
            }

            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.visitLogicalExpr(this);
            }

            readonly Expression left;
            readonly Token op;
            readonly Expression right;
        }
        public class Set : Expression
        {
            Set(Expression obj, Token name, Expression value)
            {
                this.obj = obj;
                this.name = name;
                this.value = value;
            }

            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.visitSetExpr(this);
            }

            readonly Expression obj;
            readonly Token name;
            readonly Expression value;
        }
        public class This : Expression
        {
            This(Token keyword)
            {
                this.keyword = keyword;
            }

            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.visitThisExpr(this);
            }

            readonly Token keyword;
        }
        public class Unary : Expression
        {
            Unary(Token op, Expression right)
            {
                this.op = op;
                this.right = right;
            }

            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.visitUnaryExpr(this);
            }

            readonly Token op;
            readonly Expression right;
        }
        public class Variable : Expression
        {
            Variable(Token name)
            {
                this.name = name;
            }

            public override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.visitVariableExpr(this);
            }

            readonly Token name;
        }

        public abstract T Accept<T>(Visitor<T> visitor);
    }
}
