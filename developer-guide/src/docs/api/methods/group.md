# 座席组

## 获取座席组列表

获取座席所属的技能

-   **Method**: `getGroups`

-   **Params**: 无

-   **Result**: `Array`

    数组元素是 [Group][] 对象

## 签入或签出座席组

-   **Method**: `signGroups`

-   **Params**:

    | Argument   | Type              | Default | Description                                                                      |
    | ---------- | ----------------- | ------- | -------------------------------------------------------------------------------- |
    | `id`       | `String`, `Array` | `""`    | 要签入或签出的组 ID 或 ID 数组。空数组或空字符串表示签入或签出所有的组 |
    | `isSignIn` | `Boolean`         | `true`  | `true`: 签入; `false`: 签出                                                      |

-   **Result**: 无

[group]: ../types/group.md

--8<-- "includes/glossary.md"
