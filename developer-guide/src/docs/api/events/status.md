# 座席状态相关事件

## 座席状态或工作类型改变

-   **Method**: `onStatusChanged`

-   **Params**:

    | Argument      | Type      | Description                         |
    | ------------- | --------- | ----------------------------------- |
    | `oldState`    | `Integer` | 原座席状态: [AgentState][] 枚举值   |
    | `newState`    | `Integer` | 现座席状态: [AgentState][] 枚举值   |
    | `oldWorkType` | `Integer` | 原座席工作类型: [WorkType][] 枚举值 |
    | `newWorkType` | `Integer` | 现座席工作类型: [WorkType][] 枚举值 |

## 座席摘挂机状态改变

-   **Method**: `onTeleStateChanged`

-   **Params**:

    | Argument      | Type      | Description                         |
    | ------------- | --------- | ----------------------------------- |
    | `oldState`    | `Integer` | 原座席状态: [AgentState][] 枚举值   |
    | `newState`    | `Integer` | 现座席状态: [AgentState][] 枚举值   |

[agentstate]: ../types/enums.md#座席状态
[worktype]: ../types/enums.md#座席工作类型

--8<-- "includes/glossary.md"
