using System;


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
        }

        public int Id { get; private set; }
        public bool IsValid { get; private set; }
        public bool IsRegisterActive { get; private set; }
        public int LastRegisterError { get; private set; }
    }
}
