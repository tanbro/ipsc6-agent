# 图形界面相关方法

## 登录

在座席程序的登录对话框出现时，调用该方法，向对话框自动填入登录工号与密码、自动点击确定。

-   **Method**: `login`

-   **Params**:

    | Argument    | Type     | Default | Description |
    | ----------- | -------- | ------- | ----------- |
    | `workerNum` | `String` | -       | 登录工号    |
    | `password`  | `String` | -       | 登录密码    |

-   **Result**: `null`

!!! important

    该方法仅在座席客户端桌面程序的登录对话框正在显示、等待用户输入时有效。

## 退出

-   **Method**: `exitApp`

-   **Params**: 无

-   **Result**: `null`

!!! tip

    座席程序在执行该方法时，会首先尝试注销。如果无法注销，将返回错误。

    当座席正在通话时， CTI 服务器不允许注销。

!!! attention

    退出后， WebSocket 也就不再可以访问了。

## 隐藏主窗口

-   **Method**: `hideMainWindow`

-   **Params**: 无

-   **Result**: `null`

!!! info

    该方法不与 CTI 服务器通信

!!! important

    该方法仅在座席客户端桌面程序的登录成功，主窗口完成构建后有效。

## 显示主窗口

-   **Method**: `showMainWindow`

-   **Params**: 无

-   **Result**: `null`

!!! info

    该方法不与 CTI 服务器通信

!!! important

    该方法仅在座席客户端桌面程序的登录成功，主窗口完成构建后有效。

--8<-- "includes/glossary.md"
