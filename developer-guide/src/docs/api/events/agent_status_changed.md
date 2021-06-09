# 座席状态改变

登录、注销，签入、签出技能组，示闲、示忙，来电接听等操作均可导致座席状态的改变。

这个事件将会把新的状态作为参数送出。

- **Method**: `onAgentStatusChanged`

- **Params**:

    |   Argument   |   Type    | Default |     Description      |
    | ------------ | --------- | ------- | -------------------- |
    | `agentState` | `Integer` | -       | [座席状态][] 枚举值 |
    | `workType`   | `Integer` | -       | [工作类型][] 枚举值  |

[座席状态]: ../enums/agent_state.md
[工作类型]: ../enums/agent_work_type.md
