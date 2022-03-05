using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using org.pjsip.pjsua2;

namespace SipClientWinFormsDemo
{

    public delegate void SipRegisterStateChangedEventHandler(object sender, EventArgs e);

    internal class MySipAccount: Account
    {

        public event SipRegisterStateChangedEventHandler OnRegisterStateChanged;

        public override void onRegState(OnRegStateParam prm)
        {
            OnRegisterStateChanged?.Invoke(this, new EventArgs());
        }

    }
}
