# 配置

座席程序从四个来源获取配置参数，后获取的参数覆盖之前获取的参数。

它们按顺序分别是:

1. 程序工作目录的 `Config/settings.json` 文件

    这个配置文件是**必须**的，应在分发时与可执行文件一同打包。
    系统级的用户无关设置，如服务器地址等，可以放在这里。

1. 当前 Windows 账户的 `%LOCALAPPDATA%\ipsc6.agent.wpfapp\User\settings.json` 文件

    这个配置文件是*可选的*。
    用户个性化的设置，如首选放音设备，可以放在这里。

1. `IPSC6AGENT_` 为前缀的环境变量
1. 传入到座席程序的命令启动参数

靠后的配置参数覆盖之前的。

座席程序的配置支持两级分层的键值对形式，我们用 `:` 作为键的层次分隔符。

## 配置方式

### JSON 配置文件

下面是一个具有各种配置设置的示例 settings.json 文件：

```json
{
    "Ipsc": {
        "LocalPort": 0,
        "LocalAddress": "0.0.0.0",
        "ServerList": ["192.168.2.207", "192.168.2.108"]
    },
    "WebServer": {
        "ListenPort": 9696
    },
    "Phone": {
        "LocalSipPort": 5060
    }
}
```

其中的多层键值，也可以采用非嵌套的方式书写，如:

```json
{
    "Ipsc:ServerList": ["192.168.2.207", "192.168.2.108"],
    "Ipsc:LocalPort": 0,
    "Ipsc:LocalAddress": "0.0.0.0",
    "WebServer:ListenPort": 9696,
    "Phone:LocalSipPort": 5060
}
```

或

```json
{
    "Ipsc:ServerList:0": "192.168.2.207",
    "Ipsc:ServerList:1": "192.168.2.108",
    "Ipsc:LocalPort": 0,
    "Ipsc:LocalAddress": "0.0.0.0",
    "WebServer:ListenPort": 9696,
    "Phone:LocalSipPort": 5060
}
```

### 环境变量配置

座席程序将带有前缀 `IPSC6AGENT_` 的环境变量视作配置项键值。

由于环境变量不支持 `:` 分隔符，我们改用双下划线 (`__`)，它将被程序自动替换为 `:` 。

假设我们使用环境变量配置本地 Web 服务器端口 `WebServer:ListenPort` 选项为`8080`，则应这样设定环境变量:

```bat
set IPSC6AGENT_WebServer__ListenPort=8080
```

数组选项比较特殊，我们需要把数组的索引值视为键名。
例如，我们可以这样设置 `0`, `1` 两个要连接的 CTI 服务器地址:

```bat
set IPSC6AGENT_Ipsc__ServerList__0="192.168.2.100"
set IPSC6AGENT_Ipsc__ServerList__1="192.168.2.200"
```

### 命令行参数配置

座席程序可以从命令行参数键值对中加载配置。其用法与使用环境变量配置的方式一致，除了:

1. 不需要前缀
1. 使用 `:` 而不是双下划线 (`__`) 作为分隔符。

假设我们使用命令行参数配置 CTI 服务器地址列表为 192.168.2.100 与 192.168.2.101，那么启动命令应是：

```powershell
ipsc6.agent.wpfapp.exe --Ipsc:ServerList:0 "192.168.2.100" --Ipsc:ServerList:1 "192.168.2.101"
```

命令行参数中的配置项键值对有多种写法，其规则是：

-   键与值要使用 `=` 连接；除非键之前具有 `--` 或 `/` 前缀，此时可以用空格连接键与值。
-   如果使用 `=`，则值不是必需的。 例如 `SomeKey=`。

例如：

-   以下命令使用 `=` 设置键和值：

    ```powershell
    app.exe SecretKey="Secret key from command line"
    ```

-   以下命令使用 `/` 设置键和值：

    ```powershell
    app.exe /SecretKey "Secret key set from forward slash"
    ```

-   以下命令使用 `--` 设置键和值：

    ```powershell
    app.exe --SecretKey "Secret key set from double hyphen"
    ```

!!! warning
在同一命令中，请勿将使用 `=` 的命令行参数键值对与使用空格的键值对混合使用。

## 配置项列表

### CTI 服务器网络连接配置

-   本地端口

    连接 CTI 服务器端时候，要使用的本地网络端口。

    设置为 `0` 表示自动分配端口。

    | key              | type      | required | default |
    | ---------------- | --------- | -------- | ------- |
    | `Ipsc:LocalPort` | `Integer` |          | `0`     |

-   本地地址

    连接 CTI 服务器端时候，要使用的本地网络地址。

    设置为空字符串表示在所有地址上监听。

    | key                 | type      | required | default |
    | ------------------- | --------- | -------- | ------- |
    | `Ipsc:LocalAddress` | `Integer` |          | `""`    |

-   CTI 服务器地址列表

    座席要连接的 CTI 服务器地址列表

    | key                 | type           | required | default |
    | ------------------- | -------------- | -------- | ------- |
    | `Ipsc:LocalAddress` | `List<String>` | ✔️       |         |

### 内嵌 Web 服务器配置

-   Web 服务网络端口

    | key                    | type      | required | default |
    | ---------------------- | --------- | -------- | ------- |
    | `WebServer:ListenPort` | `Integer` |          | `0`     |

    座席程序的嵌入 WebServer 模块在本地回环地址上打开这个端口，作为 HTTP 服务器进行网络通信。

    如设置 `0`，座席程序将使用 `9696` 端口。

### 软电话配置

-   本地 SIP 网络端口

    座席程序的软电话模块在所有的网络接口上使用这个端口进行双向的 SIP 网络通信。

    如设置 `0`，座席程序将使用 SIP 协议默认的 `5060` 端口。

    | key                  | type      | required | default |
    | -------------------- | --------- | -------- | ------- |
    | `Phone:LocalSipPort` | `Integer` |          | `0`     |

-   默认音频输入设备

    如设置为空字符串，座席程序将使用系统默认音频输入设备。

    | key                      | type     | required | default |
    | ------------------------ | -------- | -------- | ------- |
    | `Phone:AudioInputDevice` | `String` |          | `""`    |

-   默认音频输出设备

    如设置为空字符串，座席程序将使用系统默认音频输出设备。

    | key                       | type     | required | default |
    | ------------------------- | -------- | -------- | ------- |
    | `Phone:AudioOutputDevice` | `String` |          | `""`    |

### 其它设置

-   工号

    如果设置了这个选项，座席程序在登录界面出现时，将这个选项的值自动填写到登录表单的“工号”文本框中。

    | key                   | type     | required | default |
    | --------------------- | -------- | -------- | ------- |
    | `Misc:SavedWorkerNum` | `String` |          |         |

-   是否存储工号

    如果设选项为 `true`，座席程序会在登录成功后，将工号记录在 `%LOCALAPPDATA%\ipsc6.agent.wpfapp\User\settings.json` 文件的 `Misc.SavedWorkerNum` 属性中。

    | key                    | type      | required | default |
    | ---------------------- | --------- | -------- | ------- |
    | `Misc:IsSaveWorkerNum` | `Boolean` |          | `false` |
