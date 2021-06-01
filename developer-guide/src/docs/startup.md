# 启动

## 进程启动参数

座席程序的命令行参数与 [配置项列表](config.md#配置项列表) 中的相同。这些参数将覆盖配置文件中的设置。

eg:

```powershell
ipsc6.agent.wpfapp.exe -ServerAddress 192.168.2.107 -LocalWebServerPort 8080
```

## 从浏览器启动

座席客户端的安装程序在 Windows 注册表写入了如下的内容：

```Registry
[HKEY_CLASSES_ROOT\ipsc6-agent-app]
"URL Protocol"="\"\""
@="\"URL:ipsc6-agent-app Protocol\""

[HKEY_CLASSES_ROOT\ipsc6-agent-app\shell]

[HKEY_CLASSES_ROOT\ipsc6-agent-app\shell\open]

[HKEY_CLASSES_ROOT\ipsc6-agent-app\shell\open\command]
@="\"C:\\Program Files(x86)\\HesongInfoTech\\ipsc6.agent.wpfapp\ipsc6.agent.wpfapp.exe\" \"%1\" \"%*\""
```

这样就可以通过 `ipsc6-agent-app://` Protocol 启动程序。

例如这样的超链接:

```html
<a href="ipsc6-agent-app:///">启动座席程序</a>
```

点击这个超链接后，座席程序将会启动。

如果座席程序已经在运行，它将切换到前台显示。
