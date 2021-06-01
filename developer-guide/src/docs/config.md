# 配置

## 配置文件

当座席程序要使用某个配置项时，按照以下顺序读取配置文件:

1. 查找文件: `%LOCALAPPDATA%\HesongInfoTech\ipsc6.agent.wpfapp.ex_Url_<Hash>\<Version>\user.config`

    其中:

    - `<Hash>` 是当前可执行文件的散列值
    - `<Version>` 是当前可执行文件的版本

    如:

    ```powershell
    C:\Users\XiaoMing\AppData\Local\HesongInfoTech\ipsc6.agent.wpfapp.ex_Url_ui3sld3rvnbni5b2w0bkdv3bqklj3fwi\1.0.1.3\user.config
    ```

    如果文件存在，且文件中具有要使用的配置项，则应用该配置。

    这个配置文件是用户级的。

1. 如果上述文件不存在，或者文件存在但没有要使用的配置项，就读取可执行文件所在目录下的配置文件 `ipsc6.agent.wpfapp.exe.config`，并应用其中的配置。

    这个配置文件是系统级的，它一般用于存放配置的默认值。

上述 XML 配置文件的 `configuration\userSettings\ipsc6.agent.wpfapp.Properties.Settings` 节点，用于存放座席程序的用户配置选项。

!!! warning

    - 不要修改 `ipsc6.agent.wpfapp.exe.config` 的其它部分
    - 修改时，务必保证格式的正确

## 配置项列表

-   CTI 服务器地址

    | name              | serializeAs |
    | ----------------- | ----------- |
    | `ServerAddresses` | `String`    |

    座席要连接到 CTI 服务器(集群)地址(列表)。当有多个服务器要连接时，用 `;` 分隔多个地址，如：

    ```xml
    <setting name="CtiServerAddress" serializeAs="String">
      <value>192.168.2.107;192.168.2.207</value>
    </setting>
    ```

-   本地 SIP 监听端口

    | name           | serializeAs |
    | -------------- | ----------- |
    | `LocalSipPort` | `UInt16`    |

    座席程序的软电话模块在所有的网络接口上使用这个端口进行双向的 SIP 网络通信。

-   嵌入 Web 服务器端口

    | name           | serializeAs |
    | -------------- | ----------- |
    | `LocalSipPort` | `UInt16`    |

    座席程序的嵌入 WebServer 模块在本地回环地址上打开这个端口，作为 HTTP 服务器进行网络通信。

-   默认音频输入设备

    | name               | serializeAs |
    | ------------------ | ----------- |
    | `AudioInputDevice` | `String`    |

    如设置为空字符串，座席程序将使用系统默认音频输入设备。

-   默认音频输出设备

    | name                | serializeAs |
    | ------------------- | ----------- |
    | `AudioOutputDevice` | `String`    |

    如设置为空字符串，座席程序将使用系统默认音频输出设备。
