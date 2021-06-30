using System;
using System.Collections.Generic;
using System.Linq;


namespace ipsc6.agent.client
{
    public struct SipAccount
    {
        public SipAccount(Sip.Account account)
        {
            account = account is not null ? account : throw new ArgumentNullException(nameof(account));
            ConnectionIndex = account.ConnectionIndex;
            Uri = "";
            IsRegisterActive = false;
            LastRegisterError = 0;
            calls = new HashSet<SipCall>();
            Id = account.getId();
            IsValid = account.isValid();
            if (IsValid)
            {
                var info = account.getInfo();
                if (info.regIsConfigured)
                {
                    Uri = info.uri;
                    IsRegisterActive = info.regIsActive;
                    LastRegisterError = info.regLastErr;
                }
                calls.UnionWith(
                    from call in account.Calls
                    select new SipCall(call)
                );
            }
        }

        public int ConnectionIndex { get; }

        public int Id { get; }
        public string Uri { get; }
        public bool IsValid { get; }
        public bool IsRegisterActive { get; }
        public int LastRegisterError { get; }
        private readonly HashSet<SipCall> calls;
        public IReadOnlyCollection<SipCall> Calls => calls;
    }
}
