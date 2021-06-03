using System;


namespace ipsc6.agent.client.Sip
{
    delegate void RegisterStateChangedEventHandler(object sender, EventArgs e);

    class IncomingCallEventArgs : EventArgs
    {
        public Call Call { get; }
        public IncomingCallEventArgs(Call call) : base()
        {
            Call = call;
        }
    }
    delegate void IncomingCallEventHandler(object sender, IncomingCallEventArgs e);

    delegate void CallDisconnectedEventHandler(object sender, EventArgs e);

}
