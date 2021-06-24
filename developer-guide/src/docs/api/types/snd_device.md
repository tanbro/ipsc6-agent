# 音频设备

## 概述

座席程序 SIP 模块使用的本地音频设备

## 属性

| Attribute                 | Type      | Description                                               |
| ------------------------- | --------- | --------------------------------------------------------- |
| `id`                      | `Integer` | SIP 模块内部为设备分配的 ID                               |
| `name`                    | `String`  | 设备名称                                                  |
| `driver`                  | `String`  | 驱动名称                                                  |
| `inputCount`              | `Integer` | 该设备支持的最大输入通道数。`0`表示该设备不支持声音采集。 |
| `outputCount`             | `Integer` | 该设备支持的最大输出通道数。`0`表示该设备不支持声音播放。 |
| `isCurrentPlaybackDevice` | `Boolean` | 该设备是否为当前正在使用的播放设备                        |
| `isCurrentCaptureDevice`  | `Boolean` | 该设备是否为当前正在使用的采集设备                        |

--8<-- "includes/glossary.md"
