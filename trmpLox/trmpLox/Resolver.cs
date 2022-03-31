using System;
using System.Collections;
namespace trmpLox
{
	public class Resolver : Expression.Visitor<object?>, Statement.Visitor<object?>
	{
		readonly Interpreter interpreter;
		readonly Stack<Dictionary<string, bool>> scopes = new Stack<Dictionary<string, bool>>();
		FunctionType currentFunction = FunctionType.NONE;

		public Resolver(Interpreter interpreter)
		{
			this.interpreter = interpreter;
		}

		public object? visitBlockStatement(BlockStmt stmt)
		{
			BeginScope();
			Resolve(stmt.statements);
			EndScope();
			return null;
		}

		public object? visitVarStatement(VarStmt stmt)
		{
			Declare(stmt.name);
			if (stmt.initializer != null)
			{
				Resolve(stmt.initializer);
			}
			Define(stmt.name);
			return null;
		}

		public object? visitVariableExpr(Variable expr)
		{
			if (scopes.Count != 0 && scopes.Peek().ContainsKey(expr.name.lexeme) && scopes.Peek()[expr.name.lexeme] == false)
			{
				TrMpLox.Error(expr.name, "Can't read local variable in its own initializer.");
			}

			ResolveLocal(expr, expr.name);
			return null;
		}

		public object? visitAssignExpr(Assign expr)
		{
			Resolve(expr.value);
			ResolveLocal(expr, expr.name);
			return null;
		}

		public object? visitFunctionStatement(FunctionStmt statement)
		{
			Declare(statement.name);
			Define(statement.name);

			ResolveFunction(statement, FunctionType.FUNCTION);
			return null;
		}

		public object? visitExpressionStatement(ExpressionStmt stmt)
		{
			Resolve(stmt.expression);
			return null;
		}

		public object? visitIfStatement(IfStmt stmt)
		{
			Resolve(stmt.condition);
			Resolve(stmt.trueBranch);
			if (stmt.falseBranch != null)
				Resolve(stmt.falseBranch);
			return null;
		}

		public object? visitPrintStatement(PrintStmt stmt)
		{
			Resolve(stmt.expression);
			return null;
		}

		public object? visitReturnStatement(ReturnStmt stmt)
		{
			if (currentFunction == FunctionType.NONE)
            {
				TrMpLox.Error(stmt.keyword, "Can't return from top-level code.");
				return null;
            }

			if (stmt.value != null)
			{
				Resolve(stmt.value);
			}
			return null;
		}

		public object? visitWhileStatement(WhileStmt stmt)
		{
			Resolve(stmt.condition);
			Resolve(stmt.body);
			return null;
		}

		public object? visitBinaryExpr(Binary expr)
		{
			Resolve(expr.left);
			Resolve(expr.right);
			return null;
		}

		public object? visitCallExpr(Call expr)
		{
			Resolve(expr.callee);

			foreach (Expression arg in expr.args)
			{
				Resolve(arg);
			}
			return null;
		}

		public object? visitGroupingExpr(Grouping expression)
		{
			Resolve(expression.expr);
			return null;
		}

		public object? visitLiteralExpr(Literal expr)
		{
			return null;
		}

		public object? visitLogicalExpr(Logical expr)
		{
			Resolve(expr.left);
			Resolve(expr.right);
			return null;
		}

		public object? visitUnaryExpr(Unary expr)
		{
			Resolve(expr.right);
			return null;
		}

		public object? visitGetExpr(Get expr)
		{
			throw new NotImplementedException();
		}

		public object? visitSetExpr(Set expr)
		{
			throw new NotImplementedException();
		}

		public object? visitThisExpr(This expr)
		{
			throw new NotImplementedException();
		}

		void ResolveFunction(FunctionStmt fun, FunctionType type)
		{
			FunctionType enclosingFunction = currentFunction;
			currentFunction = type;

			BeginScope();
			foreach (Token parameter in fun.parameters)
			{
				Declare(parameter);
				Define(parameter);
			}
			Resolve(fun.body);
			EndScope();
			currentFunction = enclosingFunction;
		}

		private void ResolveLocal(Expression expr, Token name)
		{
			for (int i = scopes.Count - 1; i >= 0; i--)
			{
				if (scopes.ElementAt(i).ContainsKey(name.lexeme))
				{
					interpreter.Resolve(expr, scopes.Count - 1 - i);
					return;
				}
			}
		}

		private void Declare(Token token)
		{
			if (scopes.Count == 0) return;

			Dictionary<string, bool> scope = scopes.Peek();

			if(scope.ContainsKey(token.lexeme)) { 
				TrMpLox.Error(token, "Already a variable with this name in the scope."); 
			}
			else scope.Add(token.lexeme, false);
		}

		private void Define(Token token)
		{
			if (scopes.Count == 0) return;
			if (scopes.Peek().ContainsKey(token.lexeme)) scopes.Peek()[token.lexeme] = true;
			else scopes.Peek().Add(token.lexeme, true);
		}

		public void Resolve(List<Statement> statements)
		{
			foreach (Statement stmt in statements)
			{
				Resolve(stmt);
			}
		}

		private void Resolve(Expression expr)
		{
			expr.Accept(this);
		}

		private void Resolve(Statement stmt)
		{
			stmt.accept(this);
		}

		private void BeginScope()
		{
			scopes.Push(new Dictionary<string, bool>());
		}

		private void EndScope()
		{
			scopes.Pop();
		}
	}

	enum FunctionType
    {
		NONE,
		FUNCTION
    };
}

