# 转接

## 转到座席

将当前正在进行的、或者被保持的通话转给其它座席。

- **Method**: `xferAgent`

- **Params**:

    |    Argument    | Data Type | Default |              Description              |
    | -------------- | --------- | ------- | ------------------------------------- |
    | `workerNum`    | `String`  | *       | 转接的目标座席工号                    |
    | `groupId`      | `String`  | *       | 转接的目标技能组                      |
    | `index`        | `Integer` | `null`  | 要转接的通话所属于的CTI服务器索引值 |
    | `channel`      | `Integer` | `null`  | 要转接的通话的通道号                  |
    | `customString` | `String`  | `""`    | 转接的随路数据                        |
    | `consultative` | `Boolean` | `false` | 是否使用咨询转（不释放）              |

    !!! note
        `workerNum` 与 `groupId` 两个参数必须只能指定一个。
        如果指定了其中一个，另一个应不提供，或者为 `null`、 空字符串。

    !!! note
        - 如果要转接当前正在进行的通话，必须将 `index` 与 `channel` 的值设置为`null`或者空字符串，或者不提供这两个参数。
        - 如果要转接保持列表中的通话，则应根据保持列表中[通话保持对象][]的属性填写 `index` 与 `channel` 参数

- **Result**: `null`

## 转到外线

{>>

TODO:

<<}

[通话保持对象]: ../../objects/hold_info.md
