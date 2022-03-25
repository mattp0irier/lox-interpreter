﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace trmpLox
{
    public class Environment
    {
        public readonly Dictionary<string, object> values = new();
        public readonly Environment? enclosing;

        public Environment()
        {
            enclosing = null;
        }

        public Environment(Environment enclosing)
        {
            this.enclosing = enclosing;
        }

        public object? Get(Token name)
        {
            if (values.ContainsKey(name.lexeme))
            {
                return values[name.lexeme];
            }

            else if (enclosing != null)
                return enclosing.Get(name);

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

        public void Assign(Token name, Object value)
        {
            if (values.ContainsKey(name.lexeme))
            {
                values[name.lexeme] = value;
                return;
            }
            else if (enclosing != null)
            {
                enclosing.Assign(name, value);
                return;
            }
        }
    }
}
