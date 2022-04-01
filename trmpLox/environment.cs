using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace trmpLox
{
    // Environment class
    public class Environment
    {
        public readonly Dictionary<string, object> values = new(); // holds variables names
        public readonly Environment? enclosing; // handles environments inside of environments

        // Environment: constructs new environment with no enclosing environment
        public Environment()
        {
            enclosing = null;
        }

        // Environment: constructs new environment with enclosing environment
        public Environment(Environment enclosing)
        {
            this.enclosing = enclosing;
        }

        // Get: gets value of variable if it exists by walking up the environment chain
        public object? Get(Token name)
        {
            // check if environment dictionary contains variable
            if (values.ContainsKey(name.lexeme))
            {
                return values[name.lexeme]; // return variable value
            }

            // if there is an enclosing environment, check it
            else if (enclosing != null)
                return enclosing.Get(name);

            // else variable does not exist
            else
            {
                Console.Write("Undefined variable ");
                Console.WriteLine(name.lexeme);
                return null;
            }
        }

        // Define: define new variable by adding it to the dictionary
        public void Define(string name, object? value)
        { 
            values.Add(name, value);
        }

        // Assign: update variable if it exists by walking up the environment chain
        public void Assign(Token name, Object value)
        {
            // if variable exists in environment, update value
            if (values.ContainsKey(name.lexeme))
            {
                values[name.lexeme] = value;
                return;
            }

            // else if there is an enclosing environment, try to assign it there
            else if (enclosing != null)
            {
                enclosing.Assign(name, value);
                return;
            }
        }

        // GetAt: goes to exact distance up environment chain to get variable
        public object? GetAt(int? distance, string name)
        {
            // if variable is defined in the environment in question, return the value
            if (Ancestor(distance).values.ContainsKey(name))
            {
                return Ancestor(distance).values[name];
            }
            // else return null
            return null;
        }

        // AssignAt: goes to exact distance up environment chain to assign variable
        public void AssignAt(int? distance, Token name, object value)
        {
            // if variable is defined in the environment in question, update the value
            if (Ancestor(distance).values.ContainsKey(name.lexeme)) {
                Ancestor(distance).values[name.lexeme] = value;
            }
            // else add the variable to the environment in question
            else
            {
                Ancestor(distance).values.Add(name.lexeme, value);
            }
        }

        // Ancestor: for a given distance, return the enviroment that is that distance up the environment chain
        Environment Ancestor(int? distance)
        {
            Environment? environment = this;
            for (int i = 0; i < distance; i++)
            {
                environment = environment.enclosing;
            }

            return environment;
        }
    }
}
