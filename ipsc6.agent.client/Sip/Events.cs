using System;


namespace ipsc6.agent.client.Sip
{
    public delegate void RegisterStateChangedEventHandler(object sender, EventArgs e);

    public class CallEventArgs : EventArgs
    {
        public MyPjCall Call { get; }
        public CallEventArgs(MyPjCall call) : base()
        {
            Call = call;
        }
    }

    public delegate void IncomingCallEventHandler(object sender, CallEventArgs e);

    public delegate void CallDisconnectedEventHandler(object sender, CallEventArgs e);
    public delegate void CallStateChangedEventHandler(object sender, CallEventArgs e);



}
