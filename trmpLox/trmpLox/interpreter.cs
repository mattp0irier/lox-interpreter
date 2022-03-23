using System;
namespace trmpLox
{
	public class Interpreter : Expression.Visitor<Object>
	{
		public Interpreter()
		{
		}

        private object evaluate(Expression expr)
        {
            return expr.Accept(this);
        }

        private bool isTruthy(object obj)
        {
            if (obj == null) return false;
            if (obj is bool) return (bool)obj;
            return true;
        }

        private bool isEqual(Object a, Object b)
        {
            if (a == null && b == null)
                return true;
            if (a == null)
                return false;

            return a.Equals(b);
        }

        public object visitAssignExpr(Assign expr)
        {
            throw new NotImplementedException();
        }

        public object visitBinaryExpr(Binary expr)
        {
            object left = evaluate(expr.left);
            object right = evaluate(expr.right);

            switch (expr.op.GetType())
            {
                case TokenType.MINUS:
                    return (double)left - (double)right;
                case TokenType.SLASH:
                    return (double)left / (double)right;
                case TokenType.STAR:
                    return (double)left * (double)right;
                case TokenType.PLUS:
                    if (left is double && right is double)
                    {
                        return (double)left + (double)right;
                    }

                    if (left is string && right is string)
                    {
                        return (string)left + (string)right;
                    }
                    break;
                case TokenType.GREATER:
                    return (double)left > (double)right;
                case TokenType.GREATER_EQUAL:
                    return (double)left >= (double)right;
                case TokenType.LESS:
                    return (double)left < (double)right;
                case TokenType.LESS_EQUAL:
                    return (double)left <= (double)right;
                case TokenType.BANG_EQUAL:
                    return !isEqual(left, right);
                case TokenType.EQUAL_EQUAL:
                    return isEqual(left, right);
                default:
                    return null;
            }
            return null;
        }

        public object visitCallExpr(Call expr)
        {
            throw new NotImplementedException();
        }

        public object visitGetExpr(Get expr)
        {
            throw new NotImplementedException();
        }

        public object visitGroupingExpr(Grouping expr)
        {
            return evaluate(expr.expr);
        }

        public object visitLiteralExpr(Literal expr)
		{
			return expr.value;
		}

        public object visitLogicalExpr(Logical expr)
        {
            throw new NotImplementedException();
        }

        public object visitSetExpr(Set expr)
        {
            throw new NotImplementedException();
        }

        public object visitThisExpr(This expr)
        {
            throw new NotImplementedException();
        }

        public object visitUnaryExpr(Unary expr)
        {
            object right = evaluate(expr.right);
            switch (expr.op.GetType())
            {
                case TokenType.MINUS:
                    return -(double)right;
                case TokenType.BANG:
                    return !isTruthy(right);
                default:
                    return null;
            }
        }

        public object visitVariableExpr(Variable expr)
        {
            throw new NotImplementedException();
        }

        private String stringify(Object obj)
        {
            if (obj == null)
                return "nil";

            if (obj is double) {
                string text = obj.ToString();
                if (text.EndsWith(".0"))
                {
                    text = text.Substring(0, text.Length - 2);
                }
                return text;
            }

            return obj.ToString();
        }

        public void interpret(Expression expression)
        {
            object value = evaluate(expression);
            Console.WriteLine(stringify(value));
        }
    }
}

