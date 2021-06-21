# 排队信息对象

## 概述

座席程序用这个类型记录 CTI 服务器上与该座席有关的呼叫数据

## 属性

|   Attribute    |       Type        |        Description        |
| -------------- | ----------------- | ------------------------- |
| `index`        | `Integer`         | 呼叫对应的CTI服务器索引值 |
| `channel`      | `Integer`         | 呼叫的通道号              |
| `id`           | `String`          | 排队的ID                  |
| `sessionId`    | `String`          | 排队的会话ID              |
| `type`         | [排队类型][] 枚举 | 排队的类型                |
| `CallingNo`    | `String`          | 主叫号码                  |
| `workerNum`    | `String`          | 排队的目标工号            |
| `customString` | `String`          | 随路数据                  |

[排队类型]: ../enums/queue_type.md
