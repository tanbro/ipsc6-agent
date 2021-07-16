# 启动

## 进程启动参数

座席程序的命令行参数详见 [配置项列表](config.md#命令行参数配置) 中的相同。这些参数将覆盖配置文件中的设置。

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
@="\"C:\\Program Files(x86)\\ipsc6.agent.wpfapp\ipsc6.agent.launch.exe\" \"%1\""
```

这样就可以通过 `ipsc6-agent` Protocol 启动程序。
`ipsc6.agent.launch.exe` 专用于座席程序的 Windows Shell 启动。

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
    <a href="ipsc6-agent-launch:--WebServer:Port 8080">启动座席程序</a>
    ```

--8<-- "includes/glossary.md"
