using System;
using System.Linq;
using System.Collections.Generic;


namespace ipsc6.agent.client
{
    public struct SipAccount
    {
        public SipAccount(Sip.Account account)
        {
            if (account is null)
            {
                throw new ArgumentNullException(nameof(account));
            }
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
                    IsRegisterActive = info.regIsActive;
                    LastRegisterError = info.regLastErr;
                }
                calls.UnionWith(
                    from call in account.Calls
                    select new SipCall(call)
                );
            }

        }

        public int Id { get; }
        public bool IsValid { get; }
        public bool IsRegisterActive { get; }
        public int LastRegisterError { get; }
        private readonly HashSet<SipCall> calls;
        public IReadOnlyCollection<SipCall> Calls => calls;
    }
}
