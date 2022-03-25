using System;
namespace trmpLox
{
	public class Interpreter : Expression.Visitor<Object>, Statement.Visitor<object?>
	{
        private Environment environment = new();
		public Interpreter()
		{
		}

        private object evaluate(Expression expr)
        {
            return expr.Accept(this);
        }

        private object? Execute(Statement stmt)
        {
            return stmt.accept(this);
        }

        public object? visitExpressionStatement(ExpressionStmt stmt)
        {
            evaluate(stmt.expression);
            return null;
        }

        public object? visitPrintStatement(PrintStmt stmt)
        {
            object value = evaluate(stmt.expression);
            Console.WriteLine(stringify(value));
            return null;
        }

        public object? visitBlockStatement(BlockStmt stmt)
        {
            ExecuteBlock(stmt.statements, new Environment(environment));
            return null;
        }

        public object? visitFunctionStatement(FunctionStmt stmt)
        {
            Console.WriteLine("Not yet implemented");
            return null;
        }

        public object? visitIfStatement(IfStmt stmt)
        {
            Console.WriteLine("Not yet implemented");
            return null;
        }

        public object? visitReturnStatement(ReturnStmt stmt)
        {
            Console.WriteLine("Not yet implemented");
            return null;
        }

        public object? visitVarStatement(VarStmt stmt)
        {
            object? value = null;
            if (stmt.initializer != null)
            {
                value = evaluate(stmt.initializer);
            }

            environment.Define(stmt.name.lexeme, value);
            foreach (KeyValuePair<string, object> kvp in environment.values)
            {
                Console.WriteLine("Key: {0}, Value: {1}", kvp.Key, kvp.Value);
            }
            return null;
        }

        public object? visitWhileStatement(WhileStmt stmt)
        {
            Console.WriteLine("Not yet implemented");
            return null;
        }

        public void ExecuteBlock(List<Statement> statements, Environment environment)
        {
            Environment previous = this.environment;
            try
            {
                this.environment = environment;

                foreach (Statement statement in statements)
                {
                    Execute(statement);
                }
            }
            finally
            {
                this.environment = previous;
            }
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
            Object value = evaluate(expr.value);
            environment.Assign(expr.name, value);
            return value;
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
            return environment.Get(expr.name);
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

        public void interpret(List<Statement> statements)
        {
            foreach (Statement statement in statements)
            {
                Execute(statement);
            }
            //object value = evaluate(expression);
            //Console.WriteLine(stringify(value));
        }
    }
}

