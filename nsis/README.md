# NSIS Installers

这个目录用于存放 [NSIS][] 安装包脚本相关的文件。

## 依赖软件

`deps` 目录用于存放依赖软件的安装包，用于离线安装。

> **Note:**
>
> 在制作安装包之前，应手动将依赖软件的安装包下载到这个目录。

目前，这些软件有：

- .NET Framework 4.8

  下载参考: <https://dotnet.microsoft.com/zh-cn/download/dotnet-framework/net48>

- Microsoft Visual C++ Redistributable for Visual Studio 2015, 2017, 2019 和 2022

  下载参考: <https://learn.microsoft.com/zh-CN/cpp/windows/latest-supported-vc-redist?view=msvc-170#visual-studio-2015-2017-2019-and-2022>

## 安装包

这个目录中有多个安装包:

- `launch.nsi`: 座席程序的 URI Scheme Handler 启动程序系统级安装包
- `win32-system.nsi`: 座席程序 32bits Windows 系统级安装包
- `win32-system.nsi`: 座席程序 64bits Windows 系统级安装包
- `win64-user.nsi`: 座席程序 64bits Windows 用户级安装包

> **Tips:**
>
> 座席程序的安装包还依赖于 `launch` 安装包二进制文件，所以需要首先构建 `launch` 安装包。

## 版本计算

在 [NSIS][] 脚本 `version.nsh` 中，使用了 [GitVersion][] (只需命令行工具 [`GitVersion.Tool`](https://www.nuget.org/packages/GitVersion.Tool)) 和 [Python][] 进行计算，请首先安装这两个软件。

[NSIS]: https://sourceforge.net/projects/nsis/ "NSIS (Nullsoft Scriptable Install System) is a professional open source system to create Windows installers"
[GitVersion]: https://gitversion.net/ "From git log to SemVer in no time"
[Python]: https://www.python.org/ "Python is a programming language that lets you work quickly and integrate systems more effectively."
