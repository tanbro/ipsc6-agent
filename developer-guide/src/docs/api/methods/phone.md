# 软电话

## 摘机

- **Method**: `offHook`
- **Params**: 无
- **Result**: `null`

当座席收到来电振铃时，调用该方法将导致本地软电话直接进行呼叫应答，从而建立通话。

否则，座席会将“摘机”请求发送给服务器，由CTI服务器向该座席进行呼叫，座席在收到呼叫时自动应答。

## 挂机

- **Method**: `onHook`
- **Params**: 无
- **Result**: `null`

调用后，座席程序的本地软电话模块将挂断全部呼叫。

## 获取软电话列表

- **Method**: `getPhoneAccountList`
- **Params**: 无
- **Result**: [软电话账户信息对象][] 数组

[软电话账户信息对象]: ../objects/phone_account_info.md