using System;
using System.Collections;
namespace trmpLox
{
	// Resolver class implements Expression.Visitor and Statement.Visitor
	public class Resolver : Expression.Visitor<object?>, Statement.Visitor<object?>
	{
		readonly Interpreter interpreter;
		readonly Stack<Dictionary<string, bool>> scopes = new Stack<Dictionary<string, bool>>(); // stack to keep track of variable scopes
		FunctionType currentFunction = FunctionType.NONE;

		// Resolver: constructor taking intepreter as parameter
		public Resolver(Interpreter interpreter)
		{
			this.interpreter = interpreter;
		}

		// visitBlockStatement: implement visit method for BlockStmt
		public object? visitBlockStatement(BlockStmt stmt)
		{
			BeginScope(); // create new scope for block
			Resolve(stmt.statements); // resolve variables in that scope
			EndScope(); // exit scope
			return null;
		}

		// visitVarStatement: implement visit method for VarStmt
		public object? visitVarStatement(VarStmt stmt)
		{
			Declare(stmt.name); // initialize variable
			// if statement has value, resolve
			if (stmt.initializer != null)
			{
				Resolve(stmt.initializer);
			}
			// assign value
			Define(stmt.name);
			return null;
		}

		// visitVariableExpr: implement visit method for Variable
		public object? visitVariableExpr(Variable expr)
		{
			// if variable is being accessed while also being initialized, error
			if (scopes.Count != 0 && scopes.Peek().ContainsKey(expr.name.lexeme) && scopes.Peek()[expr.name.lexeme] == false)
			{
				TrMpLox.Error(expr.name, "Can't read local variable in its own initializer.");
			}

			// else resolve local variable
			ResolveLocal(expr, expr.name);
			return null;
		}

		// visitAssignExpr: implement visit method for Assign
		public object? visitAssignExpr(Assign expr)
		{
			// resolve expression if it contains other variables
			Resolve(expr.value);
			// resolve expression to its assigned variable
			ResolveLocal(expr, expr.name);
			return null;
		}

		// visitFunctionStatement: implement visit method for FunctionStmt
		public object? visitFunctionStatement(FunctionStmt statement)
		{
			// declare and define function in current scope
			Declare(statement.name);
			Define(statement.name);

			// resolve function body
			ResolveFunction(statement, FunctionType.FUNCTION);
			return null;
		}

		// visitExpressionStatement: implement visit method for ExpressionStmt
		public object? visitExpressionStatement(ExpressionStmt stmt)
		{
			Resolve(stmt.expression); // resolve variables in expression statement
			return null;
		}

		// visitIfStatement: implement visit method for IfStmt
		public object? visitIfStatement(IfStmt stmt)
		{
			Resolve(stmt.condition); // resolve variables in condition
			Resolve(stmt.trueBranch); // resolve variables in true branch
			if (stmt.falseBranch != null) // if there is an else branch, resolve variables there
				Resolve(stmt.falseBranch);
			return null;
		}

		// visitPrintStatement: implement visit method for PrintStmt
		public object? visitPrintStatement(PrintStmt stmt)
		{
			Resolve(stmt.expression); // resolve variables in print statement
			return null;
		}

		// visitReturnStatement: implement visit method for ReturnStmt
		public object? visitReturnStatement(ReturnStmt stmt)
		{
			// error if return statement is in top-level code
			if (currentFunction == FunctionType.NONE)
            {
				TrMpLox.Error(stmt.keyword, "Can't return from top-level code.");
				return null;
            }

			// if return value is not null, resolve it
			if (stmt.value != null)
			{
				Resolve(stmt.value);
			}
			return null;
		}

		// visitWhileStatement: implement visit method for WhileStmt
		public object? visitWhileStatement(WhileStmt stmt)
		{
			Resolve(stmt.condition); // resolve while condition
			Resolve(stmt.body); // resolve while body
			return null;
		}

		// visitBinaryExpr: implement visit method for Binary
		public object? visitBinaryExpr(Binary expr)
		{
			Resolve(expr.left); // resolve left side of expression
			Resolve(expr.right); // resolve right side of expression
			return null;
		}

		// visitCallExpr: implement visit method for Call
		public object? visitCallExpr(Call expr)
		{
			Resolve(expr.callee); // resolve function name

			// resolve each argument in argument list
			foreach (Expression arg in expr.args)
			{
				Resolve(arg);
			}
			return null;
		}

		// visitGroupingExpr: implement visit method for Grouping
		public object? visitGroupingExpr(Grouping expression)
		{
			Resolve(expression.expr); // resolve expression inside grouping
			return null;
		}

		// visitLiteralExpr: implement visit method for Literal
		public object? visitLiteralExpr(Literal expr)
		{
			return null; // literals don't need to be resolved!
		}

		// visitLogicalExpr: implement visit method for Logical
		public object? visitLogicalExpr(Logical expr)
		{
			Resolve(expr.left); // resolve left side of expression
			Resolve(expr.right); // resolve right side of expression
			return null;
		}

		// visitUnaryExpr: implement visit method for Unary
		public object? visitUnaryExpr(Unary expr)
		{
			Resolve(expr.right); //resolve right side of unary expression
			return null;
		}

		//public object? visitGetExpr(Get expr)
		//{
		//	throw new NotImplementedException();
		//}

		//public object? visitSetExpr(Set expr)
		//{
		//	throw new NotImplementedException();
		//}

		//public object? visitThisExpr(This expr)
		//{
		//	throw new NotImplementedException();
		//}

		// ResolveFunction: Resolve the body of a function
		void ResolveFunction(FunctionStmt fun, FunctionType type)
		{
			FunctionType enclosingFunction = currentFunction; // establish we are in a function
			currentFunction = type;

			BeginScope(); // create new scope for function

			// declare and define variables in new scope
			foreach (Token parameter in fun.parameters)
			{
				Declare(parameter);
				Define(parameter);
			}
			Resolve(fun.body); // resolve function body
			EndScope(); // exit scope
			currentFunction = enclosingFunction; // reset function type to whatever it was before
		}

		// ResolveLocal: resolve variable by searching through each scope, from innermost outwards
		private void ResolveLocal(Expression expr, Token name)
		{
			for (int i = 0; i < scopes.ToArray().Length; i++) // loop through scopes from inside out
			{ 
				if (scopes.ToArray()[i].ContainsKey(name.lexeme)) // if the variable is defined in the scope
				{
					interpreter.Resolve(expr, i); // resolve variable at that distance
					return;
				}
			}
		}

		// Declare: add variable to dictionary with no value
		private void Declare(Token token)
		{
			if (scopes.Count == 0) return;

			Dictionary<string, bool> scope = scopes.Peek(); // look at current scope

			// if variable is already declared, error
			if(scope.ContainsKey(token.lexeme)) { 
				TrMpLox.Error(token, "Already a variable with this name in the scope."); 
			}
			// else add variable to scope with no value
			else scope.Add(token.lexeme, false);
		}

		// Declare: add variable to dictionary with no value
		private void Define(Token token)
		{
			if (scopes.Count == 0) return; // if no scopes, exit (can't define variable)
			// if current scope contains variable, update value
			if (scopes.Peek().ContainsKey(token.lexeme)) scopes.Peek()[token.lexeme] = true;
			// else add variable to current scope
			else scopes.Peek().Add(token.lexeme, true);
		}

		// Resolve: resolves a list of statements
		public void Resolve(List<Statement> statements)
		{
			foreach (Statement stmt in statements)
			{
				Resolve(stmt);
			}
		}

		// Resolve: resolves an expression
		private void Resolve(Expression expr)
		{
			expr.Accept(this);
		}

		// Resolve: resolves a statement
		private void Resolve(Statement stmt)
		{

			stmt.accept(this);
		}

		// BeginScope: create new scope and add it to stack
		private void BeginScope()
		{
			scopes.Push(new Dictionary<string, bool>());
		}

		// EndScope: pop front most scope off stack
		private void EndScope()
		{
			scopes.Pop();
		}
	}

	// FunctionType: enum for determining if we are in a function or in top-level code
	enum FunctionType
    {
		NONE,
		FUNCTION
    };
}

