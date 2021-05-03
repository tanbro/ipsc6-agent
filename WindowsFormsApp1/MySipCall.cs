using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using org.pjsip.pjsua2;

namespace WindowsFormsApp1
{
    public class MySipCall: Call
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(MySipCall));

        public readonly MySipAccount Account;

        public MySipCall(Account account, int callId) : base(account, callId)
        {
            logger.DebugFormat("ctor - {0}", getId());
            Account = account as MySipAccount;
        }

        public override void onCallState(OnCallStateParam param)
        {
            logger.DebugFormat("dtor - {0}", getId());
            var ci = getInfo();
            if (Account.getId() != ci.accId)
            {
                throw new Exception(string.Format("AccID={0} in call differs with {1}", ci.accId, Account.getId()));
            }

            var msg = string.Format("{0} {1}", ci.remoteUri, ci.state);
            //logger.InfoFormat(msg);
            var uri = Account.getInfo().uri;

            Account.Form.Invoke(new Action(() =>
            {
                Account.Form.SetSipAccountMessage(uri, msg);
            }));
            
            switch (ci.state)
            {
                case pjsip_inv_state.PJSIP_INV_STATE_DISCONNECTED:
                    /// TODO:
                    // /* Schedule/Dispatch call deletion to another thread here */
                    Task.Run(() =>
                    {
                        if (Form1.currSipCall != null)
                        {
                            if (getId() == Form1.currSipCall.getId())
                            {
                                Dispose();
                                Form1.currSipCall = null;
                            }
                            else
                            {
                                Dispose();
                            }
                        }
                    });
                    break;
                default:
                    break;
            }
        }

        public override void onCallMediaState(OnCallMediaStateParam prm)
        {
            var ci = getInfo();
            // Iterate all the call medias
            for (int i = 0; i < ci.media.Count(); i++)
            {
                if (ci.media[i].type == pjmedia_type.PJMEDIA_TYPE_AUDIO)
                {
                    var audMed = getAudioMedia(i);
                    // Connect the call audio media to sound device
                    var mgr = Account.Form.Endpoint.audDevManager();
                    audMed.startTransmit(mgr.getPlaybackDevMedia());
                    mgr.getCaptureDevMedia().startTransmit(audMed);
                }
            }
        }

    }
}
