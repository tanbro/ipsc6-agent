using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using org.pjsip.pjsua2;

namespace SipClientWinFormsDemo
{

    internal class MySipCallEventArgs : EventArgs
    {
        public MySipCall MySipCall { get; }
        public MySipCallEventArgs(MySipCall call) : base()
        {
            MySipCall = call;
        }
    }

    internal delegate void MySipIncomingCallEventHandler(object sender, MySipCallEventArgs e);
    internal delegate void MySipCallDisconnectedEventHandler(object sender, MySipCallEventArgs e);
    internal delegate void MySipCallStateChangedEventHandler(object sender, MySipCallEventArgs e);

    internal class MySipCall : Call
    {
        public MySipCall(Account acc, bool isIncoming = false) : base(acc)
        {
            IsIncoming = isIncoming;
        }

        public MySipCall(Account acc, int callId, bool isIncoming = true) : base(acc, callId)
        {
            IsIncoming = isIncoming;
        }

        public bool IsIncoming { get; }

        public event EventHandler OnDisconnected;
        public event EventHandler OnStateChanged;

        public override void onCallState(OnCallStateParam param)
        {
            var callInfo = getInfo();
            OnStateChanged?.Invoke(this, new EventArgs());
            if (callInfo.state == pjsip_inv_state.PJSIP_INV_STATE_DISCONNECTED)
            {
                OnDisconnected?.Invoke(this, new EventArgs());
                /* Schedule/Dispatch call deletion to another thread here */
                Task.Run(() =>
                {
                    Dispose();
                });
            }
        }

        public override void onCallMediaState(OnCallMediaStateParam prm)
        {
            var mgr = Form1.SipEndpoint.audDevManager();
            var play_dev_med = mgr.getPlaybackDevMedia();
            var cap_dev_med = mgr.getCaptureDevMedia();
            AudioMedia aud_med;
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
        }
    }


    internal class MySipAccount : Account
    {

        public event EventHandler OnRegisterStateChanged;
        public event MySipIncomingCallEventHandler OnIncomingCall2;
        public event MySipCallDisconnectedEventHandler OnCallDisconnected;
        public event MySipCallStateChangedEventHandler OnCallStateChanged;

        private readonly HashSet<MySipCall> calls = new();
        public IReadOnlyCollection<MySipCall> Calls => calls;

        public override void onRegState(OnRegStateParam prm)
        {
            OnRegisterStateChanged?.Invoke(this, new EventArgs());
        }

        public override void onIncomingCall(OnIncomingCallParam prm)
        {
            var call = new MySipCall(this, prm.callId);
            if (!calls.Add(call)) throw new InvalidOperationException();
            call.OnDisconnected += Call_OnDisconnected;
            call.OnStateChanged += Call_OnStateChanged;

            OnIncomingCall2?.Invoke(this, new MySipCallEventArgs(call));
        }

        private void Call_OnDisconnected(object sender, EventArgs e)
        {
            var call = sender as MySipCall;
            if (!calls.Remove(call)) throw new InvalidOperationException();
            OnCallDisconnected?.Invoke(call, new MySipCallEventArgs(call));
        }

        private void Call_OnStateChanged(object sender, EventArgs e)
        {
            var call = sender as MySipCall;
            OnCallStateChanged?.Invoke(this, new MySipCallEventArgs(call));
        }

    }
}
