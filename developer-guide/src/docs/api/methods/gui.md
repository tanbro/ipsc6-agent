# 图形界面相关方法

## 登录

在座席程序的登录对话框出现时，调用该方法，向对话框自动填入登录工号与密码、自动点击确定。

-   **Method**: `login`

-   **Params**:

    | Argument     | Type     | Default | Description        |
    | ------------ | -------- | ------- | ------------------ |
    | `workerNum`  | `String` | -       | 登录工号           |
    | `password`   | `String` | -       | 登录密码           |
    | `serverList` | `Array`  | `null`  | CTI 服务器地址列表 |

    `serverList`
    : 其数组元素应是 `String` 类型。 如果传入 `null`(默认) 或空数组，座席程序将根据配置项设置的地址列表连接 CTI 服务器。

-   **Result**: `null`

!!! important

    该方法仅在座席客户端桌面程序已经初始化完毕，但还没有的登录时有效。

## 退出

-   **Method**: `exitApp`

-   **Params**:

    | Argument | Type      | Default | Description |
    | -------- | --------- | ------- | ----------- |
    | `code`   | `Integer` | `0`     | 进程结束码  |

-   **Result**: `null`

!!! info

    该方法不与 CTI 服务器通信

!!! important

    该方法仅在座席客户端桌面程序的登录成功，主窗口完成构建后有效。

!!! attention

    退出后， WebSocket 也就不再可以访问了。

## 隐藏主窗口

-   **Method**: `hideApp`

-   **Params**: 无

-   **Result**: `null`

!!! info

    该方法不与 CTI 服务器通信

!!! important

    该方法仅在座席客户端桌面程序的登录成功，主窗口完成构建后有效。

## 显示主窗口

-   **Method**: `showApp`

-   **Params**: 无

-   **Result**: `null`

!!! info

    该方法不与 CTI 服务器通信

!!! important

    该方法仅在座席客户端桌面程序的登录成功，主窗口完成构建后有效。

--8<-- "includes/glossary.md"
