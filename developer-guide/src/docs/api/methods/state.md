# 座席状态

## 获取座席状态

获取座席当前的状态以及工作类型

获取当前的座席状态([AgentState][])和工作类型([WorkType][])

-   **Method**: `getStatus`

-   **Params**: 无

-   **Result**:

    返回值是一维数组。数组元素有两个，第一个是 [AgentState][] 枚举值，第二个是 [WorkType][] 枚举值。

## 座席示忙

-   **Method**: `setBusy`

-   **Params**:

    | Argument   | Type      | Default | Description                     |
    | ---------- | --------- | ------- | ------------------------------- |
    | `workType` | `Integer` | `10`    | 忙碌的种类: [WorkType][] 枚举值 |

-   **Result**: 无

## 座席示闲

-   **Method**: `setIdle`
-   **Params**: 无
-   **Result**: 无

[agentstate]: ../types/enums.md#座席状态
[worktype]: ../types/enums.md#座席工作类型

--8<-- "includes/glossary.md"
