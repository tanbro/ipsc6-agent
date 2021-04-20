# IPSC 6 呼叫中心系统 的 Agent Windows 桌面客户端程序

这个项目依赖于许多 Native 代码。为了兼容 32bit 系统，我们统一采用 Win32 构建。

## pjproject

这个依赖项目作为 `git submodule` 存放在 `submodules/pjproject`，如果尚未初始化这个 `git` 子模块，应执行：

```sh
git submodule update --init
```

本项目中，我们在 Windows x86_64 桌面环境下，使用 VisualStudio 2019 构建这个依赖项目。

参考：<https://trac.pjsip.org/repos/wiki/Getting-Started/Windows>

1. 使用 VisualStudio 2019 打开 `pjproject-vs14.sln`，按照提示升级到最新的 VisualStudio 项目格式，忽略不支持的项目
2. 按照提示升级所有打开的项目的 `Windows SDK`(目前是 `v10.0`) 和 `平台工具集` 到最新的版本(目前是 `v142`)
3. 如果提示安装 `UWP SDK`，不必理会
4. 在项目列表中，将 `pjsua` "设为启动项目"
5. 生成 `pjsua`。生成的库文件在 `lib` 目录，形如 `libpjproject-i386-Win32-vc14-Debug.lib`

## RakNet

IPSC 一向以来使用这个库进行服务器-坐席客户端的网络通信

此依赖项十分的老旧，无法使用现有的 VisualStudio 打开。

不过目前尚可用其提供的 `CMake` 设置，利用 `CMake` 配置出与本机开发环境相符的 VisualStudio 项目文件。

在这个子项目的目录下新建子目录 `build`，然后在该目录中执行:

```sh
cmake -G"Visual Studio 2019" -A Win32 ..
```

从而产生 VisualStudio 项目文件。

但是，生成的 `.vcxproj` 项目文件可能有错误。

1. 转义字符串错误:

   在低版本的 `Windows` (如 `Windows 7`) 上生成的 `.vcxproj` 项目文件中可能存在转义字符错误。

   使用文本编辑器打开生成的 Visual Studio 项目文件， 如 `build/Lib/LibStatic/RakNetLibStatic.vcxproj`，找到其中行如

   ```xml
   <AdditionalOptions>%(AdditionalOptions) /machine:X86 LIBCMTD.lib "MSVCRT.lib&amp;quot"%3B""</AdditionalOptions>
   ```

   的设置项，将 这样包含错误转义字符的选项改为 `%(AdditionalOptions) /machine:X86 LIBCMTD.lib MSVCRT.lib` 。

1. “首选的生成工具体系结构”设置错误

   使用 IDE 查看这几个项目的属性，将 `配置属性 -> 高级 -> 首选的生成工具体系结构` 修改为 "32 位(x86)"
