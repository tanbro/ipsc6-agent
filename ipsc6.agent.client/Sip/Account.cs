using System;
using System.Linq;
using System.Collections.Generic;

namespace ipsc6.agent.client.Sip
{
    public class Account : org.pjsip.pjsua2.Account
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(Account));
        public int ConnectionIndex { get; }
        public string RingerWaveFile { get; }

        public Account(int connectionIndex, string ringerWaveFile = "") : base()
        {
            ConnectionIndex = connectionIndex;
            RingerWaveFile = ringerWaveFile;
            MakeString();
        }

        ~Account()
        {
            shutdown();
        }

        public event RegisterStateChangedEventHandler OnRegisterStateChanged;
        public event IncomingCallEventHandler OnIncomingCall;
        public event CallDisconnectedEventHandler OnCallDisconnected;
        public event CallStateChangedEventHandler OnCallStateChanged;

        private string _string;

        private string MakeString()
        {
            if (isValid())
            {
                var info = getInfo();
                _string = info.regIsConfigured
                    ? $"<{GetType().Name}@{GetHashCode():x8} Id={info.id}, Uri={info.uri}, RegisterStatus={info.regStatus}>"
                    : $"<{GetType().Name}@{GetHashCode():x8} Id={info.id}>";
            }
            else
            {
                _string = $"<{GetType().Name}@{GetHashCode():x8}>";
            }
            return _string;
        }
        public override void onRegState(org.pjsip.pjsua2.OnRegStateParam prm)
        {
            logger.DebugFormat("RegState - {0} : {1}", getInfo().uri, prm.code);
            MakeString();
            OnRegisterStateChanged?.Invoke(this, new EventArgs());
        }

        private readonly HashSet<Call> calls = new();
        public IReadOnlyCollection<Call> Calls => calls;

        private static readonly object ringerSentinel = new();
        private static org.pjsip.pjsua2.AudioMedia ringerMedia;
        private static org.pjsip.pjsua2.AudioMediaPlayer ringerPlayer;

        public override void onIncomingCall(org.pjsip.pjsua2.OnIncomingCallParam prm)
        {
            var call = new Call(this, prm.callId);
            if (!calls.Add(call)) throw new InvalidOperationException();
            call.OnDisconnected += Call_OnDisconnected;
            call.OnStateChanged += Call_OnStateChanged;
            logger.DebugFormat("IncomingCall - {0} : {1}", this, call);

            if (!string.IsNullOrWhiteSpace(RingerWaveFile))
            {
                lock (ringerSentinel)
                {
                    if (ringerPlayer == null)
                    {
                        ringerPlayer = new();
                        try
                        {
                            ringerMedia = org.pjsip.pjsua2.Endpoint.instance().audDevManager().getPlaybackDevMedia();
                            ringerPlayer.createPlayer(RingerWaveFile);
                            ringerPlayer.startTransmit(ringerMedia);
                        }
                        catch (Exception e)
                        {
                            ringerPlayer?.Dispose();
                            ringerPlayer = null;
                            logger.ErrorFormat("音频文件 {0} 启动播放错误: {1}", RingerWaveFile, e);
                        }
                    }
                }
            }

            OnIncomingCall?.Invoke(this, new CallEventArgs(call));
        }

        private void Call_OnStateChanged(object sender, EventArgs e)
        {
            var call = sender as Call;

            var ci = call.getInfo();
            org.pjsip.pjsua2.pjsip_inv_state[] states =
            {
                org.pjsip.pjsua2.pjsip_inv_state.PJSIP_INV_STATE_CONNECTING,
                org.pjsip.pjsua2.pjsip_inv_state.PJSIP_INV_STATE_CONFIRMED,
                org.pjsip.pjsua2.pjsip_inv_state.PJSIP_INV_STATE_DISCONNECTED,
            };

            if (states.Contains(call.getInfo().state))
            {
                lock (ringerSentinel)
                {
                    if (ringerPlayer != null)
                    {
                        try
                        {
                            ringerPlayer.stopTransmit(ringerMedia);
                        }
                        catch (Exception exception)
                        {
                            logger.ErrorFormat("音频文件 {0} 停止播放错误: {1}", RingerWaveFile, exception);
                        }
                        finally
                        {
                            ringerPlayer?.Dispose();
                            ringerPlayer = null;
                        }
                    }
                }
            }

            OnCallStateChanged?.Invoke(this, new CallEventArgs(call));
        }

        private void Call_OnDisconnected(object sender, EventArgs e)
        {
            var call = sender as Call;
            if (!calls.Remove(call)) throw new InvalidOperationException();
            logger.DebugFormat("CallDisconnected - {0} : {1}", this, call);
            OnCallDisconnected?.Invoke(call, new CallEventArgs(call));
        }

        public override string ToString()
        {
            return string.IsNullOrEmpty(_string) ? base.ToString() : _string;
        }

    }
}
