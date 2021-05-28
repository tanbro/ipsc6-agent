using System;
using System.Threading.Tasks;

namespace ipsc6.agent.client.Sip
{
    class Account : org.pjsip.pjsua2.Account
    {
        static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(Account));
        public int ConnectionIndex { get; }

        public Account(int connectionIndex) : base()
        {
            ConnectionIndex = connectionIndex;
            MakeString();
        }

        ~Account()
        {
            shutdown();
        }

        public event RegisterStateChangedEventHandler OnRegisterStateChanged;
        public event IncomingCallEventHandler OnIncomingCall;
        public event CallDisconnectedEventHandler OnCallDisconnected;

        string _string = null;
        string MakeString()
        {
            if (isValid())
            {
                var info = getInfo();
                if (info.regIsConfigured)
                {
                    _string = $"<{GetType().Name}@{GetHashCode():x8} Id={info.id}, Uri={info.uri}, RegisterStatus={info.regStatus}>";
                }
                else
                {
                    _string = $"<{GetType().Name}@{GetHashCode():x8} Id={info.id}>";
                }
            }
            else
            {
                _string = $"<{GetType().Name}@{GetHashCode():x8}>";
            }
            return _string;
        }
        public override void onRegState(org.pjsip.pjsua2.OnRegStateParam param)
        {
            logger.DebugFormat("RegState: {0} {1}", getInfo().uri, param.code);
            MakeString();
            Task.Run(() =>
            {
                OnRegisterStateChanged?.Invoke(this, new EventArgs());
            });
        }

        public override void onIncomingCall(org.pjsip.pjsua2.OnIncomingCallParam param)
        {
            var call = new Call(this, param.callId);
            call.OnCallDisconnected += Call_OnCallDisconnected;
            logger.DebugFormat("IncomingCall - {0} : {1}", this, call);
            Task.Run(() =>
            {
                OnIncomingCall?.Invoke(this, new IncomingCallEventArgs(call));
            });
        }

        private void Call_OnCallDisconnected(object sender, EventArgs e)
        {
            var call = sender as Call;
            logger.DebugFormat("CallDisconnected - {0} : {1}", this, call);
            Task.Run(() =>
            {
                OnCallDisconnected?.Invoke(call, new EventArgs());
            });
        }

        public override string ToString()
        {
            return string.IsNullOrEmpty(_string) ? base.ToString() : _string;
        }

    }
}
