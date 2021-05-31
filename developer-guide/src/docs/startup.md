# 启动

## 启动参数

座席程序接受 `JSON` 格式的字符串作为参数。

参数名与参数的定义与 [配置](config.md) 一节所述相同。

eg:

```bat
"C:\Program Files (x86)\hesong\ipsc6-agent\ipsc6.agent.wpfapp.exe" ^
'{ ^
    "ServerAddress": ["192.168.2.100", "192.168.2.200"], ^
    "LocalSipPort": 5060, ^
    "LocalWebServerPort": 9876 ^
}'
```

## 从浏览器启动

如何浏览器启动

如下:

```Registry
[HKEY_CURRENT_USER\Software\Classes\mycustproto]
"URL Protocol"="\"\""
@="\"URL:MyCustProto Protocol\""

[HKEY_CURRENT_USER\Software\Classes\mycustproto\DefaultIcon]
@="\"mycustproto.exe,1\""

[HKEY_CURRENT_USER\Software\Classes\mycustproto\shell]

[HKEY_CURRENT_USER\Software\Classes\mycustproto\shell\open]

[HKEY_CURRENT_USER\Software\Classes\mycustproto\shell\open\command]
@="\"C:\\Program Files\\MyProgram\\myprogram.exe\" \"%1\""
```
