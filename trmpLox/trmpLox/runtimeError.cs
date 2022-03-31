using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace trmpLox
{
    public class runtimeError : Exception
    {
        public readonly Token token;

        public runtimeError(Token token, string msg) : base(msg)
        {
            this.token = token;

        }
    }
}
