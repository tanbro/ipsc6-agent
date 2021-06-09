# 排队

## 获取排队列表

- **Method**: `getQueueList`
- **Params**: 无
- **Result**: [排队信息对象][] 数组

[排队信息对象]: ../objects/queue_info.md

## 取排队

将正在队列中、尚未排到座席的来话直接分配到发起调用的座席

- **Method**: `dequeue`

- **Params**:

    | Argument  | Data Type | Default |           Description           |
    | --------- | --------- | ------- | ------------------------------- |
    | `index`   | `Integer` | `null`  | 目标来话所属于的CTI服务器索引值 |
    | `channel` | `Integer` | `null`  | 目标来话的通道号                |

- **Result**: `null`
