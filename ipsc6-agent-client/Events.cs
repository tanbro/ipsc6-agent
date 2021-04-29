using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ipsc6.agent.network;

namespace ipsc6.agent.client
{
    public delegate void ServerSendEventHandler(object sender, AgentMessageReceivedEventArgs e);
    public delegate void ClosedEventHandler(Object sender);
    public delegate void LostEventHandler(Object sender);
}
