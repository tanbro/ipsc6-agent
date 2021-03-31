# pjsua2 wrapper

使用 `pjproject` 在 `pjsip-apps/swig` 中提供的 `swig wrapper`，构建适用于 `.net` 的 `C#` 语言的类型库。

该项目的 `src` 目录中的源代码文件由 `swig` 生成。

这个项目输出到动态库名为 `pjsua2.dll`。这个动态库由`C#`项目 `org.pjsip.pjusa2`包装后加以使用。

# pjsua2-python

在 windows 中，使用 Visual Studio 2009 构建 `pjproject` 的 Python 类型库

## 先决条件

为了兼容 windows 7 以及 x86 32bit 环境，我们应安装 `Python3.8 win32` 的最新稳定版，本文成文时是 `3.8.8`。

注意，安装时要选择 "Customize installation"，而后选择下载 "debugging symbols" 和 "debugging binaries"。具体参考 <https://docs.microsoft.com/en-us/visualstudio/python/debugging-symbols-for-mixed-mode-c-cpp-python>

如果 installer 无法下载，可考虑手工在以下地址下载相关安装包文件：

- <https://www.python.org/ftp/python/3.8.8/win32/core_d.msi>
- <https://www.python.org/ftp/python/3.8.8/win32/core_pdb.msi>
- <https://www.python.org/ftp/python/3.8.8/win32/dev_d.msi>
- <https://www.python.org/ftp/python/3.8.8/win32/exe_d.msi>
- <https://www.python.org/ftp/python/3.8.8/win32/exe_pdb.msi>
- <https://www.python.org/ftp/python/3.8.8/win32/lib_d.msi>
- <https://www.python.org/ftp/python/3.8.8/win32/lib_pdb.msi>
- <https://www.python.org/ftp/python/3.8.8/win32/test_b.msi>
- <https://www.python.org/ftp/python/3.8.8/win32/test_pdb.msi>
- <https://www.python.org/ftp/python/3.8.8/win32/tcltk_d.msi>
- <https://www.python.org/ftp/python/3.8.8/win32/tcltk_pdb.msi>

下载的文件放在 installer 所在目录再次运行。

## 使用 Swig 生成源代码

在 windows7 x86 + visualstudio 2019 + python3.8 环境，编译 pjproject 的 python library。

在目录 `pjproject-2.0/src/swig/python` 执行：

```sh
swig -c++ -I ../../../../pjlib/include  -I ../../../../pjlib-util/include -I ../../../../pjmedia/include -I ../../../../pjsip/include -I ../../../../pjnath/include -python -py3 -builtin ../pjsua2.i
```

将生成的文件:

- `pjsua2_wrap.h`
- `pjsua2_wrap.cxx`
- `pjsua2.py`

连同这个目录下的

- `setup.py`

复制出来，行程本项目的起始文件。

## 新建VisualStudio项目与解决方案

打开 VisualStudio2019

文件->创建新项目->动态连接库(DLL)

将解决方案命名为`pjsua2-python`，保存在本项目的 `vs2019` 子目录中。

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

`PJPROJECT_ROOT` 为 PJSIP 的项目源代码路径

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

- `libpjproject-$(TargetCPU)-$(PlatformName)-vc$(VSVer)-$(ConfigurationName).lib`
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

再次在IDE中打开该属性文件，找到 通用属性 -> 常规 -> 目标文件名

修改为： `_pjsua2`

找到 通用属性 -> 常规 -> 目标文件扩展名

修改为：`.pyd`

### 项目属性

配置属性 -> C/C++ -> 代码生成 -> 运行库

凡 Debug 配置，改为 `/MTd`
