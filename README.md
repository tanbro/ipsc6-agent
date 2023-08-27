# IPSC 6 呼叫中心系统 的 Agent Windows 桌面客户端程序

## 简介

这个项目是 IPSC 6.0 呼叫中心的 Windows 客户端，它的主要特性有：

1. 现代化的 Metro 风格 GUI；自动隐藏的横幅式主窗口；

   ![LoginWindow](developer-guide/src/images/LoginWindow.png)
   ![MainWindow](developer-guide/src/images/MainWindow.png)

1. 嵌入了一个桌面级别的轻量级 Web 服务器，提供编程接口，可以从浏览器直接调用

作为一个实际投入使用的桌面程序，学习者可以通过这个项目看到许多有一定深入性的内容：

- 在 C# GUI程序中使用 C++ 库的复杂技术，包括 CMake, swig, c++/cli, dotnet GUI 线程和 Native 线程协同使用等
- 手动编写 VisualStudio C/C++ 以及 C# 项目模板，以及使用 MSBuild 同时针对多个目标进行构建
- 如何在桌面程序嵌入轻量级 Web 服务器，以及在此基础上实现一个 JSON-RPC over Websocket 服务程序，并最终打通 Web 服务与 GUI 程序
- 按照 MVVM 设计原则编写桌面应用
- 如何管理一个有十余个项目的 VisualStudio 解决方案，以及如何在这个解决方案上运用 git 进行版本控制

> **Note:**
>
> 版权和许可信息详见 [LICENSE](LICENSE.txt) 文件

---

> **Note:**
>
> 访问 <https://tanbro.github.io/ipsc6-agent/> 查看开发文档

---

> **Note:**
>
> 经测试， VisualStudio 2019, 2022 可以成功的构建这个项目。

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

如果使用 `MSBuild` 进行构建，命令是:

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
> 如果当前开发环境的平台工具集版本与项目指定的版本不一致，建议使用 VisualStudio 打开解决方案，并按照提示自动更新项目配置，然后再构建。

## RakNet

IPSC 一向以来使用这个库进行服务器-坐席客户端的网络通信

此依赖项十分的老旧，无法使用现有的 `VisualStudio` 直接打开，不过目前尚可用其提供的 `CMake` 设置。

在这个子项目的目录下新建子目录 `build`，然后在 `build` 下新建两个子目录 `Win32` 与 `x64`，分别作为 `x86` 和 `x86_64` 的 `CMake` 构建目录。

```sh
RakNet
└── build
    └── Win32
    └── x64
```

对于 `x86`，我们打开 `x86 Native Tools Command Prompt for VS`，在 `Win32` 目录执行:

```bat
cd submodules/RakNet
mkdir build/win32
cd build/win32
cmake -A Win32 ../..
```

对于 `x86_64`，我们打开 `x64 Native Tools Command Prompt for VS`，在 `Win32` 目录执行:

```bat
cd submodules/RakNet
mkdir build/x64
cd build/x64
cmake -A x64 ../..
```

从而产生 VisualStudio 项目文件。

但是，生成的 `.vcxproj` 项目文件有以下问题：

1. `Win32` 项目的 “首选的生成工具体系结构” 设置错误:

    > ℹ **Tip**:
    >
    > 经测试，构建工具实际上并不会选择意料之外的生成工具体系结构，所以可以忽略这个问题。

    要修复这个错误，我们可以：

    - 使用 IDE 查看这几个项目的属性，将 `配置属性 -> 高级 -> 首选的生成工具体系结构` 修改为 "32 位(x86)"

    - 或者直接修改 `build/Win32/Lib/DLL/RakNetDLL.vcxproj` 文件，将行如

        ```xml
        <PropertyGroup>
          <PreferredToolArchitecture>x64</PreferredToolArchitecture>
        </PropertyGroup>
        ```

        的改为:

        ```xml
        <PropertyGroup>
          <PreferredToolArchitecture>x86</PreferredToolArchitecture>
        </PropertyGroup>
        ```

1. 转义字符串错误:

> ℹ **Tip**:
>
> 这个错误只出现在静态库构建对象的项目文件中。而我们并不需要 RakNet 的静态库，所以可以忽略这个问题。

生成的 `RakNetLibStatic.vcxproj` 项目文件中可能存在转义字符错误。

使用文本编辑器打开生成的 `Visual Studio` 项目文件， `build/{Win32|x64}/Lib/LibStatic/RakNetLibStatic.vcxproj`，找到

```xml
<AdditionalOptions>%(AdditionalOptions) /machine:X86 LIBCMTD.lib "MSVCRT.lib&amp;quot"%3B""</AdditionalOptions>
```

设置项，将 这样包含错误转义字符的选项改为 `%(AdditionalOptions) /machine:X86 LIBCMTD.lib MSVCRT.lib` 。

然后进行构建，方法是：

- 使用 `VisualStudio` 分别打开解决方案文件:

  - `x86`: `submodules/RanNet/build/Win32/RakNet.sln`
  - `x64`: `submodules/RanNet/build/x64/RakNet.sln`

  然后构建项目 `RakNetDLL`

- 或者使用 `CMake` 命令

  对于 `x86`，我们打开 `x86 Native Tools Command Prompt for VS`，在 `submodules/RanNet/build/Win32` 目录执行构建命令；
  
  对于 `x86_64`，我们打开 `x64 Native Tools Command Prompt for VS`，在 `submodules/RanNet/build/64` 目录构建命令。

  - 构建 `Debug` 目标:

    ```bat
    cmake --build . --target RakNetDLL --config Debug
    ```

  - 构建 `Release` 目标:

    ```bat
    cmake --build . --target RakNetDLL --config Release
    ```

最后，为了确保将所需头文件复制到 `include` 目录，执行:

```sh
cmake -DRAKNET_GENERATE_INCLUDE_ONLY_DIR=ON ../..
```

## ZLIB

在 VisualStudio 中打开这个解决方案，然后打开方案的终端（“视图” -> “终端” | 快捷键 “Ctrl+`”）

```powershell
vcpkg x-update-baseline  --add-initial-baseline
```

```powershell
vcpkg install --x-feature=VcpkgAdditionalInstallOptions
```

> **Tips:**
>
> `vcpkg` 支持 `ALL_PROXY`, `HTTP_PROXY` 等环境变量
