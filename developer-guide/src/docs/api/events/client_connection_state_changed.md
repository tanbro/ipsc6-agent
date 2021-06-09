# 客户端-服务器连接状态变化

这个事件将会把新的状态作为参数送出。

- **Method**: `onConnectStateChanged`

- **Params**:

    | Argument |   Type    | Default |             Description             |
    | -------- | --------- | ------- | ----------------------------------- |
    | `index`  | `Integer` | -       | 连接状态发生变化的CTI服务器的索引值 |
    | `state`  | `Integer` | -       | [客户端-服务器连接状态][] 枚举值    |

[客户端-服务器连接状态]: ../enums/client_connect_state.md
