# IPSC 6 呼叫中心系统 的 Agent Windows 桌面客户端程序

## 简介

这个项目是 IPSC 6.0 呼叫中心的 Windows 客户端，它的主要特性有：

1. 现代化的 Metro 风格 GUI；自动隐藏的横幅式主窗口；

   ![LoginWindow](developer-guide/src/images/LoginWindow.png)
   ![MainWindow](developer-guide/src/images/MainWindow.png)

1. 嵌入了一个桌面级别的轻量级 Web 服务器，提供编程接口，可以从浏览器直接调用

这个项目的主要技术点有：

- 在 C# GUI 程序中使用 C++ 库的复杂技术，包括 [CMake][], [swig][], [C++/CLI][], dotnet GUI 线程和 Native 线程协同使用等
- 手动编写 VisualStudio C/C++ 以及 C# 项目模板，以及使用 MSBuild 进行多目标构建
- 桌面程序嵌入轻量级 Web 服务器，以及在此基础上实现一个 JSON-RPC over Websocket 服务程序，并最终打通 Web 服务与 GUI 程序
- 运用 MVVM 设计模式的 WPF 桌面应用
- 有十余个项目和多个外部依赖的 VisualStudio 解决方案，以及在这个解决方案上运用 git 进行版本控制

> **Note:**
>
> 版权和许可信息详见 [LICENSE](LICENSE.txt) 文件

---

> **Note:**
>
> 访问 <https://tanbro.github.io/ipsc6-agent/> 查看开发文档；
> 或参考 [developer-guide/README.md](developer-guide/README.md) 自行构建开发手册。

## 开发环境

经测试，在 Windows 10, 11 (x86_64) 桌面环境下，使用 **VisualStudio 2022** Community edition 可以成功的构建这个项目。

安装时，要选择“工作负荷”有：

- 使用 C++ 的桌面开发
- .NET 的桌面开发

其它需要的开发工具有：

- Git 2.0 或以上
- [CMake][] - 访问 <https://cmake.org/> 下载安装最新的稳定版本。注意确保 `cmake` 命令所在目录处于系统路径中。
- [swig][] - 访问 <https://swig.org/> 下载安装最新的稳定版本。注意确保 `swig` 命令所在目录处于系统路径中。

## 如何构建

依赖项目 RakNet 与 [PJSIP][] 以 `git submodule` 的形式存放在 `submodules` 子目录中，如果尚未初始化这个 `git` 子模块，应执行以下命令更新 submodule：

```sh
git submodule update --init
```

1. 安装 C/C++ 依赖包

   这个项目采用 [vcpkg][] 作为 C/C++ 包管理器。

   > **Note:**
   > 目前仅使用了 zlib ，但有问题。至于是否沿用、如何替换，还再犹豫。

   在 VisualStudio 中打开这个解决方案，然后打开方案的终端（主菜单 “视图” -> “终端” 或 快捷键 “Ctrl+`”），执行命令：

   ```powershell
   vcpkg install --triplet x64-windows
   ```

   如果提示 baseline 错误，请执行:

   ```powershell
   vcpkg x-update-baseline --add-initial-baseline
   ```

   > **Tips:**
   >
   > - `vcpkg` 支持 `ALL_PROXY`, `HTTP_PROXY` 等环境变量

1. 构建 RakNet

   IPSC 长期以来使用这个库进行服务器-坐席客户端的网络通信

   此依赖项十分的老旧，无法使用现有的 `VisualStudio` 直接打开，不过目前尚可用其提供的 `CMake` 设置。其步骤是:

   在这个子项目的目录下新建子目录 `build`，然后在 `build` 下新建两个子目录 `Win32` 与 `x64`，分别作为 `x86` 和 `x86_64` 的 `CMake` 构建目录。

   ```sh
   RakNet
   └── build
       └── Win32
       └── x64
   ```

   1. Configure

      对于 `x86`，我们打开 `x86 Native Tools Command Prompt for VS`执行:

      ```bat
      cd submodules\RakNet
      mkdir build\win32
      cd build\win32
      cmake -A Win32 ..\..
      ```

      对于 `x86_64`，我们打开 `x64 Native Tools Command Prompt for VS` 执行:

      ```bat
      cd submodules\RakNet
      mkdir build\x64
      cd build\x64
      cmake -A x64 ..\.
      ```

      从而产生 VisualStudio 项目文件。

      > **Note:**
      >
      > 生成的 `.vcxproj` 项目文件有以下问题：
      >
      > 1. `Win32` 项目的 “首选的生成工具体系结构” 设置错误:
      >
      >    **经测试，构建工具实际上并不会选择意料之外的生成工具体系结构，所以可以忽略这个问题。**
      >
      >    要修复这个错误，我们可以：
      >
      >    - 使用 IDE 查看这几个项目的属性，将 `配置属性 -> 高级 -> 首选的生成工具体系结构` 修改为 "32 位(x86)"
      >
      >    - 或者直接修改 `build/Win32/Lib/DLL/RakNetDLL.vcxproj` 文件，将行如
      >
      >      ```xml
      >      <PropertyGroup>
      >        <PreferredToolArchitecture>x64</PreferredToolArchitecture>
      >      </PropertyGroup>
      >      ```
      >
      >      的改为:
      >
      >      ```xml
      >      <PropertyGroup>
      >        <PreferredToolArchitecture>x86</PreferredToolArchitecture>
      >      </PropertyGroup>
      >      ```
      >
      > 1. 转义字符串错误:
      >
      >    **这个错误只出现在静态库构建对象的项目文件中。而我们并不计划使用 RakNet 的静态库，所以可以忽略这个问题。**
      >
      >    生成的 `RakNetLibStatic.vcxproj` 项目文件中可能存在转义字符错误。修复方法是用文本编辑器打开生成的项目文件， `build/{Win32|x64}/Lib/LibStatic/RakNetLibStatic.vcxproj`，找到:
      >
      >    ```xml
      >    <AdditionalOptions>%(AdditionalOptions) /machine:X86 LIBCMTD.lib "MSVCRT.lib&amp;quot"%3B""</AdditionalOptions>
      >    ```
      >
      >    设置项的 XML 片段，将这样包含错误转义字符的选项改为:
      >
      >    ```xml
      >    <AdditionalOptions>%(AdditionalOptions) /machine:X86 LIBCMTD.lib MSVCRT.lib</AdditionalOptions>
      >    ```

   1. Build

      - 使用 `VisualStudio` 分别打开解决方案文件:

        - `x86`: `submodules\RanNet\build\Win32\RakNet.sln`
        - `x64`: `submodules\RanNet\build\x64\RakNet.sln`

        然后构建项目 `RakNetDLL`

      - 或者使用 `CMake` 命令直接构建

        对于 `x86`，打开 `x86 Native Tools Command Prompt for VS`，在 `submodules/RanNet/build/Win32` 目录执行构建命令；
        对于 `x86_64`，打开 `x64 Native Tools Command Prompt for VS`，在 `submodules/RanNet/build/64` 目录构建命令。
        命令是：

        - 构建 `Debug` 目标:

          ```bat
          cmake --build . --target RakNetDLL --config Debug
          ```

        - 构建 `Release` 目标:

          ```bat
          cmake --build . --target RakNetDLL --config Release
          ```

   1. 生成头文件

      最后，为了确保将提供给引用方使用的头文件复制到 `include` 目录，执行:

      ```bat
      cmake -DRAKNET_GENERATE_INCLUDE_ONLY_DIR=ON ..\..
      ```

1. 构建 [PJSIP][]

   参考：<https://docs.pjsip.org/en/latest/get-started/windows/build_instructions.html>

   1. 在目录 `pjlib/include/pj/config_site_sample.h` 新建头文件 `config_site.h`，这个文件可以为空（默认值），或者按照 <https://docs.pjsip.org/en/latest/get-started/guidelines-development.html#config-site-h> 的说明：

      ```c
      #include <pj/config_site_sample.h>
      ```

   1. 使用 VisualStudio 打开 `pjproject-vs14.sln`，按照提示升级到最新的 VisualStudio 项目格式，以及重定项目目标到与当前开发环境匹配的 Windows SDK 与平台工具集版本，忽略不支持的项目
   1. 按照提示升级所有打开的项目的 `Windows SDK` 和 `平台工具集` 到当前开发环境匹配的版本
   1. 如果提示安装 `UWP SDK`，不必理会
   1. 在项目列表中，将 `pjsua` "设为启动项目"
   1. 生成 `pjsua`。生成的库文件在 `lib` 目录，形如 `libpjproject-i386-Win32-vc14-Debug.lib`

   如果使用 `MSBuild` 进行构建，应在 "Developer PowerShell for VS" 或 "Developer Command Prompt for VS" 命令行环境下执行:

   > **Note:**
   >
   > 由于开发环境的平台工具集版本与项目指定的版本大概率不一致，应该使用 VisualStudio 打开解决方案，并按照提示升级到最新的 VisualStudio 项目格式，以及重定项目目标到与当前开发环境匹配的 Windows SDK 与平台工具集版本，忽略不支持的项目，然后再构建。
   > 当然也可手动修改项目文件的 XML 内容。

   - Win32 Debug 静态库:

     ```powershell
     msbuild pjproject-vs14.sln -target:pjsua -m -property:Configuration=Debug -property:Platform=Win32
     ```

   - Win32 Release 静态库:

     ```powershell
     msbuild pjproject-vs14.sln -target:pjsua -m -property:Configuration=Release -property:Platform=Win32
     ```

   - x64 Debug 静态库:

     ```powershell
     msbuild pjproject-vs14.sln -target:pjsua -m -property:Configuration=Debug -property:Platform=x64
     ```

   - x64 Release 静态库:

     ```powershell
     msbuild pjproject-vs14.sln -target:pjsua -m -property:Configuration=Release -property:Platform=x64
     ```

   > **Note:**
   >
   > 目前使用的 [PJSIP][] 版本是 `2.13.1`，如果更新库的版本，且有 C++ 对外接口或数据、类型定义的变化，就需要使用 [swig][] 重新生成 `pjsua2_wrap` 与 `org.pjsip.pjsua2` 的源文件。具体参考这两个目录的 README。

1. 生成客户端 GUI 程序

   在 VisualStudio 中打开这个解决方案，然后：

   1. 在解决方案管理器中将项目 `ipsc6.agent.wpfapp`。
   1. 根据需要正确设置方案的构建目标（“x64” 或者 “x86”，但不能是 “Any CPU”），即可生成或者进入调试。

[PJSIP]: https://www.pjsip.org/ "PJSIP is a free and open source multimedia communication library written in C language implementing standard based protocols such as SIP, SDP, RTP, STUN, TURN, and ICE."
[swig]: https://swig.org/ "SWIG is a software development tool that connects programs written in C and C++ with a variety of high-level programming languages."
[CMake]: https://cmake.org/ "CMake is an open-source, cross-platform family of tools designed to build, test and package software."
[C++/CLI]: https://learn.microsoft.com/cpp/dotnet/dotnet-programming-with-cpp-cli-visual-cpp "By using C++/CLI you can create C++ programs that use .NET classes as well as native C++ types."
[vcpkg]: https://github.com/microsoft/vcpkg "C++ Library Manager for Windows, Linux, and MacOS"
