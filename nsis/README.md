# NSIS Installers

这个目录用于存放 [NSIS][] 安装包脚本相关的文件。

## 依赖软件

`deps` 目录用于存放依赖软件的安装包，用于离线安装。

> [!NOTE]
> 在制作安装包之前，应手动将依赖软件的安装包下载到这个目录。

目前，这些软件有：

- .NET Framework 4.6.1

  下载地址: <https://dotnet.microsoft.com/download/dotnet-framework/net461>

- Microsoft Visual C++ Redistributable for Visual Studio 2015, 2017 and 2019

  参考: <https://support.microsoft.com/topic/the-latest-supported-visual-c-downloads-2647da03-1eea-4433-9aff-95f26a218cc0>

  下载地址:

  - x86: <https://aka.ms/vs/16/release/vc_redist.x86.exe>
  - x64: <https://aka.ms/vs/16/release/vc_redist.x64.exe>

## 安装包

这个目录中有多个安装包:

- `launch.nsi`: 座席程序的 URI Scheme Handler 启动程序系统级安装包
- `win32-system.nsi`: 座席程序 32bits Windows 系统级安装包
- `win32-system.nsi`: 座席程序 64bits Windows 系统级安装包
- `win64-user.nsi`: 座席程序 64bits Windows 用户级安装包

> [!TIP]
> 座席程序的安装包还依赖于 `launch` 安装包二进制文件，所以需要首先构建 `launch` 安装包。

[NSIS]: https://sourceforge.net/projects/nsis/ "NSIS (Nullsoft Scriptable Install System) is a professional open source system to create Windows installers"
