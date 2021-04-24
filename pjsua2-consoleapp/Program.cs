using System;
using System.Threading;
using org.pjsip.pjsua2;

namespace pjsua2_consoleapp
{
    // Subclass to extend the Account and get notifications etc.
    public class MyAccount : Account
    {
        override public void onRegState(OnRegStateParam prm)
        {
            Console.WriteLine("*** On registration state: " + prm.code + prm.reason);
        }
    }

    class Program
    {
        static void Main(string[] _)
        {
            try
            {
                // Create endpoint
                using Endpoint ep = new();
                ep.libCreate();
                try
                {
                    // Initialize endpoint
                    EpConfig epConfig = new();
                    ep.libInit(epConfig);
                    // Create SIP transport. Error handling sample is shown
                    TransportConfig sipTpConfig = new();
                    sipTpConfig.port = 5060;
                    ep.transportCreate(pjsip_transport_type_e.PJSIP_TRANSPORT_UDP, sipTpConfig);
                    // Start the library
                    ep.libStart();

                    AccountConfig acfg = new();
                    acfg.idUri = "sip:test@pjsip.org";
                    acfg.regConfig.registrarUri = "sip:pjsip.org";
                    AuthCredInfo cred = new("digest", "*", "test", 0, "secret");
                    acfg.sipConfig.authCreds.Add(cred);

                    // Create the account
                    using MyAccount acc = new();

                    // Here we don't have anything else to do..
                    Thread.Sleep(10000);

                }
                finally
                {
                    // Explicitly destroy and delete endpoint
                    ep.libDestroy();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
                return;
            }
        }
    }
}
