using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace trmpLox
{

    //LoxFunction implements LoxCallable
    internal class LoxFunction : LoxCallable
    {
        // functions have a declaration and a local environment (closure)
        readonly FunctionStmt declaration;
        readonly Environment closure;

        //basic constructor
        public LoxFunction(FunctionStmt declaration, Environment closure)
        {
            this.closure = closure;
            this.declaration = declaration;
        }

        //calling a function requires an interpreter and arguments
        public object Call(Interpreter interpreter, List<object> arguments)
        {
            //functions run within an environment
            Environment env = new Environment(closure);
            for (int i = 0; i < declaration.parameters.Count; i++)
            {
                //define function parameters with arguments
                env.Define(declaration.parameters[i].lexeme, arguments[i]);
            }

            try
            {
                //execute function body
                interpreter.ExecuteBlock(declaration.body, env);
            }
            catch(Return returnValue)
            {
                // return value is caught as an exception
                return returnValue.value;
            }
            return null;
        }

        // every function delcaration has 0+ parameters
        public int Arity() { return declaration.parameters.Count; }

        // toString() method is simply <fn functionName>
        public string toString() { return "<fn " + declaration.name.lexeme + ">"; }
    }
}
