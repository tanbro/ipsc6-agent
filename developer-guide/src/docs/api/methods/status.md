# 座席状态相关方法

## 获取座席状态

获取座席当前的状态以及工作类型

获取当前的座席状态([AgentState][])和工作类型([WorkType][])

-   **Method**: `getStatus`

-   **Params**: 无

-   **Result**: `Array`

    返回一维数组。数组元素有两个，第一个是 [AgentState][] 枚举值，第二个是 [WorkType][] 枚举值。

## 座席示忙

-   **Method**: `setBusy`

-   **Params**:

    | Argument   | Type      | Default | Description                     |
    | ---------- | --------- | ------- | ------------------------------- |
    | `workType` | `Integer` | `10`    | 忙碌的种类: [WorkType][] 枚举值 |

-   **Result**: `null`

## 座席示闲

-   **Method**: `setIdle`
-   **Params**: 无
-   **Result**: `null`

## 获取完整的座席信息

-   **Method**: `getAgentFull`

-   **Params**: 无

-   **Result**: `Object`

    返回完整的 [Agent][] 对象

[agent]: ../types/agent.md
[agentstate]: ../types/enums.md#座席状态
[worktype]: ../types/enums.md#座席工作类型

--8<-- "includes/glossary.md"
