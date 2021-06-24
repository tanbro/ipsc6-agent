# CTI 服务器相关事件

## CTI 服务器连接状态变化事件

-   **Method**: `onCtiConnectionStateChanged`

-   **Params**:

    | Argument   | Type      | Default | Description                                     |
    | ---------- | --------- | ------- | ----------------------------------------------- |
    | `ctiIndex` | `Integer` | -       | 连接状态发生变化的 CTI 服务器的索引值           |
    | `oldState` | `Integer` | -       | 原连接状态：[CtiServerConnectionState][] 枚举值 |
    | `newState` | `Integer` | -       | 现连接状态：[CtiServerConnectionState][] 枚举值 |

[ctiserverconnectionstate]: ../types/enums.md#CTI-服务器连接状态 "CTI 服务器连接状态"

--8<-- "includes/glossary.md"
