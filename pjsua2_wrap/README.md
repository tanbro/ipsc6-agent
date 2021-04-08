# PJSUA2 wrapper

使用 `pjproject` 在 `pjsip-apps/swig` 中提供的 `swig wrapper`，构建适用于 `.net` 的 `C#` 语言的类型库。

该项目的 `src` 目录中的源代码文件由 `swig` 生成。

这个项目输出到动态库名为 `pjsua2.dll`。这个动态库由`C#`项目 `org.pjsip.pjusa2`包装后加以使用。

## 使用 Swig 生成源代码

临时新建一个目录，使用它作为工作目录，设置环境变量`PJPROJECT_DIR` 为  `pjproject` 的工程代码所在目录，然后执行：

```sh
PJPROJECT_DIR=dir/of/pjproject && swig -c++ -I${PJPROJECT_DIR}/pjlib/include -I${PJPROJECT_DIR}/pjlib-util/include -I${PJPROJECT_DIR}/pjmedia/include -I${PJPROJECT_DIR}/pjsip/include -I${PJPROJECT_DIR}/pjnath/include -csharp -outcurrentdir -outdir . -namespace org.pjsip.pjsua2 ${PJPROJECT_DIR}/pjsip-apps/src/swig/pjsua2.i
```

- 将生成的 `C++` 源文件:

  - `pjsua2_wrap.h`
  - `pjsua2_wrap.cxx`

  用于 C++ Wrapper 项目

- 将生成的 `C#` 源文件用于包装上述 `Wrapper` 项目的 `dotnet` 类型库项目。

## 新建 Visual Studio 项目与解决方案

打开 Visual Studio 2019

文件->创建新项目->动态连接库(`DLL`)

将解决方案命名为 `pjsua2_wrap` ，保存在本项目的 `vs2019` 子目录中。

在项目管理器中：

- 将 `pjsua2_wrap.h` 添加到头文件
- 将 `pjsua2_wrap.cxx` 添加到源文件

## 项目配置

### 预编译头

项目属性中：

配置属性 -> C/C++ -> 预编译头 -> 不使用预编译头

### 属性文件

主菜单 -> 视图 -> 其它视图 -> 属性管理器

然后再属性管理器中，添加新的项目属性文件，保存到项目根目录。

打开这个属性文件，中 通用属性 -> 用户宏 中添加：

`PJPROJECT_ROOT` 为 `PJSIP` 的项目源代码路径

注意选择“将此宏设置为生成环境中的环境变量”

继续编辑该属性文件

通用属性 -> C/C++ -> 常规 的附加包含目录添加:

- `$(PJPROJECT_ROOT)\pjlib\include`
- `$(PJPROJECT_ROOT)\pjlib-util\include`
- `$(PJPROJECT_ROOT)\pjmedia\include`
- `$(PJPROJECT_ROOT)\pjnath\include`
- `$(PJPROJECT_ROOT)\pjsip\include`

通用属性 -> 链接器 -> 常规 的附加库目录 添加:

`$(PJPROJECT_ROOT)\lib`

通用属性 -> 链接器 -> 输入 的附加依赖项 添加:

- `libpjproject-$(TargetCPU)-$(PlatformName)-vc14-$(ConfigurationName).lib`
- `ws2_32.lib`

然后用文本编辑器打开这个属性文件 `PropertySheet.props`

在编译和链接配置项之前加上：

```xml
  <Choose>
    <When Condition="'$(Platform)'=='Win32' ">
      <PropertyGroup Label="UserMacros">
        <TargetCPU>i386</TargetCPU>
      </PropertyGroup>
    </When>
    <When Condition="'$(Platform)'=='x64'">
      <PropertyGroup Label="UserMacros">
        <TargetCPU>x86_64</TargetCPU>
      </PropertyGroup>
    </When>
  </Choose>
```

再次在 `IDE` 中打开该属性文件，找到 通用属性 -> 常规 -> 目标文件名

修改为： `pjsua2`

### 项目属性

配置属性 -> C/C++ -> 代码生成 -> 运行库

Debug 配置: 改为 `/MTd`

Release 配置：改为 `/MT`

