# 座席

## 概述

这是座席程序的核心对象。它集中记录了座席的状态和各种相关数据。

## 属性

### workerNum

`workerNum`
: 工号

    |  Attribute  |   Type   |
    | ----------- | -------- |
    | `workerNum` | `String` |

    系统中，每个座席的工号都是唯一的。

### ctiServerCollection

`ctiServerCollection`
: CTI 服务器连接数组

    |       Attribute       |  Type   |            Remark             |
    | --------------------- | ------- | ----------------------------- |
    | `ctiServerCollection` | `Array` | 数组元素是 [CtiServer][] 对象 |

    座席客户端可同时连接多个 CTI 服务器节点。该数组属性存放一个或多个服务连接的信息对象实例。

    数组元素的类型是 [CtiServer][] 对象

### state

`state`
: 座席状态

    | Attribute |   Type    |        Remark         |
    | --------- | --------- | --------------------- |
    | `state`   | `Integer` | [AgentState][] 枚举值 |

    座席状态用 [AgentState][] 枚举值对应的整数表示

### workType

`workType`
: 座席工作类型

    | Attribute  |   Type    |       Remark        |
    | ---------- | --------- | ------------------- |
    | `workType` | `Integer` | [WorkType][] 枚举值 |


    工作类型有两重含义:

    -   当座席处理“工作状态”(正在进行通话)时：表示通话的类型和状态，如呼出、通话保持
    -   当座席处理“非工作状态”(没有进行通话)时：表示“示忙”的类型，如离开、小休等

### callCollection

`callCollection`
: 与该座席相关的呼叫数组

    |    Attribute     |  Type   |          Remark          |
    | ---------------- | ------- | ------------------------ |
    | `callCollection` | `Array` | 数组元素是 [Call][] 对象 |

    !!! important

        每当座席相关的呼叫信息发生变化，如来电振铃、通话结束、通话保持、外呼等，该数组中的数据会改变。

        但是，只要座席的通话不结束，这个数组中的元素就不会被移除。只有当座席的所有通话都结束，状态改变为“非工作”时，这个列表才会清空。
        
        例如：座席正在进行通话，同时还有一个通话被保持。此时，如果保持的通话挂断，这个通话对应的对象不会从数组中移除。

### sipAccountCollection

`sipAccountCollection`
: 与该座席相关的呼叫数组

    |       Attribute        |  Type   |             Remark             |
    | ---------------------- | ------- | ------------------------------ |
    | `sipAccountCollection` | `Array` | 数组元素是 [SipAccount][] 对象 |

    每当座席 SIP 注册状态发生变化，该数组中的数据会改变。

    当本地 SIP 呼叫建立或释放，如接听、挂断、启动外呼时，对应的 [SipAccount][] 对象的 [SipCall][] 数组元素会出现增减。

[ctiserver]: cti_server.md
[agentstate]: enums.md#座席状态
[worktype]: enums.md#座席工作类型
[call]: call.md
[sipaccount]: sip_account.md
[sipcall]: sip_call.md

--8<-- "includes/glossary.md"
