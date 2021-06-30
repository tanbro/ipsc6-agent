# 座席组相关事件

## 座席组的签入或签出状态改变

-   **Method**: `onSignedGroupsChanged`

-   **Params**: 无

该事件仅通知座席组的签入发生改变，而没有提供具体的签入/签出数据。如果需要这些数据，可以调用 [getStatus][] 获取完整的 [Group][] 对象列表。

[group]: ../types/group.md
[getstatus]: ../methods/status.md#getStatus

--8<-- "includes/glossary.md"
