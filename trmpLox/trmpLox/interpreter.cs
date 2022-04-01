using System;
namespace trmpLox
{
    //Interpreter Class: implements visitor functions for expressions and statements
	public class Interpreter : Expression.Visitor<object>, Statement.Visitor<object?>
	{

        // Native function: clock() can be called to display the current time in milliseconds
        public class ClockCallable : LoxCallable
        {
            //clock() takes no arguments
            public int Arity() { return 0; }
            
            public object Call(Interpreter interpreter, List<object> arguments)
            {
                //DateTime can return current time
                return (double)DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            }

            // denotes clock() as a native function
            public string toString() { return "<native fn>"; }
        };

        // variable globals holds variables at the interpreter
        public readonly Environment globals = new();

        // for resolving variables at various depths
        private readonly Dictionary<Expression, int> locals = new Dictionary<Expression, int>();

        // holds interpreter environment
        private Environment environment;

        public Interpreter()
		{
            //upon declaration, native function clock() gets added
            environment = globals;
            globals.Define("clock", new ClockCallable());
		}

        //evaluate: evauates an expression using the interpreter object
        private object evaluate(Expression expr)
        {
            return expr.Accept(this);
        }

        //execute: executes a statement using the interpreter object
        private object? Execute(Statement stmt)
        {
            return stmt.accept(this);
        }

        //visiting an expression statement is the same thing as evaluating the expression that it holds
        public object? visitExpressionStatement(ExpressionStmt stmt)
        {
            evaluate(stmt.expression);
            return null;
        }

        //visiting a print statement is the same thing as evaluating its expression and writing it
        //to the console
        public object? visitPrintStatement(PrintStmt stmt)
        {
            object value = evaluate(stmt.expression);
            Console.WriteLine(stringify(value));
            return null;
        }

        //visiting a block statement also creates a new environment because it has {}
        public object? visitBlockStatement(BlockStmt stmt)
        {
            ExecuteBlock(stmt.statements, new Environment(environment));
            return null;
        }

        //visiting a function statement involves creating a new function object and adding it to the
        //environment
        public object? visitFunctionStatement(FunctionStmt stmt)
        {
            LoxFunction function = new(stmt, environment);
            environment.Define(stmt.name.lexeme, function);
            return null;
        }

        //visiting an if statement involves either executing the true branch or the false branch
        //based on the truthiness of the condition
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

        //a return statement is either null or some value to be evaluated
        public object? visitReturnStatement(ReturnStmt stmt)
        {
            object? value = null;
            if (stmt.value != null) value = evaluate(stmt.value);

            //return statement is handled as an exception so that it breaks out of recursive calls
            //or can be handled without executing later code.
            throw new Return(value);
        }

        //a variable statement defines a new variable in the environment
        public object? visitVarStatement(VarStmt stmt)
        {
            object? value = null;

            // a variable definition may include an initializer
            if (stmt.initializer != null)
            {
                value = evaluate(stmt.initializer);
            }

            environment.Define(stmt.name.lexeme, value);
            /*
            foreach (KeyValuePair<string, object> kvp in environment.values)
            {
                Console.WriteLine("Key: {0}, Value: {1}", kvp.Key, kvp.Value);
            }
            */
            return null;
        }


        //visiting a while statement looks like a simple while loop based
        //on the truthiness of the condition
        public object? visitWhileStatement(WhileStmt stmt)
        {
            while (isTruthy(evaluate(stmt.condition)))
            {
                Execute(stmt.body);
            }
            return null;
        }

        //to execute a block statement, simply execute each statement one at a time
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

        //isTruthy evaluates to false for a null object (nil) or a false boolean value, and true otherwise
        private bool isTruthy(object obj)
        {
            if (obj == null) return false;
            if (obj is bool)
            {
                return (bool)obj;
            }
            return true;
        }


        //isEqual checks for equality between two objects
        private bool isEqual(Object a, Object b)
        {
            if (a == null && b == null)
                return true;
            if (a == null)
                return false;

            return a.Equals(b);
        }

        //to visit an assignment expression, find the variable in the local or global environment
        //and update the value
        public object visitAssignExpr(Assign expr)
        {
            // evaluate the right side of the expression
            object value = evaluate(expr.value);

            if (locals.ContainsKey(expr))
            {
                // variable exists locally
                int? distance = locals[expr];
                environment.AssignAt(distance, expr.name, value);
            }
            else
            {
                // try to assign to global environment
                globals.Assign(expr.name, value);
            }
            // returns the value assigned
            return value;
        }

        // visiting binary expressions
        public object visitBinaryExpr(Binary expr)
        {
            //evaluate left and right arguments
            object left = evaluate(expr.left);
            object right = evaluate(expr.right);

            switch (expr.op.GetType())
            {
                //for math operations, make sure arguments are numbers before evaluating
                //this is the purpose of CheckNumOperands before each return
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
                    // plus operator can also concatenate strings
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
                // == or != may compare something other than numbers
                case TokenType.BANG_EQUAL:
                    return !isEqual(left, right);
                case TokenType.EQUAL_EQUAL:
                    return isEqual(left, right);
                default:
                    return null;
            }
            return null;
        }

        // visiting a function call
        public object visitCallExpr(Call expr)
        {
            // evaluate the callee
            object callee = evaluate(expr.callee);

            List<object> arguments = new List<object>();
            foreach (Expression argument in expr.args)
            {
                // add evaluate arguments before running function
                arguments.Add(evaluate(argument));
            }

            // object must be a callable object
            if (!(callee is LoxCallable)) {
                Console.WriteLine("Can only call functions and classes.");
            }

            LoxCallable function = (LoxCallable)callee;
            // number of arguments must be proper
            if (arguments.Count != function.Arity())
            {
                Console.WriteLine("Expected " + function.Arity() + " arguments but got " + arguments.Count + ".");
            }
            // call function object with arguments
            return function.Call(this, arguments);
        }

        //public object visitGetExpr(Get expr)
        //{
        //    throw new NotImplementedException();
        //}

        // for grouping expression, simply evaluate the expression inside
        public object visitGroupingExpr(Grouping expr)
        {
            return evaluate(expr.expr);
        }

        // literal expression returns the value
        public object visitLiteralExpr(Literal expr)
		{
			return expr.value;
		}


        // visiting logical operations
        public object visitLogicalExpr(Logical expr)
        {
            // evalute left side
            object left = evaluate(expr.left);

            if (expr.op.type == TokenType.OR)
            {
                // lazy exeuction for an or expression
                if (isTruthy(left)) return left;
            }
            else
            {
                // lazy exeuction of an and statement
                if (!isTruthy(left)) return left;
            }
            
            // only evaluate the right side if necessary
            return evaluate(expr.right);
        }

        //public object visitSetExpr(Set expr)
        //{
        //    throw new NotImplementedException();
        //}

        //public object visitThisExpr(This expr)
        //{
        //    throw new NotImplementedException();
        //}

        // visiting a unary expression
        public object visitUnaryExpr(Unary expr)
        {
            // evaluate the right side
            object right = evaluate(expr.right);
            switch (expr.op.GetType())
            {
                case TokenType.MINUS:
                    // minus operator needs to be with a number
                    CheckNumOperand(expr.op, right);
                    return -(double)right;
                case TokenType.BANG:
                    // not operator
                    return !isTruthy(right);
                default:
                    return null;
            }
        }

        void CheckNumOperand(Token op, object operand)
        {
            // throws an error if operand is not a number
            if (operand is double) return;
            throw new runtimeError(op, "Operand must be a number.");
        }

        void CheckNumOperands(Token op, object operand1, object operand2)
        {
            // throws an error if both operands are not a number
            if (operand1 is double && operand2 is double) return;
            throw new runtimeError(op, "Operands must be numbers.");
        }

        //visiting a variable expression involves obtaining the variable's value
        public object visitVariableExpr(Variable expr)
        {
            return LookUpVariable(expr.name, expr);
        }

        //lookUpVariable finds a variable in an environment
        private object? LookUpVariable(Token name, Expression expr)
        {
            int? distance = null;
            if (locals.ContainsKey(expr)) distance = locals[expr];
            if (distance != null)
            {
                // variable exists in local
                return environment.GetAt(distance, name.lexeme);
            }
            else
            {
                // variable doesn't exist in local, find it in global
                return globals.Get(name);
            }
        }

        //stringify: turns some non-string objects into strings
        private string stringify(object obj)
        {
            // null is represented as nil
            if (obj == null)
                return "nil";

            // if double value is an integer, .0 gets truncated
            if (obj is double) {
                string? text = obj.ToString();
                if (text.EndsWith(".0"))
                {
                    text = text.Substring(0, text.Length - 2);
                }
                return text;
            }

            // otherwise, the object already has toString() defined elsewhere
            return obj.ToString();
        }

        //resolve: adds variable at a given depth within local
        public void Resolve(Expression expr, int depth)
        {
            locals.Add(expr, depth);
        }

        //interpret: executes statements in order
        public void interpret(List<Statement> statements)
        {
            // may throw a runtime exception
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

