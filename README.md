# IPSC 6 呼叫中心系统 的 Agent Windows 桌面客户端程序

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

-   Win32 Debug 静态库:

    ```powershell
    msbuild pjproject-vs14.sln -target:pjsua -m -property:Configuration=Debug -property:Platform=Win32
    ```

-   Win32 Release 静态库:

    ```powershell
    msbuild pjproject-vs14.sln -target:pjsua -m -property:Configuration=Release -property:Platform=Win32
    ```

-   x64 Debug 静态库:

    ```powershell
    msbuild pjproject-vs14.sln -target:pjsua -m -property:Configuration=Debug -property:Platform=x64
    ```

-   x64 Release 静态库:

    ```powershell
    msbuild pjproject-vs14.sln -target:pjsua -m -property:Configuration=Release -property:Platform=x64
    ```

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

```sh
cmake -A Win32 ../..
```

从而产生 VisualStudio 项目文件。

但是，生成的 `.vcxproj` 项目文件有一下问题：

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

-   使用 `VisualStudio` 打开解决方案，构建项目 `RakNetDLL`

-   或者使用 `CMake` 命令:

    -   构建 `Debug` 目标:

        ```sh
        cmake --build . --target RakNetDLL --config Debug
        ```

    -   构建 `Release` 目标:

        ```sh
        cmake --build . --target RakNetDLL --config Release
        ```

`x86_64` 的构建与之类似，在 `x64` 目录，使用 `x64 Native Tools Command Prompt for VS` 重复上述过程即可（注意替换目标架构参数为`x64`）。

最后，为了确保将所需头文件复制到 `include` 目录，执行:

```sh
cmake -DRAKNET_GENERATE_INCLUDE_ONLY_DIR=ON ../..
```
