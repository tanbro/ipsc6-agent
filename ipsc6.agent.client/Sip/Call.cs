using System;
using System.Threading.Tasks;

namespace ipsc6.agent.client.Sip
{
    public class MyPjCall : org.pjsip.pjsua2.Call
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(MyPjCall));

        public MyPjCall(org.pjsip.pjsua2.Account acc) : base(acc)
        {
            MakeString();
        }

        public MyPjCall(org.pjsip.pjsua2.Account acc, int callId) : base(acc, callId)
        {
            MakeString();
        }

        private string _string;

        private string MakeString()
        {
            var info = getInfo();
            _string = $"<{GetType().Name}@{GetHashCode():x8} Id={info.id}, RemoteUri={info.remoteUri}, State={info.state}>";
            return _string;
        }

        public override void onCallState(org.pjsip.pjsua2.OnCallStateParam param)
        {
            logger.DebugFormat("CallState - {0} : {1}", this, param.e.type);

            MakeString();
            var callInfo = getInfo();
            OnStateChanged?.Invoke(this, new EventArgs());
            if (callInfo.state == org.pjsip.pjsua2.pjsip_inv_state.PJSIP_INV_STATE_DISCONNECTED)
            {
                OnDisconnected?.Invoke(this, new EventArgs());
                /* Schedule/Dispatch call deletion to another thread here */
                Task.Run(() =>
                {
                    Dispose();
                });
            }
        }

        public override void onCallMediaState(org.pjsip.pjsua2.OnCallMediaStateParam prm)
        {
            var mgr = Agent.SipEndpoint.audDevManager();
            var play_dev_med = mgr.getPlaybackDevMedia();
            var cap_dev_med = mgr.getCaptureDevMedia();
            org.pjsip.pjsua2.AudioMedia aud_med;
            try
            {
                // Get the first audio media
                aud_med = getAudioMedia(-1);
            }
            catch
            {
                return;
            }
            // This will connect the sound device/mic to the call audio media
            cap_dev_med.startTransmit(aud_med);
            // And this will connect the call audio media to the sound device/speaker
            aud_med.startTransmit(play_dev_med);

            /* Deprecated: for PJSIP version 2.8 or earlier
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
            */
        }

        public event EventHandler OnDisconnected;
        public event EventHandler OnStateChanged;

        public override string ToString()
        {
            return string.IsNullOrEmpty(_string) ? base.ToString() : _string;
        }
    }
}
