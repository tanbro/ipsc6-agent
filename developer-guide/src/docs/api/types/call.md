# 呼叫

## 概述

座席程序用这个类型记录 CTI 服务器上与该座席有关的呼叫数据

## 属性

| Attribute      | Type      | Description                                            |
| -------------- | --------- | ------------------------------------------------------ |
| `ctiIndex`     | `Integer` | 所属的 CTI 服务器索引值                                |
| `channel`      | `Integer` | 通话的通道号                                           |
| `direction`    | `Integer` | [呼叫方向][]枚举值                                     |
| `isHeld`       | `Boolean` | 该呼叫是否被当前座席保持                               |
| `holdType`     | `Integer` | [保持事件类型][]枚举值，仅在 `isHeld` 为 `true` 时有效 |
| `remoteTelNum` | `String`  | 远端的号码                                             |
| `remoteLoc`    | `String`  | 远端的地理位置                                         |
| `workerNum`    | `String`  | 座席工号                                               |
| `skillGroupId` | `String`  | 技能组 ID                                              |
| `ivrPath`      | `String`  | IVR 路径                                               |
| `customString` | `String`  | 随路数据                                               |

[呼叫方向]: enums.md#呼叫方向
[保持事件类型]: enums.md#保持事件类型
