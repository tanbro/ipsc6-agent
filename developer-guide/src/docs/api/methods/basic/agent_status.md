# 座席状态

## 获取座席的当前状态

获取当前的 [座席状态][] 和 [工作类型][]

- **Method**: `getStatus`

- **Params**: 无

- **Result**:

    返回值是一维数组。数组元素有两个，第一个是[座席状态][]枚举值，第二个是座席的[工作类型][]枚举值。

    例如：

    1. 尚未登录时，调用该方法，返回数据的 `result` 属性是:

        ```json
        {"result": [-1, 0]}
        ```

    1. 在座席空闲(示闲)，且没有被排队选中，也不做任何呼叫动作时，调用该方法，返回数据的 `result` 属性是:

        ```json
        {"result": [5, 0]}
        ```

## 示闲

- **Method**: `setIdle`
- **Params**: 无
- **Result**: `null`

## 示忙

- **Method**: `setBusy`

- **Params**:

    |  Argument  |   Type    | Default |            Description             |
    | ---------- | --------- | ------- | ---------------------------------- |
    | `workType` | `Integer` | `10`    | 忙碌的种类，是 [工作类型][] 枚举值 |

- **Result**: `null`

[座席状态]: ../../enums/agent_state.md
[工作类型]: ../../enums/agent_work_type.md
