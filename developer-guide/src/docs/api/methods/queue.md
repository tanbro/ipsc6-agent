# 排队信息相关方法

## 获取排队信息列表

获取当前座席相关的排队信息对象数组。

-   **Method**: `getQueueInfos`

-   **Params**: 无

-   **Result**: `Array`

    数组元素是 [QueueInfo][] 对象

## 排队抢接

将正在队列中、尚未接通的来话直接分配到发起调用的座席。

-   **Method**: `dequeue`

-   **Params**:

    | Argument   | Type      | Default | Description                       |
    | ---------- | --------- | ------- | --------------------------------- |
    | `ctiIndex` | `Integer` | -       | 目标来话所属于的 CTI 服务器索引值 |
    | `channel`  | `Integer` | -       | 目标来话的通道号                  |

-   **Result**: `null`

[queueinfo]: ../types/queue_info.md "排队信息"

--8<-- "includes/glossary.md"
