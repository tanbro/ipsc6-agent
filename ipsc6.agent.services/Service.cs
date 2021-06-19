using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ipsc6.agent.services
{
    class Service
    {
        public string Echo(string s) => s;
        public void Throw() => throw new Exception();

#pragma warning disable VSTHRD200
        public async Task<string> DelayEcho(string s, int milliseconds)
#pragma warning restore VSTHRD200
        {
            await Task.Delay(milliseconds);
            return s;
        }
    }
}
