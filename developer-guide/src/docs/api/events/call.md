# 呼叫信息相关事件

## 收到振铃消息

当本座席被排队选中时，触发该事件

-   **Method**: `onRingCallReceived`

-   **Params**:

    | Argument   | Type      | Default | Description                               |
    | ---------- | --------- | ------- | ----------------------------------------- |
    | `ctiIndex` | `Integer` | -       | 选中该座席的排队对应的 CTI 服务器的索引值 |
    | `call`     | `Object`  | -       | 选中该座席的排队对应的呼叫([Call][]) 对象 |

## 收到通话保持消息

当通话保持相关的呼叫发生变化，包括进行保持、取消保持，皆触发该事件。

-   **Method**: `onHeldCallReceived`

-   **Params**:

    | Argument   | Type      | Default | Description                                     |
    | ---------- | --------- | ------- | ----------------------------------------------- |
    | `ctiIndex` | `Integer` | -       | 保持状态发生变化的呼叫对应的 CTI 服务器的索引值 |
    | `call`     | `Object`  | -       | 保持状态发生变化的呼叫([Call][]) 对象           |

[call]: ../types/call.md

--8<-- "includes/glossary.md"
