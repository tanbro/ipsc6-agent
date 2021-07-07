# 呼叫信息

## 概述

座席程序用这个类型记录 CTI 服务器上与该座席有关的呼叫数据

## 属性

| Attribute      | Type      | Description                |
| -------------- | --------- | -------------------------- |
| `ctiIndex`     | `Integer` | 所属的 CTI 服务器索引值    |
| `channel`      | `Integer` | 通话的通道号               |
| `direction`    | `Integer` | [CallDirection][]枚举值    |
| `isHeld`       | `Boolean` | 该呼叫是否被当前座席保持   |
| `holdType`     | `Integer` | [HoldEventType][]枚举值    |
| `remoteTelNum` | `String`  | 远端的号码                 |
| `h24CallCount` | `Integer` | 该号码最近 24 小时呼叫次数 |
| `h48CallCount` | `Integer` | 该号码最近 48 小时呼叫次数 |
| `remoteLoc`    | `String`  | 远端的地理位置             |
| `workerNum`    | `String`  | 座席工号                   |
| `groupId`      | `String`  | 座席组 ID                  |
| `ivrPath`      | `String`  | IVR 路径                   |
| `customString` | `String`  | 随路数据                   |

!!! info

    `holdType` 反映了最进一次通话保持相关操作的类型，它仅在 `isHeld` 为 `true` 时有效。

[calldirection]: enums.md#呼叫方向
[holdeventtype]: enums.md#保持事件类型

--8<-- "includes/glossary.md"
