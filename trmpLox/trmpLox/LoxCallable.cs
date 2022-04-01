using System;
namespace trmpLox
{
	//LoxCallable interface: a callable object
	//must have an arity and some call function.
	public interface LoxCallable
	{
		//these are implemented in a callable object, in this case
		//it will get used to create the native clock() function.
		int Arity();
		Object Call(Interpreter interpreter, List<Object> arguments);
	}
}

