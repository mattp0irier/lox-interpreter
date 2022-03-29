using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace trmpLox
{
    internal class LoxFunction : LoxCallable
    {
        readonly FunctionStmt declaration;

        public LoxFunction(FunctionStmt declaration)
        {
            this.declaration = declaration;
        }

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            Environment env = new(interpreter.globals);
            for (int i = 0; i < declaration.parameters.Count; i++)
            {
                env.Define(declaration.parameters[i].lexeme, arguments[i]);
            }

            try
            {
                interpreter.ExecuteBlock(declaration.body, env);
            }
            catch(Return returnValue)
            {
                return returnValue.value;
            }
            return null;
        }

        public int Arity() { return declaration.parameters.Count; }

        public string toString() { return "<fn " + declaration.name.lexeme + ">"; }
    }
}
