# 排队信息

## 概述

座席程序用这个类型记录 CTI 服务器上与该座席有关的排队信息数据

## 属性

| Attribute      | Type      | Description             |
| -------------- | --------- | ----------------------- |
| `ctiIndex`     | `Integer` | 所属的 CTI 服务器索引值 |
| `channel`      | `Integer` | 通话的通道号            |
| `id`           | `String`  | ID                      |
| `queueType`    | `Integer` | [QueueInfoType][]枚举值 |
| `workerNum`    | `String`  | 座席工号                |
| `groups`       | `Array`   | 座席组数组              |
| `customString` | `String`  | 随路数据                |

`groups`
: 这次排队相关的技能组数组，数组元素是 [Group][] 对象。如果该排队是指定座席的而不是指定组的，则此数组为空。

[queueinfotype]: enums.md#排队信息类型
[group]: group.md

--8<-- "includes/glossary.md"
