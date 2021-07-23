# 启动

## 进程启动参数

座席程序的命令行参数的使用方法详见 [配置项列表](config.md#命令行参数配置)；参数定义详见 [配置项列表](config.md#配置项列表)。

命令行中指定的参数将覆盖配置文件参数设置。

!!! example

    ```powershell
    ipsc6.agent.wpfapp.exe Ipsc:ServerList:0="192.168.2.100" WebServer:ListenPort="8080"
    ```

## 从浏览器启动

座席客户端的安装程序在 Windows 注册表写入了如下的内容：

```Registry
Windows Registry Editor Version 5.00

[HKEY_CLASSES_ROOT\ipsc6-agent]
"URL Protocol"="\"\""
@="\"URL:ipsc6-agent-launch Protocol\""

[HKEY_CLASSES_ROOT\ipsc6-agent-launch\shell]

[HKEY_CLASSES_ROOT\ipsc6-agent-launch\shell\open]

[HKEY_CLASSES_ROOT\ipsc6-agent-launch\shell\open\command]
@="\"C:\\Program Files\\ipsc6-agent-launch\ipsc6.agent.launch.exe\" \"%1\""
```

这样，当 Windows 操作系统处理以 `ipsc6-agent-launch:` 开头的 URI 时，就会调用 `ipsc6.agent.launch.exe` 启动座席程序。 [^1]

例如这样的超链接:

!!! example

    ```html
    <a href="ipsc6-agent-launch:">启动座席程序</a>
    ```

点击这个超链接后，座席程序将会启动。

如果座席程序已经在运行，它将切换到前台显示。

我们甚至可以为启动连接加上命令行参数，例如：

!!! example

    ```html
    <a href="ipsc6-agent-launch:--WebServer:Port 8080">
        启动座席程序，在8080端口启动 WebServer
    </a>
    ```

    ```html
    <a href="ipsc6-agent-launch:WebServer:Port=8080 Ipsc:ServerList:0=10.10.10.1">
        启动座席程序，在8080端口启动 WebServer，连接 CTI 服务器 10.10.10.1
    </a>
    ```

`ipsc6.agent.launch.exe` 会自动查找并运行座席程序，查找顺序是:

1. 环境变量 `IPSC6AGENT_DIR` 定义的目录
1. 用户级安装的座席程序
1. 系统级安装的座席程序
1. `ipsc6.agent.launch.exe` 自身所在目录(通常是 `%ProgramFiles%\ipsc6-agent-launch`)
1. 如果以上都没有找到，就在当前工作目录（通常是 `C:\WINDOWS\system32`）尝试直接运行座席程序

!!! tip

    当座席程序不是使用安装包，而是手动复制文件（见 [安装](install.md)）部署时，启动器是无法找到座席程序的。此时，我们可以设置环境变量 `IPSC6AGENT_DIR` 为座席程序所在的目录。

[^1]: 根据 <https://docs.microsoft.com/previous-versions/windows/internet-explorer/ie-developer/platform-apis/aa767914(v=vs.85)> 提供的方法。

--8<-- "includes/glossary.md"
