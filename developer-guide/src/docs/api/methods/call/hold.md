# 通话保持

## 取得通话保持列表

- **Method**: `getHoldList`

- **Params**: 无

- **Result**:

    [通话保持对象][] 数组

## 保持当前通话

- **Method**: `hold`
- **Params**: 无
- **Result**: `null`

将当前正在进行的通话放到服务器端“保持”列表。

操作完成后，对端进入保持队列，听保持音乐；双方相互听不到。

## 取消保持

- **Method**: `unHold`

- **Params**:

    | Argument  | Data Type | Default |                Description                |
    | --------- | --------- | ------- | ----------------------------------------- |
    | `index`   | `Integer` | -       | 要取消保持的通话所属于的CTI服务器索引值 |
    | `channel` | `Integer` | -       | 要取消保持的通话的通道号                  |

    `index` 与 `channel` 应通过 [取得通话保持列表](#取得通话保持列表) 读取。

- **Result**: `null`

[通话保持对象]: ../../objects/hold_info.md
