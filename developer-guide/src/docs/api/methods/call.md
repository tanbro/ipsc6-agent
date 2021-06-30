# 呼叫信息相关方法

## 获取呼叫列表

获取与当前座席相关的呼叫对象数组。

-   **Method**: `getCalls`

-   **Params**: 无

-   **Result**: `Array`

    数组元素是 [Call][] 对象

!!! info

    此处的呼叫指的是 CTI 服务器告知客户端的，服务器一侧的呼叫信息。

## 获取通话保持列表

获取当前座席的呼叫列表中被保持的通话。

-   **Method**: `getHeldCalls`

-   **Params**: 无

-   **Result**: `Array`

    数组元素是 [Call][] 对象

这个方法相当于从 `getCalls` 返回的数组中，过滤 `isHeld` 属性为 `true` 的 [Call][] 对象。
用以下伪代码表示:

```js
const result = getCalls().filter((call) => call.isHeld);
```

## 通话保持

保持(Hold)当前座席的正在进行的通话

-   **Method**: `hold`

-   **Params**: 无

-   **Result**: `null`

操作成功后，对应的 [Call][] 对象 `isHeld` 属性变为 `true`

## 取消保持

让被保持的呼叫恢复正常通话

-   **Method**: `unHold`

-   **Params**:

    | Argument   | Type      | Default | Description                   |
    | ---------- | --------- | ------- | ----------------------------- |
    | `ctiIndex` | `Integer` | `null`  | 呼叫所属于的 CTI 服务器索引值 |
    | `channel`  | `Integer` | `null`  | 呼叫的通道号                  |

-   **Result**: `null`

!!!tip

    如果 `ctiIndex` 和 `channel` 两个参数均省略或均为 `null`，则座席客户端会尝试对其 [Call][] 列表中的任一被保持的呼叫进行“取消保持”操作。

    仅省略一个参数或仅设置一个参数的值为 `null` 会引发错误。

!!!important

    当座席正在通话，同时也具有被保持的呼叫时，如果对被保持的呼叫执行此操作，则原来的通话会被自动保持。

操作成功后，对应的 [Call][] 对象 `isHeld` 属性变为 `false`。

[call]: ../types/call.md

--8<-- "includes/glossary.md"
