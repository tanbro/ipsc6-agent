# 启动

## 启动参数

进程启动的参数，参数与配置项目的对应关系

## 从浏览器启动

如何浏览器启动

如下:

    [HKEY_CURRENT_USER\Software\Classes\mycustproto]
    "URL Protocol"="\"\""
    @="\"URL:MyCustProto Protocol\""
    
    [HKEY_CURRENT_USER\Software\Classes\mycustproto\DefaultIcon]
    @="\"mycustproto.exe,1\""
    
    [HKEY_CURRENT_USER\Software\Classes\mycustproto\shell]
    
    [HKEY_CURRENT_USER\Software\Classes\mycustproto\shell\open]
    
    [HKEY_CURRENT_USER\Software\Classes\mycustproto\shell\open\command]
    @="\"C:\\Program Files\\MyProgram\\myprogram.exe\" \"%1\""
