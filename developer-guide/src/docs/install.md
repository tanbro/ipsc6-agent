# 安装

## 先决条件

### 硬件要求

-   CPU Intel i5-3xxx 系列以及其它性能相当或更高的处理器
-   RAM: 8GB 或以上
-   HDD: 3GB 以上可用空间
-   Graphics card: Intel HD Graphics 4400 (integrated) 以及其它性能相当或更高的图形适配器
-   Graphics display resolution: 至少 1280 x 1024
-   Sound card: 双工立体声
-   键盘鼠标
-   头戴式耳机和麦克风

### 平台/操作系统

座席客户端可以在以下平台/操作系统上运行：

-   Windows 7 Service Pack 1 (x86/x86_64)
-   Windows 8 (x86/x86_64)
-   Windows 8.1 (x86/x86_64)
-   Windows 10 (x86/x86_64)

### 依赖软件

座席客户端程序依赖以下软件，只有在这些软件全部正确安装之后程序方可运行。

1.  适用于 Visual Studio 2015、2017 和 2019 的 Microsoft Visual C++ 可再发行软件包

    下载地址: <https://support.microsoft.com/topic/the-latest-supported-visual-c-downloads-2647da03-1eea-4433-9aff-95f26a218cc0>

    !!! tip

        座席程序支持 x86 和 x64 架构，并分别提供不同的程序文件。

        x86 程序可以在32位或64位 Windows 下以运行；x64 程序只能在64位 Windows 下运行。

        安装 Microsoft Visual C++ 可再发行软件包时，注意选择对应的架构。

1.  .NET Framework 4.6.1 及以上

    `.NET Framework` 版本 `4.6.1` 至 `4.8` 均可兼容座席程序。低于 `4.6.1` 的，如 `4.6`, `4.5` 不满足要求。

    下载地址: <https://dotnet.microsoft.com/download/dotnet-framework>

    !!! tip

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

座席程序的安装程序需要管理员权限。它将:

-   把程序文件复制到 `%programfiles(x86)%\HesongInfoTech\ipsc6.agent.wpfapp` 目录
-   建立快捷方式
-   新建注册表项 `HKEY_CLASSES_ROOT\ipsc6-agent-app`

--8<-- "includes/glossary.md"
