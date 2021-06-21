# CTI 服务器

## 概要

座席程序的接口中，使用这个对象表示客户端连接到 CTI 服务节点。

一个客户端可以同时连接多个不同的 CTI 服务器。

## 属性

| Attribute | Type      | Description                              |
| --------- | --------- | ---------------------------------------- |
| `index`   | `Integer` | CTI 服务器索引值(从 0 开始)              |
| `host`    | `String`  | CTI 服务器的网络地址                     |
| `port`    | `Integer` | CTI 服务器的网络访问端口(0 表示默认端口) |
| `isMain`  | `Boolean` | 是否主连接                               |
| `state`   | `Integer` | [连接状态][]枚举值                           |

[连接状态]: ./enums.md#CTI%20服务器连接状态
