# README

该目录中的脚本用于生成 `pjproject` 的 `C#` wrapper，生成的源文件包含 C/C++ 源文件/头文件，以及 CSharp 源文件。

> ❗ **重要**:
>
> 1. 执行这些脚本需要预先安装 [swig][]，并将命令所在目录加入到系统的搜索路径中。
> 1. 应在整个代码库的根目录执行这些脚本，而不是脚本文件所在目录。

> ℹ️ **Tip**:
>
> 生成的 `c#` 代码中，有些枚举值定义有误，类似
>
> ```csharp
> public enum pjmedia_format_id {
>     // ....
>     PJMEDIA_FORMAT_INVALID = 0xFFFFFFFF
> }
> ```
>
> 需要手动修正为:
>
> ```csharp
> public enum pjmedia_format_id {
>     // ....
>     PJMEDIA_FORMAT_INVALID = -1
> }
> ```

[swig]: http://swig.org/ "SWIG is an interface compiler that connects programs written in C and C++ with scripting languages such as Perl, Python, Ruby, and Tcl."
