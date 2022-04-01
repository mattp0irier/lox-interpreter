using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace trmpLox
{
    //runtimeError: runtimeErrors are unique exceptions that are thrown
    //when an invalid operation is attempted
    public class runtimeError : Exception
    {
        // keeps track of the problematic token
        public readonly Token token;

        // constructor includes Exception(string) constructor
        public runtimeError(Token token, string msg) : base(msg)
        {
            this.token = token;

        }
    }
}
