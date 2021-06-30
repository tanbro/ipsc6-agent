# 本地 SIP 相关事件

## SIP 账户注册状态改变

-   **Method**: `onSipRegisterStateChanged`

-   **Params**: 无

该事件仅通知 SIP 注册状态发生改变，而没有提供具体的数据。如果需要这些数据，可以调用 [getSipAccounts][] 获取完整的 SIP 账户 [SipAccount][] 对象列表。

[sipaccount]: ../types/sip_account.md
[getsipaccounts]: ../methods/sip.md#获取本地-SIP-账户列表

--8<-- "includes/glossary.md"
