using System;


namespace ipsc6.agent.client.Sip
{
    public delegate void RegisterStateChangedEventHandler(object sender, EventArgs e);

    public class IncomingCallEventArgs : EventArgs
    {
        public Call Call { get; }
        public IncomingCallEventArgs(Call call) : base()
        {
            Call = call;
        }
    }
    public delegate void IncomingCallEventHandler(object sender, IncomingCallEventArgs e);

    public delegate void CallDisconnectedEventHandler(object sender, EventArgs e);

}
