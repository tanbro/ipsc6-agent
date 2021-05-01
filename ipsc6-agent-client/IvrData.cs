using System;
using System.Collections.Generic;
using System.Text;

namespace ipsc6.agent.client
{
    public class IvrData: ServerSideData
    {
        public readonly int N;
        public readonly string S;
        public IvrData(ConnectionInfo connectionInfo, int n, string s) : base(connectionInfo)
        {
            N = n;
            S = s;
        }
    }
}
