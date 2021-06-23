# 排队信息

## 概述

座席程序用这个类型记录 CTI 服务器上与该座席有关的排队信息数据

## 属性

| Attribute      | Type      | Description             |
| -------------- | --------- | ----------------------- |
| `ctiIndex`     | `Integer` | 所属的 CTI 服务器索引值 |
| `channel`      | `Integer` | 通话的通道号            |
| `id`           | `String`  | ID                      |
| `queueType`    | `Integer` | [排队类型][]枚举值      |
| `workerNum`    | `String`  | 座席工号                |
| `skills`       | `Array`   | 技能组数组              |
| `customString` | `String`  | 随路数据                |

`skills`
: 这次排队相关的技能组数组，数组元素是 [Skill][] 对象。如果是指定座席而不是按技能排的，数组为空。

[排队类型]: enums.md#排队类型
[skill]: skill.md

--8<-- "includes/glossary.md"
