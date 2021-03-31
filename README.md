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
