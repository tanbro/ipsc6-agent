using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using org.pjsip.pjsua2;

namespace WindowsFormsApp1
{
    
    public class MySipAccount: Account
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(MySipAccount));
        public readonly Form1 Form;
        public readonly int Index;
        public MySipAccount(int index, Form1 form) : base()
        {
            Index = index;
            Form = form;
        }

        ~MySipAccount()
        {
            shutdown();
        }

        public override void onRegState(OnRegStateParam param)
        {
            if (Form.IsDisposed) return;
            var msg = string.Format("RegState {0} {1}", (int)param.code, param.reason);
            //logger.InfoFormat(msg);
            Form.Invoke(new Action(() =>
            {
                Form.SetSipAccountMessage(Index, msg);
            }));
        }

        public override void onIncomingCall(OnIncomingCallParam param)
        {
            if (Form.IsDisposed) return;
            var call = new MySipCall(this, param.callId);
            if (Form1.currSipCall !=null)
            {
                using (var op = new CallOpParam
                {
                    statusCode = pjsip_status_code.PJSIP_SC_DECLINE
                })
                {
                    call.hangup(op);
                }
                return;
            }
            else
            {
                Form1.currSipCall = call;
                var ci = call.getInfo();
                var msg = string.Format("{0} {1}", ci.remoteUri, ci.state);
                //logger.InfoFormat(msg);
                Form.Invoke(new Action(() =>
                {
                    Form.SetSipAccountMessage(Index, msg);
                }));
            }
        }
    }
}
