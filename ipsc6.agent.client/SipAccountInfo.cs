using System;
using System.Linq;
using System.Collections.Generic;


namespace ipsc6.agent.client
{
    public class SipAccountInfo
    {
        public SipAccountInfo(Sip.Account account)
        {
            if (account is null)
            {
                throw new ArgumentNullException(nameof(account));
            }
            Id = account.getId();
            IsValid = account.isValid();
            if (!IsValid)
            {
                return;
            }
            var info = account.getInfo();
            if (info.regIsConfigured)
            {
                IsRegisterActive = info.regIsActive;
                LastRegisterError = info.regLastErr;
            }
            Calls = new HashSet<SipCallInfo>(
                from call in account.Calls
                select new SipCallInfo(call)
            );
        }

        public int Id { get; }
        public bool IsValid { get; }
        public bool IsRegisterActive { get; }
        public int LastRegisterError { get; }

        public IReadOnlyCollection<SipCallInfo> Calls { get; }
    }
}
