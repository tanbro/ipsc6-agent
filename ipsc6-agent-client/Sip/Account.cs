using System;
using System.Collections.Generic;
using System.Text;

namespace ipsc6.agent.client.Sip
{
    class Account : org.pjsip.pjsua2.Account
    {
        static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(Account));

        public Account() : base() { }

        ~Account()
        {
            shutdown();
        }

        public override void onRegState(org.pjsip.pjsua2.OnRegStateParam param)
        {
            var info = getInfo();
            logger.DebugFormat("RegState: {0} {1} {2}", info.uri, param.code);
        }

        public override void onIncomingCall(org.pjsip.pjsua2.OnIncomingCallParam param)
        {

        }

    }
}
