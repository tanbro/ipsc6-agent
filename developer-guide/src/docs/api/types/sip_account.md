# SIP 账户

## 概述

CTI 服务器在座席客户端登录后，告知其一个或多个 SIP 账户。
该对象主要用于记录 SIP 账户的注册状态。

## 属性

| Attribute           | Type      | Description                       |
| ------------------- | --------- | --------------------------------- |
| `ctiIndex`          | `Integer` | 对应的 CTI 服务器索引值           |
| `isValid`           | `Boolean` | 账户是否有效                      |
| `id`                | `Integer` | 账户 ID                           |
| `uri`               | `String`  | 账户 URI                          |
| `isRegisterActive`  | `Boolean` | 账户的注册会话是否存活            |
| `lastRegisterError` | `Integer` | 最近的注册错误码。`0`表示注册成功 |

--8<-- "src/includes/glossary.md"
