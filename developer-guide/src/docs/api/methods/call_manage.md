# 呼叫管控

## 监听

以只听模式进入正在进行的座席通话

- **Method**: `monitor`

- **Params**:

    |  Argument   | Data Type | Default | Description  |
    | ----------- | --------- | ------- | ------------ |
    | `workerNum` | `String`  | -       | 目标座席工号 |

- **Result**: `null`

## 取排队

见 [排队 → 取排队](queue.md#取排队)

## 拦截

拦截某座席正在振铃的排队

- **Method**: `intercept`

- **Params**:

    |  Argument   | Data Type | Default | Description  |
    | ----------- | --------- | ------- | ------------ |
    | `workerNum` | `String`  | -       | 目标座席工号 |

- **Result**: `null`

## 插话(强插)

以听+说模式进入正在进行的座席通话

- **Method**: `Interrupt`

- **Params**:

    |  Argument   | Data Type | Default | Description  |
    | ----------- | --------- | ------- | ------------ |
    | `workerNum` | `String`  | -       | 目标座席工号 |

- **Result**: `null`

## 打断(强拆)

将正在进行的座席通话强行挂断

- **Method**: `Interrupt`

- **Params**:

    |  Argument   | Data Type | Default | Description  |
    | ----------- | --------- | ------- | ------------ |
    | `workerNum` | `String`  | -       | 目标座席工号 |

- **Result**: `null`
