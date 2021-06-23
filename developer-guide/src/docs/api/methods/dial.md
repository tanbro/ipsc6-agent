# 拨号相关方法

## 呼叫外线

-   **Method**: `dial`

-   **Params**:

    | Argument        | Data Type | Default | Description |
    | --------------- | --------- | ------- | ----------- |
    | `calledTelNum`  | `String`  | -       | 被叫号码    |
    | `callingTelNum` | `String`  | `""`    | 主叫号码    |
    | `channelGroup`  | `String`  | `""`    | 通道组      |
    | `option`        | `String`  | `""`    | 其它参数    |

-   **Result**: `null`

## 转接座席

-   **Method**: `xfer`

-   **Params**:

    | Argument       | Data Type | Default | Description                           |
    | -------------- | --------- | ------- | ------------------------------------- |
    | `ctiIndex`     | `Integer` | -       | 要转接的呼叫所属于的 CTI 服务器索引值 |
    | `channel`      | `Integer` | -       | 要转接的呼叫的通道号                  |
    | `groupId`      | `String`  | -       | 转接目标座席所属的组 ID               |
    | `workerNum`    | `String`  | `""`    | 转接目标座席队工号                    |
    | `customString` | `String`  | `""`    | 随路数据                              |

-   **Result**: `null`

## 咨询座席

-   **Method**: `xferConsult`

-   **Params**:

    | Argument       | Data Type | Default | Description             |
    | -------------- | --------- | ------- | ----------------------- |
    | `groupId`      | `String`  | -       | 转接目标座席所属的组 ID |
    | `workerNum`    | `String`  | `""`    | 转接目标座席队工号      |
    | `customString` | `String`  | `""`    | 随路数据                |

-   **Result**: `null`

## 转接外线

-   **Method**: `xferExt`

-   **Params**:

    | Argument        | Data Type | Default | Description                           |
    | --------------- | --------- | ------- | ------------------------------------- |
    | `ctiIndex`      | `Integer` | -       | 要转接的呼叫所属于的 CTI 服务器索引值 |
    | `channel`       | `Integer` | -       | 要转接的呼叫的通道号                  |
    | `calledTelNum`  | `String`  | -       | 被叫号码                              |
    | `callingTelNum` | `String`  | `""`    | 主叫号码                              |
    | `channelGroup`  | `String`  | `""`    | 通道组                                |
    | `option`        | `String`  | `""`    | 其它参数                              |

-   **Result**: `null`

## 咨询外线

-   **Method**: `xferExtConsult`

-   **Params**:

    | Argument        | Data Type | Default | Description |
    | --------------- | --------- | ------- | ----------- |
    | `calledTelnum`  | `String`  | -       | 被叫号码    |
    | `callingTelnum` | `String`  | `""`    | 主叫号码    |
    | `channelGroup`  | `String`  | `""`    | 通道组      |
    | `option`        | `String`  | `""`    | 其它参数    |

-   **Result**: `null`

--8<-- "includes/glossary.md"
