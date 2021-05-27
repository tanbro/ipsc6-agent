using System;

namespace ipsc6.agent.client.Sip
{
    class Account : org.pjsip.pjsua2.Account
    {
        static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(Account));
        public int ConnectionIndex { get; }

        public Account(int connectionIndex) : base()
        {
            ConnectionIndex = connectionIndex;
        }

        ~Account()
        {
            shutdown();
        }

        public event RegisterStateChangedEventHandler OnRegisterStateChanged;
        public event IncomingCallEventHandler OnIncomingCall;
        public event CallDisconnectedEventHandler OnCallDisconnected;

        public override void onRegState(org.pjsip.pjsua2.OnRegStateParam param)
        {
            logger.DebugFormat("RegState: {0} {1}", getInfo().uri, param.code);
            Agent.SyncFactory.StartNew(() =>
            {
                OnRegisterStateChanged?.Invoke(this, new EventArgs());
            }).Wait();
        }

        public override void onIncomingCall(org.pjsip.pjsua2.OnIncomingCallParam param)
        {
            var call = new Call(this, param.callId);
            logger.DebugFormat("IncomingCall: {0} {1}", getInfo().uri, call.getInfo().remoteUri);
            call.OnCallDisconnected += Call_OnCallDisconnected;
            Agent.SyncFactory.StartNew(() =>
            {
                OnIncomingCall?.Invoke(this, new IncomingCallEventArgs(call));
            }).Wait();
        }

        private void Call_OnCallDisconnected(object sender, EventArgs e)
        {
            var call = sender as Call;
            logger.DebugFormat("CallDisconnected: {0} {1}", getInfo().uri, call.getInfo().remoteUri);
            Agent.SyncFactory.StartNew(() =>
            {
                OnCallDisconnected?.Invoke(call, new EventArgs());
            }).Wait();
        }
    }
}
