using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace trmpLox
{
    internal class Return : Exception
    {
        public readonly object? value;

        public Return(object? value)
        {
            // base(null, null, false, false);
            this.value = value;
        }
    }
}
