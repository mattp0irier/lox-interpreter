using System;
namespace trmpLox
{
	public class Interpreter : Expression.Visitor<object>, Statement.Visitor<object?>
	{
        public class ClockCallable : LoxCallable
        {
            public int Arity() { return 0; }
            public object Call(Interpreter interpreter, List<object> arguments)
            {
                return (double)DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            }

            public string toString() { return "<native fn>"; }
        };

        public readonly Environment globals = new();
        private readonly Dictionary<Expression, int> locals = new Dictionary<Expression, int>();
        private Environment environment;

        public Interpreter()
		{
            environment = globals;
            globals.Define("clock", new ClockCallable());
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
            LoxFunction function = new(stmt, environment);
            environment.Define(stmt.name.lexeme, function);
            return null;
        }

        public object? visitIfStatement(IfStmt stmt)
        {
            if (isTruthy(evaluate(stmt.condition)))
            {
                Execute(stmt.trueBranch);
            }
            else if (stmt.falseBranch != null)
            {
                Execute(stmt.falseBranch);
            }
            return null;
        }

        public object? visitReturnStatement(ReturnStmt stmt)
        {
            object? value = null;
            if (stmt.value != null) value = evaluate(stmt.value);

            throw new Return(value);
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
            while (isTruthy(evaluate(stmt.condition)))
            {
                Execute(stmt.body);
            }
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
            if (obj is bool)
            {
                return (bool)obj;
            }
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
            object value = evaluate(expr.value);

            if (locals.ContainsKey(expr))
            {
                int? distance = locals[expr];
                environment.AssignAt(distance, expr.name, value);
            }
            else
            {
                globals.Assign(expr.name, value);
            }
            
            return value;
        }

        public object visitBinaryExpr(Binary expr)
        {
            object left = evaluate(expr.left);
            object right = evaluate(expr.right);

            switch (expr.op.GetType())
            {
                case TokenType.MINUS:
                    CheckNumOperands(expr.op, left, right);
                    return (double)left - (double)right;
                case TokenType.SLASH:
                    CheckNumOperands(expr.op, left, right);
                    return (double)left / (double)right;
                case TokenType.STAR:
                    CheckNumOperands(expr.op, left, right);
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
                    throw new runtimeError(expr.op, "Operands must be two numbers or two strings.");
                case TokenType.GREATER:
                    CheckNumOperands(expr.op, left, right);
                    return (double)left > (double)right;
                case TokenType.GREATER_EQUAL:
                    CheckNumOperands(expr.op, left, right);
                    return (double)left >= (double)right;
                case TokenType.LESS:
                    CheckNumOperands(expr.op, left, right);
                    return (double)left < (double)right;
                case TokenType.LESS_EQUAL:
                    CheckNumOperands(expr.op, left, right);
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
            object callee = evaluate(expr.callee);

            List<object> arguments = new List<object>();
            foreach (Expression argument in expr.args)
            {
                arguments.Add(evaluate(argument));
            }

            if (!(callee is LoxCallable)) {
                Console.WriteLine("Can only call functions and classes.");
            }

            LoxCallable function = (LoxCallable)callee;
            if (arguments.Count != function.Arity())
            {
                Console.WriteLine("Expected " + function.Arity() + " arguments but got " + arguments.Count + ".");
            }
            return function.Call(this, arguments);
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
            object left = evaluate(expr.left);

            if (expr.op.type == TokenType.OR)
            {
                if (isTruthy(left)) return left;
            }
            else
            {
                if (!isTruthy(left)) return left;
            }

            return evaluate(expr.right);
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
                    CheckNumOperand(expr.op, right);
                    return -(double)right;
                case TokenType.BANG:
                    return !isTruthy(right);
                default:
                    return null;
            }
        }

        void CheckNumOperand(Token op, object operand)
        {
            if (operand is double) return;
            throw new runtimeError(op, "Operand must be a number.");
        }

        void CheckNumOperands(Token op, object operand1, object operand2)
        {
            if (operand1 is double && operand2 is double) return;
            throw new runtimeError(op, "Operands must be numbers.");
        }

        public object visitVariableExpr(Variable expr)
        {
            return LookUpVariable(expr.name, expr);
        }

        private object? LookUpVariable(Token name, Expression expr)
        {
            int? distance = null;
            if (locals.ContainsKey(expr)) distance = locals[expr];
            if (distance != null)
            {
                return environment.GetAt(distance, name.lexeme);
            }
            else
            {
                return globals.Get(name);
            }
        }

        private string stringify(object obj)
        {
            if (obj == null)
                return "nil";

            if (obj is double) {
                string? text = obj.ToString();
                if (text.EndsWith(".0"))
                {
                    text = text.Substring(0, text.Length - 2);
                }
                return text;
            }

            return obj.ToString();
        }

        public void Resolve(Expression expr, int depth)
        {
            locals.Add(expr, depth);
        }

        public void interpret(List<Statement> statements)
        {
            try
            {
                foreach (Statement statement in statements)
                {
                    Execute(statement);
                }
            }
            catch (runtimeError error)
            {
                TrMpLox.RuntimeError(error);
            }
        }
    }
}

