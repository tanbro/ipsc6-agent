# 软电话账户信息对象

该对象主要用于记录软电话的 [SIP][] 注册状态

其属性有：

|      Attribute       |   Type    |      Description      |
| -------------------- | --------- | --------------------- |
| `index`              | `Integer` | 对应的CTI服务器索引值 |
| `uri`                | `String`  | 账户 URI              |
| `registerStatusCode` | `Integer` | 最近的注册结果状态码  |

eg:

```json
{
    "index": 0,
    "uri": "sip:1001@10.10.100.21:5060",
    "registerStatusCode": 200
}
```

[SIP]: https://datatracker.ietf.org/doc/html/rfc3261 "Session Initiation Protocol"
