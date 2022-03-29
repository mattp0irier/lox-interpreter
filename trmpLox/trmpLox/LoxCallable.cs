using System;
namespace trmpLox
{
	public interface LoxCallable
	{
		int Arity();
		Object Call(Interpreter interpreter, List<Object> arguments);
	}
}

