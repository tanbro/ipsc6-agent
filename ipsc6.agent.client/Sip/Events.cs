using System;


namespace ipsc6.agent.client.Sip
{
    public delegate void RegisterStateChangedEventHandler(object sender, EventArgs e);

    public class CallEventArgs : EventArgs
    {
        public Call Call { get; }
        public CallEventArgs(Call call) : base()
        {
            Call = call;
        }
    }

    public delegate void IncomingCallEventHandler(object sender, CallEventArgs e);

    public delegate void CallDisconnectedEventHandler(object sender, CallEventArgs e);
    public delegate void CallStateChangedEventHandler(object sender, CallEventArgs e);



}
