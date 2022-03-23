using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace trmpLox
{
    public class Environment
    {
        public readonly Dictionary<string, object> values = new();

        public object? Get(Token name)
        {
            if (values.ContainsKey(name.lexeme))
            {
                return values[name.lexeme];
            }

            else
            {
                Console.Write("Undefined variable ");
                Console.WriteLine(name.lexeme);
                return null;
            }
        }

        public void Define(string name, object? value)
        { 
            values.Add(name, value);
        }
    }
}
