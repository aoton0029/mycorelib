using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Cmds
{
    public class ArgumentBuilder
    {
        private readonly List<string> _arguments;
        public ArgumentBuilder()
        {
            _arguments = new List<string>();
        }
        public ArgumentBuilder Add(string argument)
        {
            if (!string.IsNullOrWhiteSpace(argument))
            {
                _arguments.Add(argument);
            }
            return this;
        }
        public ArgumentBuilder AddRange(IEnumerable<string> arguments)
        {
            if (arguments != null)
            {
                foreach (var arg in arguments)
                {
                    Add(arg);
                }
            }
            return this;
        }
        public string Build()
        {
            return string.Join(" ", _arguments);
        }

        public override string ToString()
        {
            return Build();
        }
    }
}
