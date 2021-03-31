# pjsua2

使用 `pjproject` 在 `pjsip-apps/swig` 中提供的 `swig wrapper`，构建适用于 `.net` 的 `C#` 语言的类型库。

该项目的 `src` 目录中的源代码文件由 `swig` 生成。

这个项目从原生`C++`动态库 `pjsua2.dll` 导入符号，该动态库由项目 `pjsua2_wrap` 生成。
