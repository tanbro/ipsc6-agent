# 安装

## 先决条件

### 硬件要求

- CPU Intel i5-3xxx 系列以及其它性能相当或更高的处理器
- RAM: 8GB 或以上
- HDD: 3GB 以上可用空间
- Graphics card: Intel HD Graphics 4400 (integrated) 以及其它性能相当或更高的图形适配器
- Graphics display resolution: 至少 1280 x 1024
- Sound card: 双工立体声
- 键盘鼠标
- 头戴式耳机和麦克风

### 平台/操作系统

座席客户端可以在以下平台/操作系统上运行：

- Windows 7 Service Pack 1 (x86/x86_64)
- Windows 8 (x86/x86_64)
- Windows 8.1 (x86/x86_64)
- Windows 10 (x86/x86_64)

### 依赖软件

- Microsoft Visual C++ Redistributable for Visual Studio 2015, 2017 and 2019 (x86).

    下载地址: <https://support.microsoft.com/en-us/topic/the-latest-supported-visual-c-downloads-2647da03-1eea-4433-9aff-95f26a218cc0>

- .NET Framework 4.6.1 ~ 4.8

    下载地址: <https://dotnet.microsoft.com/download/dotnet-framework>

## 直接使用文件

座席程序并不需要真正意义上的安装。
在依赖软件已经被安装的前提下，座席客户端程序可直接运行。

## 使用安装程序

座席程序的安装程序需要管理员权限。它将:

- 把程序文件复制到 `%programfiles(x86)%\HesongInfoTech\ipsc6.agent.wpfapp` 目录
- 建立快捷方式
- 新建注册表项 `HKEY_CLASSES_ROOT\ipsc6-agent-app`