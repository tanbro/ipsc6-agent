# 排队信息相关事件

## 收到排队消息

当与该座席相关的的排队发生变化时，CTI 服务器将排队信息通知给座席。

-   **Method**: `onQueueInfoReceived`

-   **Params**:

    | Argument    | Type     | Default | Description                            |
    | ----------- | -------- | ------- | -------------------------------------- |
    | `queueInfo` | `Object` | -       | 状态发生变化的呼叫([QueueInfo][]) 对象 |

[queueinfo]: ../types/queue_info.md

--8<-- "includes/glossary.md"
