# SIP 呼叫

## 概述

座席程序内部的 SIP 呼叫对象

!!! info

    座席程序内部的 SIP 呼叫对象与记录 CTI 服务器端呼叫信息的 [Call][] 对象是不同的。

## 属性

| Attribute   | Type      | Description       |
| ----------- | --------- | ----------------- |
| `id`        | `Integer` | SIP 呼叫的内部 ID |
| `localUri`  | `String`  | 本地 SIP URI      |
| `remoteUri` | `Boolean` | 远端 SIP URL      |

[call]: call.md

--8<-- "includes/glossary.md"
