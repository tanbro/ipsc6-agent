# 安装

## 先决条件

### 硬件要求

-   CPU Intel i5-3xxx 系列以及其它性能相当或更高的处理器
-   RAM: 4GB 或以上
-   HDD: 3GB 以上可用空间
-   Graphics card: Intel HD Graphics 4400 (integrated) 以及其它性能相当或更高的图形适配器
-   Graphics display resolution: 至少 1280 x 1024
-   Sound card: 双工立体声
-   键盘鼠标
-   头戴式耳机和麦克风

### 平台/操作系统

座席客户端可以在以下平台/操作系统上运行：

-   Windows 7 Service Pack 1 (x86/x64)
-   Windows 8 (x86/x64)
-   Windows 8.1 (x86/x64)
-   Windows 10 (x86/x64)

### 依赖软件

座席客户端程序依赖以下软件，只有在这些软件全部正确安装之后程序方可运行：

1.  适用于 Visual Studio 2015、2017、2019 和 2022 的 Microsoft Visual C++ 可再发行软件包

    见: <https://learn.microsoft.com/zh-cn/cpp/windows/latest-supported-vc-redist>

    !!! caution

        座席程序支持 x86 和 x64 架构，并分别提供不同的程序文件。

        x86 程序可以在32位或64位 Windows 下以运行；x64 程序只能在64位 Windows 下运行。

        安装 Microsoft Visual C++ 可再发行软件包时，注意选择对应的架构。

1.  .NET Framework 4.8 及以上

    `.NET Framework` 版本 `4.8` 及以上，如 `4.8`, `4.8.1` 均可兼容座席程序；低于 `4.8` 的，如 `4.7.2`, `4.5`，`3.5` 则不满足要求。

    下载地址: <https://dotnet.microsoft.com/download/dotnet-framework>

    !!! caution

        座席程序所需的是 **`.NET Framework`**，而**不是** `.NET` 或 `.NET Core`。

        `.NET Core` `2.1`、`.NET Core` `3.1`、`.NET` `5.0`、`.NET` `6.0` 等均**不支持**。

## 操作系统权限

座席程序需要以下操作系统权限:

-   音频采集
-   音频播放
-   新建和写入日志文件
-   使用网络发送和接收数据
-   打开网络端口进行监听

## 直接使用文件

在依赖软件已经被安装的前提下，将座席程序复制到具有执行权限的目录，即可直接运行其可执行文件。

## 使用安装程序

我们提供了三种座席安装程序：

1.  系统级的 Win32 安装包

    它需要管理员权限，程序文件会复制到 `%ProgramFiles(x86)%` 目录。

    这个安装程序可用于 `x86` 平台上的 `32bits` Windows 操作系统，以及 `x64` 平台上的 `32bits` 或 `64bits` Windows 操作系统。

1.  系统级的 Win64 安装包

    它需要管理员权限，程序文件会复制到 `%ProgramFiles%` 目录。

    这个安装程序只能用于 `x64` 平台上的 `64bits` Windows 操作系统。

1.  用户级的 Win64 安装包

    它不需要管理员权限，程序文件会复制到当前用户名下的 `%LocalAppData%\Programs` 目录。

    这个安装程序只能用于 `x64` 平台上的 `64bits` Windows 操作系统。

!!! tip

    安装程序会自动检测所需的依赖软件。如果没有检测到，将启动安装。依赖软件的安装包已经内嵌在安装程序中，安装过程中不会访问网络。

    如果依赖软件的安装程序被执行，操作系统可能出现要求用户确认或输入密码的确认窗口，应进行确认/授权。

!!! tip

    安装时，如果选择了 “URI Protocol Handler 启动程序”（见 [从浏览器启动](startup.md#从浏览器启动)），该组件的安装程序会单独启动。
    由于这个程序需要系统全局安装，它同样会要求用户确认或输入密码，我们应进行确认/授权。

--8<-- "includes/glossary.md"
