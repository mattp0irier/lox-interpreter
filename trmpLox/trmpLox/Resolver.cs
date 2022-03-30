﻿using System;
using System.Collections;
namespace trmpLox
{
	public class Resolver : Expression.Visitor<object?>, Statement.Visitor<object?>
	{
		readonly Interpreter interpreter;
		readonly Stack<Dictionary<string, bool>> scopes = new Stack<Dictionary<string, bool>>();

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
			if (scopes.Count != 0 && scopes.Peek()[expr.name.lexeme] == false)
			{
				Console.WriteLine("Can't read local variable in its own initializer.");
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

			ResolveFunction(statement);
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

		void ResolveFunction(FunctionStmt fun)
		{
			BeginScope();
			foreach (Token parameter in fun.parameters)
			{
				Declare(parameter);
				Define(parameter);
			}
			Resolve(fun.body);
			EndScope();
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
			scope.Add(token.lexeme, false);
		}

		private void Define(Token token)
		{
			if (scopes.Count == 0) return;
			scopes.Peek().Add(token.lexeme, true);
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
}

